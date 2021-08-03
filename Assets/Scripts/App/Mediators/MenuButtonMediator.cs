using Spine.DI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof(Button) )]
public class MenuButtonMediator : Mediator {
	public int menuItemIndex;

	[Inject]
	Button button;

	protected override void OnInitialized() {
		//button = GetComponent<Button>();
	}

	void OnEnable() {
		button.onClick.AddListener( OnClick );
	}

	void OnClick() {
		Emit( new MenuItemSelect( menuItemIndex ) );
	}
}

public readonly struct MenuItemSelect {
	public readonly int index;

	public MenuItemSelect(int index) {
		this.index = index;
	}
}
