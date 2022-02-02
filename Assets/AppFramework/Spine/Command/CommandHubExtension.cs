using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Spine {
	public static class CommandHubExtension {
		public static Context WithCommandHub(this Context context) {
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

		public static Context AutoConfigureCommands(this Context context) {
			var hub = context.injector.Get<CommandHub>();
			var assembly = Assembly.GetCallingAssembly();
			var commands = GetCommandTypes( assembly );
			foreach (var commandType in commands) {
				Debug.Log( $" → Command found: {commandType} : {commandType.GetInterfaces()[0].GetGenericArguments()[0]}" );
				var requestedType = commandType.GetInterfaces()[0].GetGenericArguments()[0];
				hub.Map( requestedType, commandType );
			}

			return context;
		}

		static IEnumerable<Type> GetCommandTypes(Assembly assembly) {
			var commands = assembly
				.ExportedTypes
				.Where( type => {
					return type
						.GetInterfaces()
						.Any( i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>) );
				} );
			return commands;
		}
	}
}
