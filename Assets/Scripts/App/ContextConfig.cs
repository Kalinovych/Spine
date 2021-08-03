using App;
using App.Commands;
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
		// GO's component find & inject
		injector.Map<Button>( component => {
			if (component is MonoBehaviour c)
				return c.GetComponent<Button>( );
			return null;
		});
		
		injector.One<MenuModel>();
		injector.One<LogAction>( Debug.Log );

		On<LaunchEvent>().Do<StartupCmd>();
		On<OpenSceneRequest>().Do<LoadSceneCmd>();
		On<MenuItemSelect>().Do<SelectMenuItemCmd>();
		On<LaunchEvent>().Do<LoadAdditional>();
		
		eventHub.Emit( new LaunchEvent() );
	}

	EventMapper<T> On<T>() => commandHub.On<T>();
}

#region Helpers

#endregion

