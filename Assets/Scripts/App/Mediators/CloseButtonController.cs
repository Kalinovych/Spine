using Spine;
using UnityEngine;
using UnityEngine.UI;

namespace App.Mediators {
	[RequireComponent( typeof(Button) )]
	public class CloseButtonController : ControllerBase {
		public void OnClick() {
			Send( new ClearGallery() );
		}
	}
}
