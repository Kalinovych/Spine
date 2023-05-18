using App;
using Spine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof(Button) )]
public class OpenButtonController : ControllerBase {
	public void OnClick() {
		Send( new OpenDemoGallery() );
	}
}
