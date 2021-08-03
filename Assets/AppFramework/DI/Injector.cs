using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

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

	/***
	 * Todo: implement injections via ExpressionTrees
	 */

	/// <summary>
	/// Injector simple implementation
	/// </summary>
	public class Injector {
		readonly DependencyRepository repository = new DependencyRepository();

		public delegate object DependencyProvider(object target);
		
		readonly Dictionary<Type, DependencyProvider> mappings = new Dictionary<Type, DependencyProvider>();
		
		/// <summary>
		/// Maps dependency by it's Type
		/// </summary>
		/// <param name="instances">A list of instances of unique types</param>
		public void AutoMap(params object[] instances) {
			foreach (var item in instances) {
				repository.Put( item.GetType(), item );
			}
		}
		
		public void One<T>() where T : new() {
			Map<T>( target => {
				var result = repository.Retrieve( typeof(T) );
				if (result == null) {
					result = new T();
					InjectIn( result );
					repository.Put( typeof(T), result );
				}
				return result;
			});
		}

		public void One<T>(T instance) {
			mappings.Add( typeof(T), target => instance );
		}

		public void Map<T>(DependencyProvider provider) {
			mappings.Add( typeof(T), provider );
		}

		public void Unmap<T>() => mappings.Remove( typeof(T) );

		public T InjectIn<T>() where T : struct {
			object targetBoxed = default(T);

			InjectIn( targetBoxed );

			return (T) targetBoxed;
		}

		public Injection<T> With<T>(T dependency) {
			return new Injection<T>( this, dependency );
		}

		public readonly struct Injection<TValue> {
			readonly Injector injector;
			readonly TValue value;

			public Injection(Injector injector, TValue value) {
				this.injector = injector;
				this.value = value;
			}

			public T Create<T>() where T : struct {
				object target = default(T);

				var injectionPoints = TypeDescriber.GetInjectionPoints( target.GetType() );

				foreach (var injection in injectionPoints) {
					var dependency = injector.repository.Retrieve( injection.TargetType );

					// override
					if (injection.TargetType == typeof(TValue)) {
						dependency = value;
					}

					if (dependency == null && injector.mappings.TryGetValue( injection.TargetType, out var provider )) {
						dependency = provider(target);
					}

					if (dependency == null && injection.isRequired) {
						Debug.LogError( $">> \t{injection.Name} = <i>[missing required reference]</i>({injection.TargetType.Name})" );
						continue;
					}

					injection.ApplyTo( target, dependency );
				}

				return (T) target;
			}
		}

		public void InjectIn<T>(ref T target) where T : struct {
			object targetBoxed = target;

			InjectIn( targetBoxed );

			target = (T) targetBoxed;
		}

		/// <summary>
		/// Fulfill target's injection requirements
		/// </summary>
		/// <param name="target"></param>
		public void InjectIn(object target) {
			Debug.Log( $"InjectIn: {target}" );
			
			var injectionPoints = TypeDescriber.GetInjectionPoints( target.GetType() );

			foreach (var injection in injectionPoints) {
				Debug.Log( $"\tpoint: {injection.Name} : {injection.TargetType}" );
				var dependency = repository.Retrieve( injection.TargetType );
				
				if (dependency == null && mappings.TryGetValue( injection.TargetType, out var provider )) {
					dependency = provider(target);
				}

				if (dependency == null && injection.isRequired) {
					Debug.LogError( $">> \t{injection.Name} = <i>[missing required reference]</i>({injection.TargetType.Name})" );
					continue;
				}

				Debug.Log( $"\tinject: {target} ← {dependency}" );
				injection.ApplyTo( target, dependency );
			}
		}

		public void Clear(object target) {
			var injectionPoints = TypeDescriber.GetInjectionPoints( target.GetType() );
			foreach (var injection in injectionPoints) {
				injection.ApplyTo( target, null );
			}
		}

		public void InjectIn<T>(T target) {
			var injectionPoints = TypeDescriber.GetInjectionPoints( typeof(T) );

			foreach (var injection in injectionPoints) {
				var dependency = repository.Retrieve( injection.TargetType );

				if (dependency is null && mappings.TryGetValue( injection.TargetType, out var provider )) {
					dependency = provider(target);
				}

				if (dependency == null && injection.isRequired) {
					Debug.LogError( $">> \t{injection.Name} = <i>[missing required reference]</i>({injection.TargetType.Name})" );
					continue;
				}

				injection.ApplyTo( target, dependency );
			}
		}
	}

	/// <summary>
	/// Dependency Storage and Provider
	/// </summary>
	class DependencyRepository // : IDependencyStorage, IDependencyProvider
	{
		public void Put(Type key, object item) {
			if (items.ContainsKey( key )) {
				Debug.LogError( $"Two or more references of the same type <b>{key}</b> detected!" );
			}

			items.Add( key, item );
		}

		public object Retrieve(Type key) {
			return items.TryGetValue( key, out var item ) ? item : null;
		}

		readonly Dictionary<Type, object> items = new Dictionary<Type, object>();
	}

	/// <summary>
	/// Provides an information about Type injection points
	/// </summary>
	public static class TypeDescriber // : ITypeDescriber
	{
		delegate R FeatureDescriber<out R>(Type type);

		static List<FeatureDescriber<ICollection<IInjectionPoint>>> describers = new List<FeatureDescriber<ICollection<IInjectionPoint>>>();

		public static void Example(Type targetType) {
			ICollection<IInjectionPoint> FieldDescriber(Type type) {
				Debug.Log( $">> {nameof(FieldDescriber)} exec" );
				return new List<IInjectionPoint>() {new FieldInjectionPoint()};
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
			//var diPoints = CollectInjectFields(targetType);

			// Inject into setters
			//diPoints.AddRange(CollectInjectProperties(targetType));

			// cache it
			injectionPointsCache[targetType] = diPoints;

			return diPoints;
		}

		static List<IInjectionPoint> CollectInjectionPoints(Type targetType) {
			var result = new List<IInjectionPoint>();
			var members = targetType.GetMembers( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			foreach (var member in members) {
				var attribute = (InjectAttribute) Attribute.GetCustomAttribute( member, typeof(InjectAttribute), true );
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

		static List<IInjectionPoint> CollectInjectFields(Type targetType) {
			var result = new List<IInjectionPoint>();
			var fields = targetType.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			foreach (var field in fields) {
				var attribute = (InjectAttribute) Attribute.GetCustomAttribute( field, typeof(InjectAttribute), true );
				if (attribute != null)
					result.Add( new FieldInjectionPoint( field, true ) );
			}

			return result;
		}

		static List<IInjectionPoint> CollectInjectProperties(Type targetType) {
			var result = new List<IInjectionPoint>();
			var properties = targetType.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			foreach (var property in properties) {
				if (property.CanWrite) {
					var attribute = (InjectAttribute) Attribute.GetCustomAttribute( property, typeof(InjectAttribute), true );
					if (attribute != null)
						result.Add( new PropertyInjectionPoint( property, !attribute.isOptional ) );
				}
			}

			return result;
		}

		static readonly Dictionary<Type, IEnumerable<IInjectionPoint>> injectionPointsCache = new Dictionary<Type, IEnumerable<IInjectionPoint>>();
	}

	// DIPoint
	public interface IInjectionPoint {
		bool isRequired { get; }

		string Name { get; }

		Type TargetType { get; }

		void ApplyTo(object target, object value);
	}

	// Field
	readonly struct FieldInjectionPoint : IInjectionPoint {
		public bool isRequired { get; }

		public Type TargetType => field.FieldType;

		public string Name => field.Name;

		public FieldInjectionPoint(FieldInfo field, bool isRequired) {
			this.field = field;
			this.isRequired = isRequired;
		}

		public void ApplyTo(object target, object value) {
			field.SetValue( target, value );
		}

		private readonly FieldInfo field;
	}


	// Property
	readonly struct PropertyInjectionPoint : IInjectionPoint {
		public bool isRequired { get; }

		public Type TargetType => property.PropertyType;

		public string Name => property.Name;

		public PropertyInjectionPoint(PropertyInfo property, bool isRequired) {
			this.property = property;
			this.isRequired = isRequired;
		}

		public void ApplyTo(object target, object value) {
			property.SetValue( target, value );
		}

		private readonly PropertyInfo property;
	}
}
