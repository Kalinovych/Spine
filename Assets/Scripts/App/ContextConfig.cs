using App;
using App.Commands;
using App.Requests;
using App.Utils;
using Spine.Signals;
using Spine;
using Spine.DI;
using UnityEngine;
using UnityEngine.UI;
using App.Models;
using Spine.Experiments;
using UnityEditor;

readonly struct ContextConfig : IContextConfig {
	[Inject]
	readonly EventHub eventHub;

	[Inject]
	readonly Injector injector;

	[Inject]
	readonly MediatorHub mediatorHub;

	public void Configure(Context context) {
		// GO's component find & inject
		context.injector.Map<Button>( component => {
			if (component is MonoBehaviour c)
				return c.GetComponent<Button>();
			return null;
		} );

		injector.Map<GalleryView>( component => {
			if (component is MonoBehaviour c)
				return c.GetComponent<GalleryView>();
			return null;
		} );

		
		context
			.ConfigureCommands( ConfigureCommands )
			.ConfigureModel( ConfigureModel );
	}

	public static void ConfigureModel(IModelConfigurator models) {
		models
			.AddScriptableModel<GalleryModel>()
			//.Add<GalleryModel>()
			.Add<MenuModel>()
			;
	}

	public static void ConfigureCommands(CommandHub commandHub) {
		commandHub
			.Map<LaunchEvent, StartupCmd>()
			.Map<OpenDemoGallery, LoadDemoGalleryCommand>()
			.Map<OpenGallery, OpenGalleryCommand>()
			.Map<ClearGallery, ClearGalleryCommand>()
			.Map<MenuItemSelect, SelectMenuItemCmd>()
			;
	}

}

#region Helpers

#endregion

