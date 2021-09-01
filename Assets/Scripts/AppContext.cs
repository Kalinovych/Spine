using Spine;
using UnityEngine;

public static class AppContext {
	public static Context Current => GetContext();

	static Context context;

	static Context GetContext() {
		return context ??= new Context()
				.InstallEventHub()
				.InstallCommandHub()
				.InstallControllerHub()
				.With<ContextConfig>()
				.Send( new LaunchEvent() )
				;
	}
}

