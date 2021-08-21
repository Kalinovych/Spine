using Spine;
using Spine.Signals;
using UnityEngine.SceneManagement;

namespace App.Commands {
	public readonly struct OpenScreenCommand : ICommand {
		const string ScreenSceneName = "Additional";
		public void Execute() {
			if (SceneManager.GetSceneByName( ScreenSceneName ).isLoaded is false)
				SceneManager.LoadSceneAsync( ScreenSceneName, LoadSceneMode.Additive );
		}
	}
	
	public readonly struct CloseScreenCommand : ICommand {
		const string ScreenSceneName = "Additional";

		public void Execute() {
			if (SceneManager.GetSceneByName( ScreenSceneName ).isLoaded)
				SceneManager.UnloadSceneAsync( ScreenSceneName );
		}
	}
}
