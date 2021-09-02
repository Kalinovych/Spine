using UnityEngine;

readonly struct Bootstrap {
	[RuntimeInitializeOnLoadMethod]
	static void Init() {
		Debug.Log( "Bootstrap.Init" );

		//AppContext.GetContext();

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
