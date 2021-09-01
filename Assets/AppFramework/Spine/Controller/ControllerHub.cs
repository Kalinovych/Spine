using System.Collections.Generic;

namespace Spine {
	public class ControllerHub {
		internal static ControllerHub instance;

		internal static Context globalContext;

		public ControllerHub(Context context) {
			globalContext = context;
			instance = this;
		}
	}

	static class ControllerHubExtension {
		public static Context InstallControllerHub(this Context context) {
			context.injector.MapSingleton( new ControllerHub( context ) );
			return context;
		}

		public static void Resolve(this ControllerBase controller) {
			ControllerHub.globalContext.injector.InjectIn( controller );
		}
		
		public static void Release(this ControllerBase controller) {
			ControllerHub.globalContext.injector.Clear( controller );
		}
	}
}
