using Spine;
using UnityEngine;

#pragma warning disable 649

public static class AppContext {
	public static Context current => GetContext();

	static Context context;

	static void Log(object msg) => Debug.Log( $"[AppContext] {msg}" );

	static Context GetContext() {
		return context ??= new Context()
			.Configure<ContextConfig>()
			.Initialize();
	}

	public static void Mediate(object view) {
		Log( $"Mediate: {view}" );
		current.injector.InjectInto( view );
	}
	
	public static void Release(object view) {
		Log( $"Mediate: {view}" );
		current.injector.Clear( view );
	}
}
