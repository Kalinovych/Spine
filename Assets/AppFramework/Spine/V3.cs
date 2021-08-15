using Spine.DI;
using Spine.Signals;
using UnityEngine;

namespace Spine {
	// --- //

	class V3Program {
		static void Log(object msg) => Debug.Log( msg );

		public void Run() {
			
			/***
			 * Map<GalleryModel>();
			 * Map<LoadURL>().To<LoadURLCommand>(); // triggers page service to load requested url
			 * Map<IPageService>().To<HttpJsonService>(); // has some result of work
			 * Map<PageLoadingResult>().To<HandleLoadedPage>();
			 * 
			 * Send(new LoadURL("/home"));
			 *
			 *	class GalleryModel {
			 *  }
			 * 
			 * 
			 * struct LoadURLCommand {
			 *		IPageService service;
			 *		Execute(LoadURL request) {
			 *			service.LoadAsync(request.url);
			 *		}
			 * }
			 *
			 * struct JsonPageHandler {
			 *		
			 *		void Handle(string json) {
			 *			
			 *		}
			 * }
			 */
			
			/***
			 * var hub = new Hub();
			 * hub.Map<VolumeModel>();
			 * hub.Map<IVolumeService>().To<SystemVolumeService>();
			 * hub.On<Startup>().Do<LoadVolumeCommand>();
			 * hub.On<SetVolume>().Do<SetVolumeCommand>();
			 * hub.Send(new SetVolume( 0.5f ));
			 *
			 * struct SetVolumeCommand {
			 *		VolumeModel model;
			 *		IVolumeService service;
			 *		void Execute(SetVolume request) {
			 *			model.Volume = request.Volume;
			 *			service.SetVolume(model.Volume);
			 *		}
			 * }
			 *
			 * struct LoadVolumeCommand {
			 *		IVolumeService service;
			 *		VolumeModel model;
			 *		void Execute() {
			 *			model.Volume = service.GetVolume();
			 *		}
			 * }
			 */

			/***
			 * var hub = new Hub();
			 * hub.Map<ICalculator>().To(new TaxCalculator(0.2f));
			 * hub.On<CalcTaxRequest>.To<CalculateTaxCommand>();
			 * hub.Send(new CalcTaxRequest(1000500f));
			 *
			 * struct CalcTaxRequest {
			 */

			Log( "Run" );

			var hub = new Hub();
			//hub.On<CalculateTax>().Do<CalculateTaxHandler>();
			//hub.On<TaxReport>().Do<PrintTaxReportToConsole>();
			hub.Map<ICalculator>().To( new TaxCalculator( 0.2f ) );
			hub.Send( new CalculateTax( 100500f ) );
		}
	}

	// APP //

	interface ICalculator {
		float Calculate(float input);
	}

	readonly struct TaxCalculator : ICalculator {
		readonly float taxRate;

		public TaxCalculator(float taxRate) {
			this.taxRate = taxRate;
		}

		public float Calculate(float input) {
			Debug.Log( $"TaxCalculator.Calculate( {input} )" );
			return input * taxRate;
		}
	}

	readonly struct CalculateTax {
		public readonly float income;

		public CalculateTax(float income) {
			this.income = income;
		}
	}

	readonly struct CalculateTaxHandler : IHandler<CalculateTax, TaxReport> {
		//[Inject]
		//readonly ICalculator calculator;

		public TaxReport Handle(CalculateTax request) {
			Debug.Log( $"CalculateTaxHandler.Handle( {request} )" );
			return new TaxReport( request.income * 0.2f );
			//return new TaxReport( calculator.Calculate( request.income ) );
		}
	}

	readonly struct TaxReport {
		public readonly float taxAmount;

		public TaxReport(float taxAmount) {
			this.taxAmount = taxAmount;
		}
	}

	struct PrintTaxReportToConsole : IHandler<TaxReport> {
		public void Handle(TaxReport report) {
			Debug.Log( $"Tax for income %income% is {report.taxAmount}" );
		}
	}

	// FRAMEWORK //

	class Hub {
		readonly EventHub eventHub = new EventHub();
		readonly HandlerHub handlerHub;

