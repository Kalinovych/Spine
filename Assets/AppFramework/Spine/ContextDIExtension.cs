using Spine.Signals;

namespace Spine {
	public static class ContextDIExtension {
		public static T Resolve<T>(this Context context) where T : new() {
			return context.injector.Resolve<T>();
		}

		public static Context InstallEventHub(this Context context) {
			context.injector.MapSingleton<EventHub>();
			return context;
		}
	}
}
