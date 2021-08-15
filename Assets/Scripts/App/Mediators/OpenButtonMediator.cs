using App.Requests;
using Spine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof(Button) )]
public class OpenButtonMediator : Mediator {
	public void OnClick() {
		Send( new OpenScreenRequest() );
	}
}
