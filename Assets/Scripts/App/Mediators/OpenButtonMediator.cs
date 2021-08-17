using App;
using Spine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof(Button) )]
public class OpenButtonMediator : Mediator {
	public void OnClick() {
		Send( new OpenDemoGallery() );
	}
}
