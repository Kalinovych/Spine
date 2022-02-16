using Spine.DI;

namespace Spine.Experimental {
	public interface IModelConfigurator {
		IModelConfigurator Add<TDependency, TImplementation>() where TImplementation : TDependency, new();
		IModelConfigurator Add<TDependency>() where TDependency : new();
		IModelConfigurator Add<TDependency>(TDependency dependency);
	}

	readonly struct ModelConfigurator : IModelConfigurator {
		readonly Injector injector;

		public ModelConfigurator(Injector injector) {
			this.injector = injector;
		}

		public IModelConfigurator Add<TDependency, TImplementation>() where TImplementation : TDependency, new() {
			injector.Add<TDependency, TImplementation>();
			return this;
		}

		public IModelConfigurator Add<TDependency>() where TDependency : new() {
			injector.Add<TDependency>();
			return this;
		}

		public IModelConfigurator Add<TDependency>(TDependency dependency) {
			injector.Add( dependency );
			return this;
		}
	}
}
