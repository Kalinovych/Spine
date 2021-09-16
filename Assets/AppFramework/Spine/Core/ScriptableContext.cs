using Spine.Integration;
using UnityEngine;

namespace Spine {
	[CreateAssetMenu]
	public class ScriptableContext : ScriptableObject {
		public readonly Context context = new Context();

		public ScriptableModelBase[] models;
	}
}
