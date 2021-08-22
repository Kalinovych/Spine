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


		public static Context Send<T>(this Context context, T signal) {
			context.injector.Retrieve<EventHub>().Send( signal );

			/***
			 * 
			 */

			return context;
		}
	}
}
