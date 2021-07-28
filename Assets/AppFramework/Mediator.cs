using System;
using Spine.DI;
using Spine.Signals;
using UnityEngine;

public abstract class Mediator : MonoBehaviour {
	[Inject]
	protected readonly EventHub eventHub;

	protected void Emit<T>(T eventSignal) => eventHub.Emit( eventSignal );

	protected void OnEvent<T>(Action<T> callback) => eventHub.On( callback );
	protected void OffEvent<T>(Action<T> callback) => eventHub.Off( callback );

	protected virtual void Awake() {
		print( "Mediator.Awake" );
		
		AppContext.Mediate( this );

		OnInitialized();
	}

	protected virtual void OnInitialized() {}
	protected virtual void OnPreDestroy() {}

	protected virtual void OnDestroy() {
		OnPreDestroy();
		AppContext.Release( this );
	}
}

