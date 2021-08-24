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
		// GO's component find & inject
		injector.Map<Button>( component => {
			if (component is MonoBehaviour c)
				return c.GetComponent<Button>();
			return null;
		} );
		
		ConfigureGallery();
	}

	void ConfigureGallery() {
		injector.Map<GalleryView>( component => {
			if (component is MonoBehaviour c)
				return c.GetComponent<GalleryView>();
			return null;
		} );

		Map<LaunchEvent, StartupCmd>();
		Map<OpenDemoGallery, LoadDemoGalleryCommand>();
		Map<OpenGallery, OpenGalleryCommand>();
		Map<MenuItemSelect, SelectMenuItemCmd>();
		Map<ClearGallery, ClearGalleryCommand>();

		injector.MapSingleton<GalleryModel>();
		injector.MapSingleton<MenuModel>();

		// find a better place to send it
		//eventHub.Send( new LaunchEvent() );
	}

	public void ConfigureOld() {
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
	void Map<TRequest, TCommand>() where TCommand: struct, ICommand => commandHub.On<TRequest>().Do<TCommand>();
}

#region Helpers

#endregion

