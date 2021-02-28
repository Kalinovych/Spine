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
		injector.Map<ILogger>( new UnityWarnLogger() );

		On<LaunchEvent>().Do<StartupCmd>();
		On<OpenSceneRequest>().Do<LoadSceneCmd>();
	}

	SignalMapper<T> On<T>() => new SignalMapper<T>( injector, eventHub );
}

interface ILogger {
	public void Log(object msg);
}

class UnityWarnLogger : ILogger {
	public void Log(object msg) {
		Debug.LogWarning( msg );
	}
}
