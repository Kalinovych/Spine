using Spine.Signals;
using Spine.DI;
using UnityEngine;

readonly struct StartupCmd : ICommand {
	[Inject]
	readonly LaunchEvent @event;

	[Inject]
	readonly ILogger logger;

	public void Execute() {
		logger.Log( $"Startup: {@event.msg}" );

		//eventHub.Emit( default(OpenSceneRequest) );
	}
}
