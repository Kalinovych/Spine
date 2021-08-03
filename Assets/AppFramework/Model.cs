using Spine.DI;
using Spine.Signals;

namespace Spine {
	public abstract class Model {
		[Inject]
		protected EventHub eventHub;

		protected void Emit<T>(T eventSignal) => eventHub.Emit( eventSignal );
	}
}
