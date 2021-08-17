using App;
using Spine;
using Spine.DI;
using UnityEngine;

public class ScreenViewMediator : Mediator {
	public MeshRenderer screenMesh;

	[Inject]
	MenuModel menuModel;

	readonly Color[] colors = { Color.black, Color.white, Color.red, Color.green, Color.blue, };

	void UpdateView() {
		print( $"[{nameof(ScreenViewMediator)}] UpdateView() > menuModel.ScreenIndex: {menuModel.ScreenIndex}" );
		var material = screenMesh.material;
		material.color = colors[menuModel.ScreenIndex];
	}

	protected override void OnInitialized() {
		OnEvent<MenuModel.ScreenChanged>( OnMenuItemSelect );
	}

	void OnMenuItemSelect(MenuModel.ScreenChanged _) => UpdateView();

	protected override void OnDestroying() {
		OffEvent<MenuModel.ScreenChanged>( OnMenuItemSelect );
	}

	void OnEnable() {
		UpdateView();
	}
}
