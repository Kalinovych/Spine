using UnityEngine;

public class AppContextBehaviour : MonoBehaviour {
	void Awake() {
		print( "AppContextBehaviour.Awake" );
		DontDestroyOnLoad(gameObject);

		AppContext.GetContext();
	}
}
