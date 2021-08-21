using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Spine.DI;
using Spine.Signals;
using UnityEngine;

namespace Spine {
	/***
	 * Hardcoded context so far
	 */
	public class Context {
		public readonly Injector injector = new Injector();
		readonly Configurator installer = new Configurator();
		//readonly Configurator configurator = new Configurator();

		static void Log(object msg) => Debug.Log( $"[Context] {msg}" );

		internal readonly Lifecycle lifecycle;

		public Context() {
			lifecycle = new Lifecycle( this );
		}

		public Context Install(IContextExtension ext) {
			installer.Add( () => ext.Extend( this ) );
			return this;
		}

		/*public Context Configure<TContextConfig>() where TContextConfig : struct, IContextConfig {
			Log( $"Configure<{typeof(TContextConfig)}>" );
			configurator.Add( () => injector.Resolve<TContextConfig>().Configure() );
			return this;
		}*/

		public Context Initialize() {
			Log( "Initialize" );
			injector.MapSingleton( this );
			injector.MapSingleton( injector );
			installer.Execute();
			//configurator.Execute();
			
			lifecycle.Initialize();
			return this;
		}

		public event Action OnInitialize;

		public class Lifecycle {
			readonly Context context;

			public Lifecycle(Context context) {
				this.context = context;
			}

			public void Initialize() {
				context.OnInitialize?.Invoke();
			}
		}
	}

	public interface IContextConfig {
		public void Configure();
	}

	/***
	 * context.Configure<TConfig>();
	 */

	public static class ContextConfigurator {
		public static Context With<TConfig>(this Context context) where TConfig : struct, IContextConfig {
			context.OnInitialize += () => context.Resolve<TConfig>().Configure();
			return context;
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
}
