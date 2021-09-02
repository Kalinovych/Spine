using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

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

		public static Context AutoConfigureCommands(this Context context) {
			var assembly = Assembly.GetCallingAssembly();
			var commands = assembly
				.ExportedTypes
				.Where( type => type
					.GetInterfaces()
					.Any( i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>) ) );
			
			foreach (var type in commands) {
				Debug.Log( $" → Command found: {type}" );
			}
			return context;
		}
	}
}
