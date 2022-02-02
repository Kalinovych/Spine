using UnityEngine;

readonly struct Bootstrap {
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Init() {
		Debug.Log( "[Bootstrap] Init..." );

		//AppContext.Initialize();
	}
}
