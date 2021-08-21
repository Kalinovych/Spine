using Spine;
using Spine.Signals;
using Spine.DI;
using UnityEngine.SceneManagement;

#pragma warning disable 169

readonly struct OpenSceneRequest {
}

readonly struct LoadSceneCmd : ICommand {
	[Inject]
	readonly OpenSceneRequest request;

	public void Execute() {
		SceneManager.LoadScene( "Main" );
	}
}
