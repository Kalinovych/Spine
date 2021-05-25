using System;
using System.Collections.Generic;
using Spine.DI;
using Spine.Signals;
using UnityEngine;

namespace Spine {
	public interface IContextConfig {
		public void Configure();
	}

	/***
	 * Hardcoded context so far
	 */
	public class Context {
		readonly Configurator configurator = new Configurator();
		public readonly Injector injector = new Injector();
		readonly EventHub eventHub = new EventHub();
		readonly MediatorHub mediatorHub = new MediatorHub();

		static void Log(object msg) => Debug.Log( $"[Context] {msg}" );

		public Context Configure<T>() where T : struct, IContextConfig {
			Log( $"Configure<{typeof(T)}>" );
			configurator.Add( () => injector.Inject<T>().Configure() );
			return this;
		}

		public Context Initialize() {
			Log( "Initialize" );
			injector.AutoMap( injector, eventHub, mediatorHub );
			configurator.Execute();
			return this;
		}
	}

	class Configurator {
		readonly Queue<Action> queue = new Queue<Action>();

		public void Add(Action configAction)
			=> queue.Enqueue( configAction );

		public void Execute() {
			while (queue.Count > 0)
				queue.Dequeue()();
		}
	}
}
