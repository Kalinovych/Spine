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

	readonly struct CommandExecutor<T> where T : struct, ICommand {
		readonly Injector injector;

		public CommandExecutor(Injector injector) {
			this.injector = injector;
		}

		public void Execute<TSignal>(TSignal signal) {
			// create
			T cmd = injector.With( signal ).Inject<T>();
			// execute
			cmd.Execute();
		}
	}

	public interface ICommand {
		void Execute();
	}
}
