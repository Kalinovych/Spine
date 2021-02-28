using Spine;
using Spine.Signals;
using UnityEngine;

public class Mediator : MonoBehaviour {
	Context context => AppContext.current;

	protected void Emit<T>(T eventSignal) => context.Emit( eventSignal );

}
