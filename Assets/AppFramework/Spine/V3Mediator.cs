﻿using System;
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
				.With<CalculatorAppContextConfig>()
				.ConfigureModel( configurator => {
					configurator.Add<ICalculationService, CalculationService>();
				} )
				.ConfigureCommands( hub => {
					hub.Map<CalculationRequest, CalculateCommand>();
				} )
				.Send( new CalculationRequest( 2 ) );
		}
	}

	public static class ContextModelExtension {
		/*public static Context ConfigureModel(this Context context, Action<Injector> configure) {
			configure( context.injector );
			return context;
		}*/

		public static Context ConfigureModel(this Context context, Action<IModelConfigurator> configure) {
			//configure( context.injector );
			configure( new ModelConfigurator( context.injector ) );
			return context;
		}
	}

	public interface IModelConfigurator {
		void Add<TDependency, TImplementation>() where TImplementation : TDependency, new();
	}

	readonly struct ModelConfigurator : IModelConfigurator {
		readonly Injector injector;

		public ModelConfigurator(Injector injector) {
			this.injector = injector;
		}

		public void Add<TDependency, TImplementation>() where TImplementation : TDependency, new() {
			injector.Add<TDependency, TImplementation>();
		}
	} 
	
	
	struct CalculatorAppContextConfig : IContextConfig {
		[Inject] CommandHub commandHub;
		[Inject] Injector injector;

		public void Configure() {
			//commandHub.Map<CalculationRequest, CalculateCommand>();

			//injector.MapSingleton<ICalculationService>(new CalculationService());
			
			//injector.Add<ICalculationService, CalculationService>();
		}
	}

	readonly struct CalculationRequest {
		public readonly float value;

		public CalculationRequest(float value) {
			this.value = value;
		}
	}

	struct CalculateCommand : ICommand<CalculationRequest> {
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
