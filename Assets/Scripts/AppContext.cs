using Spine;
using Spine.Signals;
using UnityEngine;

public static class AppContext {
	public static Context Current => GetContext();

	static Context context;

	static void Log(object msg) => Debug.Log( $"[AppContext] {msg}" );

	static Context GetContext() {
		return context ??= new Context()
			.Install( new AppBundle() )
			.Configure<ContextConfig>()
			.Initialize();
	}

	public static void Inject(object target) {
		Log( $"Inject: {target}" );
		Current.injector.InjectIn( target );
	}
	
	public static void Release(object target) {
		Log( $"Release: {target}" );
		Current.injector.Clear( target );
	}

	struct AppBundle : IContextExtension {
		public void Extend(Context context) {
			var injector = context.injector;
			injector.MapSingleton<EventHub>();
			injector.MapSingleton<MediatorHub>();
			injector.MapSingleton<CommandHub>();
		}
	}
}
