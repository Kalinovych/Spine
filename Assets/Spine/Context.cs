using System;
using System.Collections.Generic;
using Spine.Signals;
using Spine.DI;

namespace Spine {
	public interface IContextConfig {
		public void Configure();
	}

	public class Context {
		readonly Configurator configurator = new Configurator();
		readonly Injector injector = new Injector();
		readonly EventHub eventHub = new EventHub();

		public Context Configure<T>() where T : struct, IContextConfig {
			configurator.Add( () => injector.Inject<T>().Configure() );
			return this;
		}

		public Context Initialize() {
			injector.AutoMap( injector, eventHub );
			configurator.Execute();
			return this;
		}

		public void Emit<T>(T eventSignal) => eventHub.Emit( eventSignal );
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
