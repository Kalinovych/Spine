using App;
using App.Commands;
using Spine.Signals;
using Spine;
using Spine.DI;
using UnityEngine;

readonly struct ContextConfig : IContextConfig {
	[Inject]
	readonly EventHub eventHub;

	[Inject]
	readonly Injector injector;

	public void Configure() {
		injector.Map<MenuModel>( new MenuModel() );
		injector.Map<ILogger>( new DefaultLogger() );
		injector.Map<LogAction>( DefaultLogger.LogWarnStatic );

		On<LaunchEvent>().Do<StartupCmd>();
		On<OpenSceneRequest>().Do<LoadSceneCmd>();
		On<MenuItemSelect>().Do<SelectMenuItemCmd>();
	}

	SignalMapper<T> On<T>() => new SignalMapper<T>( injector, eventHub );
}

#region Helpers
public delegate void LogAction(object msg);

interface ILogger {
	public void Log(object msg);
}

class DefaultLogger : ILogger {
	public void Log(object msg) {
		Debug.LogWarning( msg );
	}

	public static void LogWarnStatic(object msg) {
		Debug.LogWarning( msg );
	}
}

#endregion

