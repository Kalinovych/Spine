using App;
using App.Commands;
using App.Utils;
using Spine.Signals;
using Spine;
using Spine.DI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

readonly struct ContextConfig : IContextConfig {
	[Inject]
	readonly EventHub eventHub;

	[Inject]
	readonly Injector injector;

	public void Configure() {
		injector.Map<Button>( component => {
			if (component is MonoBehaviour c)
				return c.GetComponent<Button>( );
			return null;
		});
		
		injector.Map( new MenuModel() );
		injector.Map<LogAction>( Debug.Log );

		On<LaunchEvent>().Do<StartupCmd>();
		On<OpenSceneRequest>().Do<LoadSceneCmd>();
		On<MenuItemSelect>().Do<SelectMenuItemCmd>();
		On<LaunchEvent>().Do<LoadAdditional>();
		
		eventHub.Emit( new LaunchEvent() );
	}

	SignalMapper<T> On<T>() => new SignalMapper<T>( injector, eventHub );
}

#region Helpers


struct LoadAdditional : ICommand {
	public void Execute() {
		SceneManager.LoadScene( "Additional", LoadSceneMode.Additive );
	}
}


#endregion

