using UnityEngine;

public delegate void LogAction(object msg);

namespace App.Utils {
	interface ILogger {
		public void Log(object msg);
		public void Warn(object msg);
	}

	class DefaultLogger : ILogger {
		public void Log(object msg) {
			Debug.LogWarning( msg );
		}

		public void Warn(object msg) {
			Debug.LogWarning( msg );
		}

		public static void LogStatic(object msg) {
			Debug.Log( msg );
		}

		public static void LogWarnStatic(object msg) {
			Debug.LogWarning( msg );
		}
	}
}
