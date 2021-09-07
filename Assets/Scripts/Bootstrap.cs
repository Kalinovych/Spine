using UnityEngine;

readonly struct Bootstrap {
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Init() {
		Debug.Log( "Bootstrap.Init" );

		AppContext.GetContext();
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
