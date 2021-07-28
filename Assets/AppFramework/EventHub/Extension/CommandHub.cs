using Spine.DI;

namespace Spine.Signals {
	public readonly struct SignalMapper<T> {
		readonly Injector injector;
		readonly EventHub eventHub;

		public SignalMapper(Injector injector, EventHub eventHub) {
			this.injector = injector;
			this.eventHub = eventHub;
		}

		public void Do<TCmd>() where TCmd : struct, ICommand {
			var ce = new CommandExecutor<TCmd>( injector );
			eventHub.On<T>( signal => ce.Execute( signal ) );
		}
	}

	readonly struct CommandExecutor<TCommand> where TCommand : struct, ICommand{
		readonly Injector injector;

		public CommandExecutor(Injector injector) {
			this.injector = injector;
		}

		public void Execute<TSignal>(TSignal signal) {
			// create
			TCommand cmd = injector.With( signal ).Create<TCommand>();
			// execute
			cmd.Execute();
		}
	}

	public interface ICommand {
		void Execute();
	}


	public interface ICommand<in T> {
		void Execute(T signal);
	}
}
