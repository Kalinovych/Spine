using Spine;
using UnityEngine;

readonly struct Bootstrap {
	[RuntimeInitializeOnLoadMethod]
	static void Init() {
		var go = new GameObject( "[AppContext]" );
		go.AddComponent<AppContextBehaviour>();
		Object.DontDestroyOnLoad( go );
	}
}

public static class AppContext {
	public static Context current;
}

public class AppContextBehaviour : MonoBehaviour {
	void Awake() {
		AppContext.current = new Context()
			.Configure<ContextConfig>()
			.Initialize();
	}

	void Start() {
		AppContext.current.Emit( new LaunchEvent( "Hello Spine!" ) );
	}
}
