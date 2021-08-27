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
			print( $"→ get: {context.injector.Get<ICalculationService>()}" );

			var service = new StringCalculationService();
			print( $"test result: {service.Calc("2+2")}" );
		}
	}

	//***//
	
	public static class ContextModelExtension {
		public static Context ConfigureModel(this Context context, Action<IModelConfigurator> configure) {
			configure( new ModelConfigurator( context.injector ) );
			return context;
		}
	}

	public interface IModelConfigurator {
		void Add<TDependency, TImplementation>() where TImplementation : TDependency, new();
		void Add<TDependency>() where TDependency : new();
	}

	readonly struct ModelConfigurator : IModelConfigurator {
		readonly Injector injector;

		public ModelConfigurator(Injector injector) {
			this.injector = injector;
		}

		public void Add<TDependency, TImplementation>() where TImplementation : TDependency, new() {
			injector.Add<TDependency, TImplementation>();
		}

		public void Add<TDependency>() where TDependency : new() {
			injector.Add<TDependency>();
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
