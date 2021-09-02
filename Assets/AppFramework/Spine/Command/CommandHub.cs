using System;
using Spine.DI;
using Spine.Signals;

namespace Spine {
	public class CommandHub {
		[Inject] Injector injector;
		[Inject] EventHub eventHub;

		public CommandHub Map<TRequest, TCommand>() where TCommand : struct, ICommand<TRequest> {
			var ce = new CommandExecutor<TRequest, TCommand>( injector );
			eventHub.On<TRequest>( ce.Execute );
			return this;
		}

		public CommandHub Map(Type requestType, Type commandType) {
			return this;
		}
	}

	readonly struct CommandExecutor<TRequest, TCommand> where TCommand : struct, ICommand<TRequest> {
		readonly Injector injector;

		public CommandExecutor(Injector injector) {
			this.injector = injector;
		}

		public void Execute(TRequest request) {
			injector.Resolve<TCommand>().Execute( request );
		}
	}

	public interface ICommand<in TRequest> {
		void Execute(TRequest request);
	}
}
