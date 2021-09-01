using UnityEngine;

readonly struct Bootstrap {
	[RuntimeInitializeOnLoadMethod]
	static void Init() {
		Debug.Log( "Bootstrap.Init" );
		//var go = new GameObject( "[AppContext]" );
		//go.AddComponent<AppContextBehaviour>();
		//Object.DontDestroyOnLoad( go );
	}
}

class BootstrapOnLoad {
	static BootstrapOnLoad() {
		Debug.Log( "BootstrapOnLoad" );

		var go = new GameObject( "[AppContext]" );
		go.AddComponent<AppContextBehaviour>();
		Object.DontDestroyOnLoad( go );
	}
}

public class AppContextBehaviour : MonoBehaviour {
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	void Start() {
		
	}
}
