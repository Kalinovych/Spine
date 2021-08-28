using Spine;
using UnityEngine;

readonly struct StartupCmd : ICommand<LaunchEvent> {
	public void Execute(LaunchEvent launch) {
		Debug.Log( $"Startup: {launch.msg}" );
	}
}
