using Spine;
using UnityEngine;

namespace App {
	public class MenuModel : Model {
		int screenIndex;

		public MenuModel() {
			Debug.LogWarning( $"MENU MODEL" );
		}

		public int ScreenIndex {
			get => screenIndex;
			set {
				if (value == screenIndex) return;
				
				screenIndex = value;
				
				Emit( new MenuEvent( screenIndex ) );
			}
		}
	}

	public readonly struct MenuEvent {
		public readonly int itemIndex;

		public MenuEvent(int itemIndex) {
			this.itemIndex = itemIndex;
		}
	}
}
