using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof(Button) )]
public class MenuButtonMediator : Mediator {
	public int menuItemIndex;
	
	Button button;

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
		Emit( new MenuItemsSelect( menuItemIndex ) );
	}
}

public readonly struct MenuItemsSelect {
	public readonly int index;

	public MenuItemsSelect(int index) {
		this.index = index;
	}
}
