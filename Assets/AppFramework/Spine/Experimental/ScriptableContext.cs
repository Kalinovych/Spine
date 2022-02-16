using Spine.Integration;
using UnityEngine;

namespace Spine.Experimental {
	[CreateAssetMenu]
	public class ScriptableContext : ScriptableObject {
		public readonly Context context = new Context();

		public ScriptableModel[] models;

		void Awake() {
			Debug.Log( "[ScriptableContext] Awake" );
		}
	}
}
