using System;
using Spine.DI;
using Spine.Experiments;
using Spine.Signals;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace App {
	[CreateAssetMenu]
	public class GalleryModel : ScriptableModel {
		public struct GalleryChanged {
		}

		public Gallery CurrentGallery { get; set; } = new Gallery( Array.Empty<GalleryItem>() );

		public void SetGallery(Gallery gallery) {
			CurrentGallery = gallery;
			Send( new GalleryChanged() );
		}
	}

	public class ScriptableModel : ScriptableObject {
		[Inject]
		protected readonly EventHub eventHub;

		protected void Send<T>(T signal) => eventHub.Send( signal );

		void Awake() {
			Debug.Log( "ScriptableModel.Awake" );
		}
	}

	public static class ModelConfiguratorExt {
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

			configurator.Add( instance );
			return configurator;
		}

		public static IModelConfigurator AddScriptableModel<T>(this IModelConfigurator configurator, T instance) where T : ScriptableObject {
			configurator.Add( instance );
			return configurator;
		}
	}

	public readonly struct Gallery {
		public readonly GalleryItem[] items;

		public Gallery(GalleryItem[] items) {
			this.items = items;
		}
	}

	public struct GalleryItem {
		public Color color;
	}
}
