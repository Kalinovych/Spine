using System;

namespace Spine {
	public static class CommandHubExtension {
		public static Context InstallCommandHub(this Context context) {
			context.injector.Add<CommandHub>();
			return context;
		}

		public static Context ConfigureCommands(this Context context, Action<CommandHub> configure) {
			if (configure is null) {
				throw new ArgumentNullException( nameof(configure) );
			}

			configure( context.injector.Get<CommandHub>() );
			return context;
		}
		public static Context ConfigureCommands(this Context context, ICommandsConfigurator configurator) {
			if (configurator is null) {
				throw new ArgumentNullException( nameof(configurator) );
			}

			configurator.Configure( context.injector.Get<CommandHub>() );
			return context;
		}
	}


	public interface ICommandsConfigurator {
		void Configure(CommandHub commandHub);
	}

}
