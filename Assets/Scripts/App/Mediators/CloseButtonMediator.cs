using App.Requests;
using Spine;
using UnityEngine;
using UnityEngine.UI;

namespace App.Mediators {
	[RequireComponent( typeof(Button) )]
	public class CloseButtonMediator : Mediator {
		public void OnClick() {
			Send( new CloseScreenRequest() );
		}
	}
}
