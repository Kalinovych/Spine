using Spine;
using UnityEngine;
using UnityEngine.SceneManagement;

readonly struct StartupCmd : ICommand<LaunchEvent> {
	public void Execute(LaunchEvent launch) {
		Debug.Log( $"Startup: {launch.msg}" );

		SceneManager.LoadSceneAsync( "Scenes/Gallery", LoadSceneMode.Additive );
	}
}
