using App;
using App.Commands;
using App.Models;
using Spine;
using Spine.Experimental;
using Spine.Integration;
using UnityEngine.UI;

readonly struct AppConfig : IContextConfig {
	public void Configure(Context context) {
		context
			.ConfigureModel( ConfigureModel )
			.ConfigureView( ConfigureView )
			.ConfigureCommands( ConfigureCommands )
			//.AutoConfigureCommands()
			;
	}

	static void ConfigureModel(IModelConfigurator models) {
		models
			.Add<GalleryModel>()
			.Add<MenuModel>()
			;
	}

	static void ConfigureCommands(CommandHub commandHub) {
		commandHub
			.Map<LaunchEvent, StartupCmd>()
			.Map<OpenDemoGallery, LoadDemoGalleryCommand>()
			.Map<OpenGallery, OpenGalleryCommand>()
			.Map<ClearGallery, ClearGalleryCommand>()
			.Map<MenuItemSelect, SelectMenuItemCmd>()
			;
	}

	static void ConfigureView(ComponentConfigurator configurator) {
		configurator
			.AddView<GalleryView>()
			.AddView<Button>();
	}
}