		public Hub() {
			handlerHub = new HandlerHub( eventHub );
		}

		public DMapper<T> Map<T>() {
			return new DMapper<T>();
		}

		public struct DMapper<T> {
			public void To(T instance) {
			}
		}

		//public HandlerHub.Mapper<T> On<T>() {
		//	return handlerHub.Map<T>();
		//}

		public void Send<T>(T request) {
			Debug.Log( $"Hub.Send<{typeof(T)}>( {request} )" );

			eventHub.Send( request );
		}
	}

	class HandlerHub {
		readonly EventHub eventHub;

		public HandlerHub(EventHub eventHub) {
			this.eventHub = eventHub;
		}

		public Mapper<TEvent, TResult> Map<TEvent, TResult>() {
			return new Mapper<TEvent, TResult>(eventHub);
		}

		public readonly struct Mapper<TInput, TOutput> {
			readonly EventHub eventHub;

			public Mapper(EventHub eventHub) {
				this.eventHub = eventHub;
			}

			public void Do<THandler>() where THandler : IHandler<TInput, TOutput>, new() {
				//var handler = injector.Resolve<THandler>();
				eventHub.On<TInput>( input => {
					var handler = new THandler();
					var result = handler.Handle( input );
				} );

				var handler = new HandlerExecutor<THandler, TInput, TOutput>( eventHub );
				eventHub.On<TInput>( handler.Handle );
			}
		}

		readonly struct HandlerExecutor<THandler, TInput, TOutput> where THandler : IHandler<TInput, TOutput>, new() {
			readonly EventHub eventHub;

			public HandlerExecutor(EventHub eventHub) {
				this.eventHub = eventHub;
			}
			
			public void Handle(TInput input) {
				var handler = new THandler();
				var result = handler.Handle( input );
				eventHub.Send( result );
			}
		}
	}

	interface IHandler<in TRequest> {
		void Handle(TRequest request);
	}
	interface IHandler<in TRequest, out TResponse> {
		TResponse Handle(TRequest request);
	}



	/*
	public readonly struct SelectMenuScreenRequest {
		public readonly int index;

		public SelectMenuScreenRequest(int index) {
			this.index = index;
		}
	}

	readonly struct SelectMenuScreenCommand : ICommand<SelectMenuScreenRequest> {
		readonly MenuModel menuModel;

		public SelectMenuScreenCommand(MenuModel menuModel) {
			this.menuModel = menuModel;
		}

		public void Execute(SelectMenuScreenRequest signal) {
			menuModel.ScreenIndex = signal.index;
		}
	}

	readonly struct SelectMenuScreenCommandProvider : ICommandProvider<SelectMenuScreenCommand> {
		readonly IDependencyProvider<MenuModel> dependencyProvider;

		public SelectMenuScreenCommandProvider(IDependencyProvider<MenuModel> dependencyProvider) {
			this.dependencyProvider = dependencyProvider;
		}

		readonly 
		public SelectMenuScreenCommand Get() {
			return new SelectMenuScreenCommand(dependencyProvider.Get());
		}
	}

	interface IDependencyProvider<T> {
		T Get();
	}

	interface IHandler<in TRequest> {
		void Handle(TRequest request);
	}

	readonly struct CommandRequestHandler<TRequest, TCommand> : IHandler<TRequest> where TCommand : struct, ICommand<TRequest> {
		readonly ICommandProvider<TCommand> commandProvider;

		public CommandRequestHandler(ICommandProvider<TCommand> commandProvider) {
			this.commandProvider = commandProvider;
		}


		public void Handle(TRequest request) {
			// create
			TCommand cmd = commandProvider.Get();
			// execute
			cmd.Execute( request );
		}
	}

	interface ICommandProvider<out TCommand> {
		public TCommand Get();
	}

	readonly struct CommandProvider {
		readonly Injector injector;

		public CommandProvider(Injector injector) {
			this.injector = injector;
		}

		public TCommand Get<TCommand>() {
			return default(TCommand);
		}
	}

	class TheContext {
		public T Resolve<T>() {
			return default;
		}
	}
	*/
}
