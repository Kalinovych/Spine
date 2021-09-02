using System;

namespace Spine.Experiments {
	public static class ModelConfiguratorExtension {
		public static Context ConfigureModel(this Context context, Action<IModelConfigurator> configure) {
			configure( new ModelConfigurator( context.injector ) );
			return context;
		}
	}
}
