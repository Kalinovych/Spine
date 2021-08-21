using Spine.DI;
using Spine.Signals;

namespace Spine {
	public class CommandHub {
		[Inject] Injector injector;
		[Inject] EventHub eventHub;

		public EventMapper<T> On<T>() => new EventMapper<T>( injector, eventHub );

		public EventMapper<T> Map<T>() => new EventMapper<T>( injector, eventHub );

		public void Map<TRequest, TCommand>() where TCommand : struct, ICommand<TRequest> {
			var ce = new CommandExecutor<TRequest, TCommand>( injector );
			eventHub.On<TRequest>( ce.Execute );
		}
	}

	public readonly struct EventMapper<T> {
		readonly Injector injector;
		readonly EventHub eventHub;

		public EventMapper(Injector injector, EventHub eventHub) {
			this.injector = injector;
			this.eventHub = eventHub;
		}

		public void Do<TCmd>() where TCmd : struct, ICommand {
			var ce = new CommandExecutor<TCmd>( injector );
			eventHub.On<T>( signal => ce.Execute( signal ) );
		}

		public void To<TCmd>() where TCmd : struct, ICommand {
			var ce = new CommandExecutor<TCmd>( injector );
			//eventHub.On<T>( signal => ce.Execute( signal ) );
			eventHub.On<T>( ce.Execute );
		}
	}

	readonly struct CommandExecutor<TCommand> where TCommand : struct, ICommand {
		readonly Injector injector;

		public CommandExecutor(Injector injector) {
			this.injector = injector;
		}

		public void Execute<TSignal>(TSignal signal) {
			// create
			TCommand cmd = injector.With( signal ).Resolve<TCommand>();
			// execute
			cmd.Execute();
		}
	}

	readonly struct CommandExecutor<TRequest, TCommand> where TCommand : struct, ICommand<TRequest> {
		readonly Injector injector;

		public CommandExecutor(Injector injector) {
			this.injector = injector;
		}

		public void Execute(TRequest request) {
			injector.With( request ).Resolve<TCommand>().Execute( request );
		}
	}

	public interface ICommand {
		void Execute();
	}


	public interface ICommand<in TRequest> {
		void Execute(TRequest request);
	}

	public static class CommandHubExtension {
		public static Context InstallCommandHub(this Context context) {
			context.injector.MapSingleton<CommandHub>();
			return context;
		}

		public static Context Send<T>(this Context context, T signal) {
			return context;
		}
	}
}
