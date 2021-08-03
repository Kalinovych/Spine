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
		public readonly Injector injector = new Injector();
		readonly Configurator installer = new Configurator();
		readonly Configurator configurator = new Configurator();

		static void Log(object msg) => Debug.Log( $"[Context] {msg}" );

		public Context Install(IContextExtension ext) {
			installer.Add( () => ext.Extend( this ) );
			return this;
		}

		public Context Configure<T>() where T : struct, IContextConfig {
			Log( $"Configure<{typeof(T)}>" );
			configurator.Add( () => injector.InjectIn<T>().Configure() );
			return this;
		}

		public Context Initialize() {
			Log( "Initialize" );
			injector.One( this );
			injector.One( injector );
			installer.Execute();
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

	public interface IContextExtension {
		void Extend(Context context);
	}

	public struct ContextExtension {
		public void Extend(Context context) {
			
		}
	}
}
