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
		var material = screenMesh.material;
		material.color = colors[menuModel.ScreenIndex];
	}

	protected override void OnInitialized() {
		OnEvent<MenuEvent>( OnMenuItemSelect );
	}

	void OnMenuItemSelect(MenuEvent _) => UpdateView();

	protected override void OnPreDestroy() {
		OffEvent<MenuEvent>( OnMenuItemSelect );
	}

	void OnEnable() {
		UpdateView();
	}
}
