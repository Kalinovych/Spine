using Spine;
using UnityEngine;

public static class AppContext {
	public static Context Current => GetContext();

	static Context context;

	static void Log(object msg) => Debug.Log( $"[AppContext] {msg}" );

	static Context GetContext() {
		return context ??= new Context()
				.InstallEventHub()
				.InstallCommandHub()
				.InstallMediatorHub()
				.With<ContextConfig>()
				.Send( new LaunchEvent() )
				;
	}

	/**
	 * Resolves target object dependencies
	 */
	public static void Resolve(object target) {
		Log( $"Inject: {target}" );
		Current.injector.Resolve( target );
	}

	/**
	 * Clear references to dependencies
	 */
	public static void Release(object target) {
		Log( $"Release: {target}" );
		Current.injector.Clear( target );
	}
}

