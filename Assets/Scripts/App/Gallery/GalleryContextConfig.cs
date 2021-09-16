using App.Commands;
using App.Models;
using Spine;
using Spine.Experiments;
using Spine.Integration;
using UnityEngine;
using UnityEngine.UI;

namespace App {
	[CreateAssetMenu]
	public class GalleryContextConfig : ScriptableContext, IContextConfig {
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
}
