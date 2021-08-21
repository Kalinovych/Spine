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
				.Initialize()
				.Send( new CalculationRequest() );
		}
	}

	static class CalculatorExtension {
		public static void Calculate(this Context context) {
			context.injector.Resolve<EventHub>().Send( new CalculationRequest() );
		}
	}

	struct CalculatorAppContextConfig : IContextConfig {
		[Inject] CommandHub commandHub;

		public void Configure() {
			commandHub.Map<CalculationRequest, CalculateCommand>();
		}
	}

	struct CalculationRequest {
	}

	struct CalculateCommand : ICommand<CalculationRequest> {
		public void Execute(CalculationRequest request) {
			Debug.Log( $"CalculateCommand.Execute: {request}" );
		}
	}
}
