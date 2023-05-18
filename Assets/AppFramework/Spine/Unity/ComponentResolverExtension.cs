using System;
using UnityEngine;

namespace Spine.Integration {
	public static class ComponentResolverExtension {
		public static Context ConfigureView(this Context context, Action<ComponentConfigurator> configure) {
			configure( new ComponentConfigurator( context ) );
			return context;
		}
	}

	public readonly struct ComponentConfigurator {
		readonly Context context;

		public ComponentConfigurator(Context context) {
			this.context = context;
		}

		public ComponentConfigurator AddView<T>() where T : MonoBehaviour {
			context.injector.Map<T>( component => {
				if (component is MonoBehaviour c)
					return c.GetComponent<T>();
				return null;
			} );
			return this;
		}
	}
}
