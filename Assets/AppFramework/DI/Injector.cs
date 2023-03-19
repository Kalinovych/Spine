//#define LOG_VERBOSE
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		private readonly Dictionary<Type, DependencyProviderDelegate> mappings = new();

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
				repository.Put( item.GetType(), item );
			}
		}

		public void MapSingleton<T>() where T : new() {
			Map<T>( target => {
				var result = repository.Retrieve( typeof(T) );
				if (result == null) {
					result = Resolve<T>();
					repository.Put( typeof(T), result );
				}

				return result;
			} );
		}

		public void MapSingleton<T>(T instance) {
			mappings.Add( typeof(T), target => instance );
		}

		public void Map<T>(DependencyProviderDelegate provider) {
			mappings.Add( typeof(T), provider );
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

		/// <summary>
		/// Resolve target dependencies marked with Inject Attribute
		/// </summary>
		/// <param name="target"></param>
		public void InjectIn(object target) {
			Log( $"InjectIn: {target}" );

			var injectionPoints = TypeDescriber.GetInjectionPoints( target.GetType() );

			foreach (var injection in injectionPoints) {
				Log( $"\tpoint: {injection.Name} : {injection.TargetType}" );
				var dependency = repository.Retrieve( injection.TargetType );

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

		public T Resolve<T>() where T : new() {
			Log( $"Resolve<{nameof(T)}>" );
			
			object targetBoxed = new T();

			InjectIn( targetBoxed );

			return (T)targetBoxed;
		}

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
	internal sealed class DependencyRepository // : IDependencyStorage, IDependencyProvider
	{
		private readonly Dictionary<Type, object> items = new();
		
		public void Put(Type key, object item) {
			if (items.ContainsKey( key )) {
				Debug.LogError( $"Two or more references of the same type <b>{key}</b> detected!" );
			}

			items.Add( key, item );
		}

		public object Retrieve(Type key) {
			return items.TryGetValue( key, out var item ) ? item : null;
		}
	}

	/// <summary>
	/// Provides an information about Type injection points
	/// </summary>
	public static class TypeDescriber // : ITypeDescriber
	{
		delegate R FeatureDescriber<out R>(Type type);

		private static readonly List<FeatureDescriber<ICollection<IInjectionPoint>>> describers = new();
		private static readonly Dictionary<Type, IEnumerable<IInjectionPoint>> injectionPointsCache = new();

		public static void Example(Type targetType) {
			ICollection<IInjectionPoint> FieldDescriber(Type type) {
				Debug.Log( $">> {nameof(FieldDescriber)} exec" );
				return new List<IInjectionPoint>() { new FieldInjectionPoint() };
			}

			ICollection<IInjectionPoint> SetterDescriber(Type type) {
				Debug.Log( $">> {nameof(SetterDescriber)} exec" );
				return new List<IInjectionPoint>();
			}

			describers.Add( CollectInjectFields );
			describers.Add( CollectInjectProperties );

			describers.ForEach( d => Debug.Log( $">> describe: {d( targetType ).Count}" ) );
		}

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

		private static List<IInjectionPoint> CollectInjectFields(Type targetType) {
			var result = new List<IInjectionPoint>();
			var fields = targetType.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			foreach (var field in fields) {
				var attribute = (InjectAttribute)Attribute.GetCustomAttribute( field, typeof(InjectAttribute), true );
				if (attribute != null)
					result.Add( new FieldInjectionPoint( field, true ) );
			}

			return result;
		}

		private static List<IInjectionPoint> CollectInjectProperties(Type targetType) {
			var result = new List<IInjectionPoint>();
			var properties = targetType.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			foreach (var property in properties) {
				if (property.CanWrite) {
					var attribute = (InjectAttribute)Attribute.GetCustomAttribute( property, typeof(InjectAttribute), true );
					if (attribute != null)
						result.Add( new PropertyInjectionPoint( property, !attribute.isOptional ) );
				}
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

		private readonly FieldInfo field;
	}


	// Property
	internal readonly struct PropertyInjectionPoint : IInjectionPoint {
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

		private readonly PropertyInfo property;
	}
}
