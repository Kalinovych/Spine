using System;
using Spine.DI;
using Spine.Signals;
using UnityEngine;

namespace Spine {
	public abstract class ControllerBase : MonoBehaviour {
		[Inject]
		protected readonly EventHub eventHub;

		protected void Send<T>(T eventSignal) {
			eventHub.Send( eventSignal );
		}

		protected void OnEvent<T>(Action<T> callback) {
			eventHub.On( callback );
		}

		protected void OffEvent<T>(Action<T> callback) {
			eventHub.Off( callback );
		}

		protected virtual void OnInitialized() { }
		protected virtual void OnDestroying() { }

		protected virtual void Awake() {
			print( "ControllerBase.Awake" );

			this.Resolve();

			OnInitialized();
		}

		protected virtual void OnDestroy() {
			print( "ControllerBase.OnDestroy" );

			OnDestroying();

			this.Release();
		}
	}
}
