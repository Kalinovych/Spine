using Spine.Signals;
using UnityEngine.SceneManagement;

struct LoadAdditional : ICommand {
	public void Execute() {
		SceneManager.LoadScene( "Additional", LoadSceneMode.Additive );
	}
} 
