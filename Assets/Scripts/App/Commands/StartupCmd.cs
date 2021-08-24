using Spine;
using Spine.DI;
using UnityEngine;

readonly struct StartupCmd : ICommand {
	[Inject]
	readonly LaunchEvent @event;

	public void Execute() {
		Debug.Log( $"Startup: {@event.msg}" );
	}
}
