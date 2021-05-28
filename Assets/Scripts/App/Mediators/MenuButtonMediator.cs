using App;
using Spine.DI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof(Button) )]
public class MenuButtonMediator : Mediator {
	public int menuItemIndex;
	
	Button button;

	[Inject]
	MenuModel menuModel;

	protected override void Awake() {
		base.Awake();
		button = GetComponent<Button>();
	}

	void OnEnable() {
		print( $"OnEnable, eventHub: {eventHub}" );
		button.onClick.AddListener( OnClick );
	}

	void OnClick() {
		print( $"OnClick, eventHub: {eventHub}" );
		Emit( new MenuItemSelect( menuItemIndex ) );
	}
}

public readonly struct MenuItemSelect {
	public readonly int index;

	public MenuItemSelect(int index) {
		this.index = index;
	}
}
