using Spine.DI;

namespace Spine {
	public class MediatorHub {
		[Inject] Context context;

		public MediatorHub() {
			
		}
	}

	static class MediatorHubInstaller {
		public static Context InstallMediatorHub(this Context context) {
			context.injector.MapSingleton<MediatorHub>();
			return context;
		}
	} 

}
