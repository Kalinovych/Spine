using Spine.Signals;
using Spine.DI;
using UnityEngine;

#pragma warning disable 649

readonly struct StartupCmd : ICommand {
	[Inject]
	readonly LaunchEvent @event;

	[Inject]
	readonly ILogger logger;

	[Inject]
	readonly EventHub eventHub;

	public void Execute() {
		Debug.Log( $"logger: {logger}, eventHub: {eventHub}" );
		logger.Log( $"Startup: {@event.msg}" );

		eventHub.Emit( default(OpenSceneRequest) );
	}
}
