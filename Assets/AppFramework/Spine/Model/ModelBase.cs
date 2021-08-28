using Spine.DI;
using Spine.Signals;

namespace Spine {
	public class ModelBase {
		[Inject]
		protected readonly EventHub eventHub;

		protected void Send<T>(T signal) => eventHub.Send( signal );
	}
	
	
}
