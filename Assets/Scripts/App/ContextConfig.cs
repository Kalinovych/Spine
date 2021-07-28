using App;
using App.Commands;
using Spine.Signals;
using Spine;
using Spine.DI;
using UnityEngine;
using UnityEngine.SceneManagement;

readonly struct ContextConfig : IContextConfig {
	[Inject]
	readonly EventHub eventHub;

	[Inject]
	readonly Injector injector;

	public void Configure() {
		injector.Map<MenuModel>( new MenuModel() );
		injector.Map<ILogger>( new DefaultLogger() );
		injector.Map<LogAction>( DefaultLogger.LogStatic );

		On<LaunchEvent>().Do<StartupCmd>();
		On<OpenSceneRequest>().Do<LoadSceneCmd>();
		On<MenuItemSelect>().Do<SelectMenuItemCmd>();
		On<LaunchEvent>().Do<LoadAdditional>();
		
		eventHub.Emit( new LaunchEvent() );
	}

	SignalMapper<T> On<T>() => new SignalMapper<T>( injector, eventHub );
}

#region Helpers
public delegate void LogAction(object msg);

struct LoadAdditional : ICommand {
	public void Execute() {
		SceneManager.LoadScene( "Additional", LoadSceneMode.Additive );
	}
}

interface ILogger {
	public void Log(object msg);
	public void Warn(object msg);
}

class DefaultLogger : ILogger {
	public void Log(object msg) {
		Debug.LogWarning( msg );
	}

	public void Warn(object msg) {
		Debug.LogWarning( msg );
	}

	public static void LogStatic(object msg) {
		Debug.Log( msg );
	}
	public static void LogWarnStatic(object msg) {
		Debug.LogWarning( msg );
	}
}

#endregion

