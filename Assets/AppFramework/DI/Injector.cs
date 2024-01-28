//#define LOG_VERBOSE
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Debug = UnityEngine.Debug;

namespace Spine.DI {
	[MeansImplicitUse]
	public sealed class InjectAttribute : Attribute {
		public readonly bool isOptional = false;

		public InjectAttribute() {
		}

		public InjectAttribute(bool optional) {
			isOptional = optional;
		}
	}

	/// <summary>
	/// Injector simple implementation
	/// </summary>
	public class Injector {
		private readonly DependencyRepository repository = new();

		public delegate object DependencyProviderDelegate(object target);

		private readonly ConcurrentDictionary<Type, DependencyProviderDelegate> mappings = new();

		[Conditional("LOG_VERBOSE")]
		private static void Log(object msg) => Debug.Log( $"[{nameof(Injector)}] {msg}" );

		private static void LogError(object msg) => Debug.LogError( $"[{nameof(Injector)}] {msg}" );

		public Injector() {
			Log( "→ Injector ←" );
		}

		/// <summary>
		/// Maps dependency by it's Type
		/// </summary>
		/// <param name="instances">A list of instances of unique types</param>
		public void AutoMap(params object[] instances) {
			foreach (var item in instances) {
				repository.Set( item.GetType(), item );
			}
		}

		public void MapSingleton<T>() where T : new() {
			Map<T>( target => {
				var result = repository.Get( typeof(T) );
				if (result == null) {
					result = Resolve<T>();
					repository.Set( typeof(T), result );
				} else {
					throw new Exception( $"Singleton<{typeof(T)}>: already mapped" );
				}

				return result;
			} );
		}

		public void MapSingleton<T>(T instance) {
			mappings.TryAdd( typeof(T), target => instance );
		}

		public void Map<T>(DependencyProviderDelegate provider) {
			mappings.TryAdd( typeof(T), provider );
		}

		// V3 //
		internal interface IDependencyProvider {
			object Get();
		}

		internal struct InstanceProvider<TDependency> : IDependencyProvider {
			private readonly TDependency dependency;
			private readonly Injector injector;

			private bool initialized;

			public InstanceProvider(TDependency dependency, Injector injector) {
				this.dependency = dependency;
				this.injector = injector;
				initialized = false;
			}

			public object Get() {
				if (!initialized) {
					injector.InjectIn( dependency );
					initialized = true;
				} 
				return dependency;
			}
		}
		
		internal struct SingletonProvider<TDependency> : IDependencyProvider where TDependency : new() {
			public Injector injector;
			private TDependency instance;

			public object Get() {
				if (instance is null) {
					Debug.Log( $"[SingletonProvider<{nameof(TDependency)}>] Get, create instance" );
					instance = new TDependency();
					injector.InjectIn( instance );
				}
				return instance;
			}
		}

		private readonly Dictionary<Type, IDependencyProvider> providers = new();

		public void Add<TDependency>(TDependency dependency) {
			Log( $"Add<{nameof(TDependency)}>({dependency})" );
			providers.Add( typeof(TDependency), new InstanceProvider<TDependency>( dependency, this ) );
		}

		public void Add<TType, TImplementation>() where TImplementation : TType, new() {
			Log( $"Add<{nameof(TType)},{typeof(TImplementation)}>" );
			providers.Add( typeof(TType), new SingletonProvider<TImplementation> {injector = this} );
		}

		public void Add<TDependency>() where TDependency : new() {
			Log( $"Add<{nameof(TDependency)}>" );
			Add<TDependency, TDependency>();
		}

		public T Get<T>() {
			if (providers.TryGetValue( typeof(T), out var provider )) {
				return (T)provider.Get();
			}

			return default;
		}

		public T Retrieve<T>() {
			if (providers.TryGetValue( typeof(T), out var dependencyProvider )) {
				return (T)dependencyProvider.Get();
			}

			if (mappings.TryGetValue( typeof(T), out var dependencyProviderAction )) {
				return (T)dependencyProviderAction( null );
			}

			return default;
		}

		public object Retrieve(Type type) {
			if (providers.TryGetValue(type, out var dependencyProvider)) {
				return dependencyProvider.Get();
			}

			if (mappings.TryGetValue(type, out var dependencyProviderAction)) {
				return dependencyProviderAction(null);
			}

			return null;
		}

		/// <summary>
		/// Resolve target dependencies marked with Inject Attribute
		/// </summary>
		/// <param name="target"></param>
		public void InjectIn(object target) {
			Log( $"InjectIn: {target}" );

			var injectionPoints = TypeDescriber.GetInjectionPoints( target.GetType() );

			foreach (var injection in injectionPoints) {
				Log( $"\tpoint: {injection.Name} : {injection.TargetType}" );
				var dependency = repository.Get( injection.TargetType );

				if (dependency == null && providers.TryGetValue( injection.TargetType, out var provider )) {
					dependency = provider.Get();
				}
				
				if (dependency == null && mappings.TryGetValue( injection.TargetType, out var providerAction )) {
					dependency = providerAction( target );
				}

				if (dependency == null && injection.isRequired) {
					LogError( $">> \t{injection.Name} = <i>[missing required reference]</i>({injection.TargetType.Name})" );
					continue;
				}

				Log( $"\tinject: {target} ← {dependency}" );
				injection.Inject( target, dependency );
			}
		}

