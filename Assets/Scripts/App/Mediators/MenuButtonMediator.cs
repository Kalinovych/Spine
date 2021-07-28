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
	
	protected override void OnInitialized() {
		button = GetComponent<Button>();
	}

	void OnEnable() {
		print( $"OnEnable, eventHub: {eventHub}" );
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
