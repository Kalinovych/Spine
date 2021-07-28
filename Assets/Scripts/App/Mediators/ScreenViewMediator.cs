using App;
using Spine.DI;
using UnityEngine;

public class ScreenViewMediator : Mediator {
	public MeshRenderer screenMesh;

	[Inject]
	MenuModel menuModel;

	readonly Color[] colors = { Color.black, Color.white, Color.red, Color.green, Color.blue, };

	void UpdateView() {
		var material = screenMesh.material;
		material.color = colors[menuModel.screenIndex];
	}

	protected override void Awake() {
		base.Awake();

		eventHub.On<MenuItemSelect>( viewIndex => { UpdateView(); } );
	}

	void OnEnable() {
		UpdateView();
	}
}
