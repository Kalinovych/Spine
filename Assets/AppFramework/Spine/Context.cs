using Spine.DI;
using UnityEngine;

namespace Spine {
	/***
	 * Hardcoded context so far
	 */
	public class Context {
		public readonly Injector injector = new Injector();

		static void Log(object msg) => Debug.Log( $"[Context] {msg}" );

		public Context() {
			injector.MapSingleton( this );
			injector.MapSingleton( injector );
		}
	}

	public interface IContextConfig {
		public void Configure();
	}

	public static class ContextConfigurator {
		public static Context With<TConfig>(this Context context) where TConfig : struct, IContextConfig {
			context.Resolve<TConfig>().Configure();
			return context;
		}
	}
}