		public T Resolve<T>() {
			Log( $"Resolve<{typeof(T)}>" );

			var targetType = typeof(T);
			var injectionPoint = TypeDescriber.GetConstructorInjectionPoints(targetType).FirstOrDefault();
			

			object targetBoxed = default(T);

			if (injectionPoint is ConstructorInjectionPoint constructorInjectionPoint) {
				targetBoxed = Activator.CreateInstance(typeof(T), constructorInjectionPoint.Parameters.Select( p => repository.Get( p.ParameterType ) ).ToArray());
			}

			// If there's no constructor injection point, use the default constructor
			targetBoxed ??= Activator.CreateInstance<T>();

			InjectIn( targetBoxed );

			return (T)targetBoxed;
		}
		
		
		/// <summary>
		/// Clears all the dependencies marked with Inject Attribute
		/// </summary>
		/// <param name="target"></param>
		public void Clear(object target) {
			var injectionPoints = TypeDescriber.GetInjectionPoints( target.GetType() );
			foreach (var injection in injectionPoints) {
				injection.Inject( target, null );
			}
		}
	}

	/// <summary>
	/// Dependency Storage and Provider
	/// </summary>
	internal sealed class DependencyRepository {
		private readonly ConcurrentDictionary<Type, object> items = new();
    
		public void Set(Type key, object item) {
			if (!items.TryAdd(key, item)) {
				Debug.LogError($"Two or more references of the same type <b>{key}</b> detected!");
			}
		}

		public object Get(Type key) {
			items.TryGetValue(key, out var value);
			return value;
		}
	}


	/// <summary>
	/// Provides an information about Type injection points
	/// </summary>
	public static class TypeDescriber // : ITypeDescriber
	{
		private static readonly ConcurrentDictionary<Type, IEnumerable<IInjectionPoint>> injectionPointsCache = new();
		private static readonly ConcurrentDictionary<Type, IEnumerable<IInjectionPoint>> cInjectionPointsCache = new();

		public static IEnumerable<IInjectionPoint> GetInjectionPoints(Type targetType) {
			// try to restore from the cache
			if (injectionPointsCache.TryGetValue( targetType, out var cached )) {
				return cached;
			}

			// TODO: configure collectors(DIPoint providers) through the context config

			// collect all the fields marked by the Inject attribute
			var diPoints = CollectInjectionPoints( targetType );
			
			// cache it
			injectionPointsCache[targetType] = diPoints;

			return diPoints;
		}
		
		public static IEnumerable<IInjectionPoint> GetConstructorInjectionPoints(Type targetType) {
			// try to restore from the cache
			if (cInjectionPointsCache.TryGetValue( targetType, out var cached )) {
				return cached;
			}
			
			// collect all the fields marked by the Inject attribute
			var diPoints = CollectConstructorInjectionPoints( targetType );
			
			// cache it
			cInjectionPointsCache[targetType] = diPoints;
			
			return diPoints;
		}

		private static List<IInjectionPoint> CollectInjectionPoints(Type targetType) {
			var result = new List<IInjectionPoint>();
			var members = targetType.GetMembers( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			foreach (var member in members) {
				var attribute = (InjectAttribute)Attribute.GetCustomAttribute( member, typeof(InjectAttribute), true );
				if (attribute != null) {
					var isRequired = !attribute.isOptional;
					switch (member) {
						case FieldInfo field:
							result.Add( new FieldInjectionPoint( field, isRequired ) );
							break;
						case PropertyInfo property:
							result.Add( new PropertyInjectionPoint( property, isRequired ) );
							break;
					}
				}
			}

			return result;
		}

		private static List<IInjectionPoint> CollectConstructorInjectionPoints(Type targetType) {
			var result = new List<IInjectionPoint>();

			// Add constructor injection points
			var constructors = targetType.GetConstructors();
			foreach (var constructor in constructors) {
				var attribute = (InjectAttribute)Attribute.GetCustomAttribute(constructor, typeof(InjectAttribute), true);
				var isRequired = attribute is { isOptional: true };
				result.Add(new ConstructorInjectionPoint(constructor, isRequired));
			}
			
			return result;
		}
	}

	// DIPoint
	public interface IInjectionPoint {
		bool isRequired { get; }

		string Name { get; }

		Type TargetType { get; }

		void Inject(object target, object value);
	}

	// Field
	internal readonly struct FieldInjectionPoint : IInjectionPoint {
		private readonly FieldInfo field;
		
		public bool isRequired { get; }

		public Type TargetType => field.FieldType;

		public string Name => field.Name;

		public FieldInjectionPoint(FieldInfo field, bool isRequired) {
			this.field = field;
			this.isRequired = isRequired;
		}

		public void Inject(object target, object value) {
			object finalValue = value;
			
			if (!field.FieldType.IsInstanceOfType(value)) {
				finalValue = Convert.ChangeType(value, field.FieldType);
			}
			
			field.SetValue(target, finalValue);
		}
	}


	// Property
	internal readonly struct PropertyInjectionPoint : IInjectionPoint {
		private readonly PropertyInfo property;
		
		public bool isRequired { get; }

		public Type TargetType => property.PropertyType;

		public string Name => property.Name;

		public PropertyInjectionPoint(PropertyInfo property, bool isRequired) {
			this.property = property;
			this.isRequired = isRequired;
		}

		public void Inject(object target, object value) {
			object finalValue = value;

			if (!property.PropertyType.IsInstanceOfType(value)) {
				finalValue = Convert.ChangeType(value, property.PropertyType);
			}

			property.SetValue(target, finalValue);
		}
	}

	internal readonly struct ConstructorInjectionPoint : IInjectionPoint {
		private readonly ConstructorInfo constructor;
		
		public bool isRequired { get; }

		public Type TargetType => constructor.DeclaringType;

		public string Name => constructor.Name;
		
		public ParameterInfo[] Parameters => constructor.GetParameters();

		public ConstructorInjectionPoint(ConstructorInfo constructor, bool isRequired) {
			this.constructor = constructor;
			this.isRequired = isRequired;
		}

		public void Inject(object target, object value) {
			
		}
	}
}
