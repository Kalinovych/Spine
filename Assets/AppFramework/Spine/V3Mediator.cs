using System;
using Spine.DI;
using Spine.Signals;
using UnityEngine;

namespace Spine.Experiments {
	public class V3Mediator : MonoBehaviour {
		void Start() {
			print( "V3Mediator.Start" );

			//new V3Program().Run();+

			var context = new Context()
				.InstallEventHub()
				.InstallCommandHub()
				.ConfigureModel( configurator => {
					configurator.Add<ICalculationService, CalculationService>();
					configurator.Add<StringCalculationService>();
				} )
				.ConfigureCommands( hub => {
					hub.Map<CalculateRequest, CalculateCommand>();
				} )
				.Send( new CalculateRequest( "2+2" ) )
				;
		}
	}

	//*** Framework ***//
	
	public static class ContextModelExtension {
		public static Context ConfigureModel(this Context context, Action<IModelConfigurator> configure) {
			configure( new ModelConfigurator( context.injector ) );
			return context;
		}
	}
	
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
	
	//---
	
	readonly struct CalculateRequest {
		public readonly string input;

		public CalculateRequest(string input) {
			this.input = input;
		}
	}

	struct CalculateCommand : ICommand<CalculateRequest> {
		[Inject] StringCalculationService service;
		public void Execute(CalculateRequest request) {
			Debug.Log( $"Calculate: {request.input}, result: {service.Calc( request.input )}" );
		}
	}

	class StringCalculationService {
		public float Calc(string input) {
			Debug.Log( $"Calc: {input}" );
			return 4;
		}
	}

	//---

	readonly struct CalculationRequest {
		public readonly float value;

		public CalculationRequest(float value) {
			this.value = value;
		}
	}

	struct CalculationCommand : ICommand<CalculationRequest> {
		[Inject] ICalculationService service;

		public void Execute(CalculationRequest request) {
			Debug.Log( $"CalculateCommand.Execute: {request.value} -> {service.Calculate( request )}" );
		}
	}


	interface ICalculationService {
		float Calculate(CalculationRequest request);
	}

	struct CalculationService : ICalculationService {
		const float rate = 2f;

		public float Calculate(CalculationRequest request) {
			return request.value * rate;
		}
	}
}
