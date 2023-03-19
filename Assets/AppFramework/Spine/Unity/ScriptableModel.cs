using Spine.DI;
using Spine.Signals;
using UnityEngine;

namespace Spine.Integration {
	public abstract class ScriptableModel : ScriptableObject {
		[Inject]
		protected readonly EventHub eventHub;

		protected void Emit<T>(T signal) => eventHub.Send( signal );
	}
}
