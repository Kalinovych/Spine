using System.Linq;
using Spine.DI;
using Spine.Signals;
using UnityEngine;

namespace Spine.Experiments {
	public class V3Controller : MonoBehaviour {
		Context context;

		void Start() {
			print( "V3Mediator.Start" );

			context = BuildContext()
					.Send( new CalculateRequest( "2+2" ) )
					.Send( new CalculateRequest( "2+3" ) )
				;
		}

		static Context BuildContext()
			=> new Context()
			.WithEventHub()
			.WithCommandHub()
			.ConfigureModel( configurator => {
				configurator.Add<ICalculationService, AddCalculationService>();
			} )
			.ConfigureCommands( hub => {
				hub.Map<CalculateRequest, CalculateCommand>();
				hub.Map<CalculationResult, PrintResultToLog>();
			} );
	}

	//--- App ---//

	readonly struct PrintResultToLog : ICommand<CalculationResult> {
		public void Execute(CalculationResult result) {
			Debug.Log( $"Result: {result.Value}" );
		}
	}

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
		[Inject] EventHub eventHub;

		public void Execute(CalculateRequest request) {
			Debug.Log( $"Calculate: {request.input}" );

			var result = service.Calculate( request.input );
			eventHub.Send( new CalculationResult( result ) );
		}
	}

	interface ICalculationService {
		float Calculate(string input);
	}

	class TwoPlusTwoCalculationService : ICalculationService {
		[Inject] EventHub eventHub;

		public float Calculate(string input) {
			return 4;
		}
	}

	class AddCalculationService : ICalculationService {
		[Inject] EventHub eventHub;

		public float Calculate(string input) {
			return Calc( input );
		}

		public float Calc(string input) {
			Debug.Log( $"AddCalculationService.Calc: {input}" );
			var parts = input.Split( '+' ).Select( float.Parse ).ToArray();
			return parts[0] + parts[1];
		}
	}
}
