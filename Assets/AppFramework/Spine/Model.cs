using Spine.DI;
using Spine.Signals;

namespace Spine {
	public class Model {
		[Inject]
		protected readonly EventHub eventHub;

		protected void Emit<T>(T eventSignal) => eventHub.Send( eventSignal );
	}
}
