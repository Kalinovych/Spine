using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof(Button) )]
public class MenuButtonMediator : Mediator {
	Button button;

	void Awake() {
		button = GetComponent<Button>();
	}

	void OnEnable() {
		button.onClick.AddListener( OnClick );
	}

	void OnClick() {
		Emit( new ButtonClickEvent() );
	}
}

public struct ButtonClickEvent {
}
