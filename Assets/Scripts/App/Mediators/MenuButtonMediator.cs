using Spine;
using Spine.DI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof(Button) )]
public class MenuButtonMediator : Mediator {
	public int menuItemIndex;

	[Inject]
	Button button;

	void Start() {
		button = GetComponent<Button>();
		button.onClick.AddListener( OnClick );
	}

	void OnClick() {
		Send( new MenuItemSelect( menuItemIndex ) );
	}
}

public readonly struct MenuItemSelect {
	public readonly int index;

	public MenuItemSelect(int index) {
		this.index = index;
	}
}
