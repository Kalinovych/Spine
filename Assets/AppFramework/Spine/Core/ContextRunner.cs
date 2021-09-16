using System;
using UnityEditor;
using UnityEngine;

namespace Spine {
	[CreateAssetMenu]
	public class ContextRunner : ScriptableSingleton<ContextRunner> {
		public ScriptableContext context;
		
		void Awake() {
			
		}
	}
}
