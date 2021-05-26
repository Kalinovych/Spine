using System;
using App;
using Spine.DI;
using Spine.Signals;
using UnityEngine;

public class ScreenViewMediator : Mediator {
	public MeshRenderer screenMesh;

	[Inject]
	MenuModel menuModel;

	[Inject]
	EventHub eventHub;

	readonly Color[] colors = new[] {Color.black, Color.white, Color.red, Color.green, Color.blue,};

	void UpdateView() {
		var material = screenMesh.material;
		material.color = colors[menuModel.screenIndex];
	}

	protected override void Awake() {
		base.Awake();
		
		eventHub.On<MenuItemsSelect>( viewIndex => { UpdateView(); } );
	}

	void OnEnable() {
		UpdateView();
	}
}
