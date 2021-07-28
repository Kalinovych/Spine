﻿using Spine.DI;
using Spine.Signals;
using UnityEngine;

public abstract class Mediator : MonoBehaviour {
	[Inject]
	protected readonly EventHub eventHub;

	protected void Emit<T>(T eventSignal) => eventHub.Emit( eventSignal );

	protected virtual void Awake() {
		print( "Mediator.Awake" );
		AppContext.Mediate( this );
	}

	void Start() {
		print( $"Mediator.OnEnable: {this}" );
	}

	void OnTransformChildrenChanged() {
		print( "Mediator.OnTransformChildrenChanged" );
	}

	void OnTransformParentChanged() {
		print( "Mediator.OnTransformParentChanged" );
	}
}

interface IMediator {
	void OnCreate();
}
