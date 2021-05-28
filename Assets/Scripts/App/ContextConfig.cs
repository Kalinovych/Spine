using System;
using App;
using App.Commands;
using Spine.Signals;
using Spine;
using Spine.DI;
using UnityEngine;

#pragma warning disable 649

readonly struct ContextConfig : IContextConfig {
	[Inject]
	readonly EventHub eventHub;

	[Inject]
	readonly Injector injector;

	public void Configure() {
		injector.Map<MenuModel>( new MenuModel() );
		injector.Map<ILogger>( new UnityWarnLogger() );
		injector.Map<LogAction>( UnityWarnLogger.LogStatic );

		On<LaunchEvent>().Do<StartupCmd>();
		On<OpenSceneRequest>().Do<LoadSceneCmd>();
		On<MenuItemSelect>().Do<SelectMenuItemCmd>();
	}

	SignalMapper<T> On<T>() => new SignalMapper<T>( injector, eventHub );
}

public delegate void LogAction(object msg);

interface ILogger {
	public void Log(object msg);
}

class UnityWarnLogger : ILogger {
	public void Log(object msg) {
		Debug.LogWarning( msg );
	}

	public static void LogStatic(object msg) {
		Debug.LogWarning( msg );
	}
}
