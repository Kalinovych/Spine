using Spine;
using UnityEngine;

public static class AppContext {
	static Context context;

	[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
	static void Initialize() {
		Debug.Log( "[AppContext] Initialize" );
		context ??= new Context()
				.WithEventHub()
				.WithCommandHub()
				.WithControllerHub()
				.Configure<AppConfig>()
				.Send<LaunchEvent>(default)
			;
	}
}
