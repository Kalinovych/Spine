using App;
using App.Commands;
using App.Requests;
using App.Utils;
using Spine.Signals;
using Spine;
using Spine.DI;
using UnityEngine;
using UnityEngine.UI;

readonly struct ContextConfig : IContextConfig {
	[Inject]
	readonly EventHub eventHub;

	[Inject]
	readonly Injector injector;

	[Inject]
	readonly MediatorHub mediatorHub;

	[Inject]
	readonly CommandHub commandHub;

	public void Configure() {
		injector.Map<GalleryView>( component => {
			if (component is MonoBehaviour c)
				return c.GetComponent<GalleryView>();
			return null;
		} );
		
		commandHub.Map<LaunchEvent>().To<LoadDemoGallery>();

		injector.MapSingleton<GalleryModel>();

		// find a better place to send it
		eventHub.Send( new LaunchEvent() );
	}

	public void ConfigureOld() {
		// GO's component find & inject
		injector.Map<Button>( component => {
			if (component is MonoBehaviour c)
				return c.GetComponent<Button>( );
			return null;
		});

		injector.MapSingleton<MenuModel>();
		injector.MapSingleton<LogAction>( Debug.Log );

		On<LaunchEvent>().Do<StartupCmd>();
		On<MenuItemSelect>().Do<SelectMenuItemCmd>();
		//On<OpenSceneRequest>().Do<LoadSceneCmd>();
		//On<LaunchEvent>().Do<LoadAdditional>();
		
		Map<OpenScreenRequest>().To<OpenScreenCommand>();
		Map<CloseScreenRequest>().To<CloseScreenCommand>();
		
		eventHub.Send( new LaunchEvent() );
	}

	EventMapper<T> On<T>() => commandHub.On<T>();
	EventMapper<T> Map<T>() => commandHub.On<T>();
}

#region Helpers

#endregion

