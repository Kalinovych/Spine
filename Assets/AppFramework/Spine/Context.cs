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
			Log( "Context" );
			injector.Add( this );
			injector.Add( injector );
		}
	}

	public interface IContextConfig {
		public void Configure(Context context);
	}
}
