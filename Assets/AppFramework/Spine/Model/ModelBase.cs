using Spine.DI;
using Spine.Signals;

namespace Spine {
	public class ModelBase {
		[Inject]
		protected readonly EventHub eventHub;

		protected void Emit<T>(T signal) => eventHub.Send( signal );
	}
}
