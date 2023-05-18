using System;
using Spine.Experimental;
using UnityEditor;
using UnityEngine;

namespace Spine.Integration {
	public static class ScriptableModelExtension {
	#if UNITY_EDITOR
		public static IModelConfigurator AddScriptableModel<T>(this IModelConfigurator configurator) where T : ScriptableObject {
			T instance = null;
			
			var assets = AssetDatabase.FindAssets( $"t:{typeof(T)}" );
			foreach (var s in assets) {
				instance = AssetDatabase.LoadAssetAtPath<T>( AssetDatabase.GUIDToAssetPath( s ) );
				break;
			}

			if (instance is null) {
				throw new NullReferenceException( $"An instance of the scriptable model <{typeof(T)}> not found!" );
			}

			configurator.AddScriptableModel( instance );
			return configurator;
		}

		public static IModelConfigurator AddScriptableModel<T>(this IModelConfigurator configurator, T instance) where T : ScriptableObject {
			configurator.Add( instance );
			return configurator;
		}
	#endif
	}
}
