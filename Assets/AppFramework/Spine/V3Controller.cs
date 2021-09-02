﻿using System.Linq;
using Spine.DI;
using Spine.Signals;
using UnityEngine;

namespace Spine.Experiments {
	public class V3Controller : MonoBehaviour {
		Context context;

		void Start() {
			print( "V3Mediator.Start" );

			context = new Context()
					.InstallEventHub()
					.InstallCommandHub()
					.ConfigureModel( configurator => {
						configurator.Add<ICalculationService, AddCalculationService>();
					} )
					.ConfigureCommands( hub => {
						hub.Map<CalculateRequest, CalculateCommand>();
						hub.Map<CalculationResult, PrintResultToLog>();
					} )
					.Send( new CalculateRequest( "2+2" ) )
					.Send( new CalculateRequest( "2+3" ) )
				;
		}
	}

	readonly struct PrintResultToLog : ICommand<CalculationResult> {
		public void Execute(CalculationResult result) {
			Debug.Log( $"Result: {result.Value}" );
		}
	}

	//--- App ---//

	readonly struct CalculateRequest {
		public readonly string input;

		public CalculateRequest(string input) {
			this.input = input;
		}
	}

	readonly struct CalculationResult {
		public readonly float Value;

		public CalculationResult(float value) {
			Value = value;
		}
	}

	struct CalculateCommand : ICommand<CalculateRequest> {
		[Inject] ICalculationService service;

		public void Execute(CalculateRequest request) {
			Debug.Log( $"Calculate: {request.input}" );

			service.Calculate( request.input );
		}
	}

	interface ICalculationService {
		float Calc(string input);

		void Calculate(string input);
	}

	class TwoPlusTwoCalculationService : ICalculationService {
		public float Calc(string input) {
			Debug.Log( $"TwoPlusTwoCalculationService.Calc: {input}" );
			return 4;
		}

		[Inject] EventHub eventHub;

		public void Calculate(string input) {
			eventHub.Send( new CalculationResult( 4 ) );
		}
	}

	class AddCalculationService : ICalculationService {
		public float Calc(string input) {
			Debug.Log( $"AddCalculationService.Calc: {input}" );
			var parts = input.Split( '+' ).Select( float.Parse ).ToArray();
			return parts[0] + parts[1];
		}

		[Inject] EventHub eventHub;

		public void Calculate(string input) {
			eventHub.Send( new CalculationResult( Calc( input ) ) );
		}
	}
}
