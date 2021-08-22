using Spine;
using Spine.Signals;
using UnityEngine;

public static class AppContext {
	public static Context Current => GetContext();

	static Context context;

	static void Log(object msg) => Debug.Log( $"[AppContext] {msg}" );

	static Context GetContext() {
		return context ??= new Context()
			.InstallAppBundle()
			.With<ContextConfig>()
			.Initialize();
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

static class AppBundleInstaller {
	public static Context InstallAppBundle(this Context context) {
		context.Install( new AppBundle() );
		return context;
	}
}

readonly struct AppBundle : IContextExtension {
	public void Extend(Context context) {
		context
			.InstallCommandHub()
			.InstallMediatorHub();
		
		var injector = context.injector;
		injector.MapSingleton<EventHub>();
		//injector.MapSingleton<MediatorHub>();
		//injector.MapSingleton<CommandHub>();
	}
}



static class MediatorHubInstaller {
	public static Context InstallMediatorHub(this Context context) {
		context.injector.MapSingleton<MediatorHub>();
		return context;
	}
} 
