using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof(Button) )]
public class MenuButtonMediator : Mediator {
	Button button;

	void Awake() {
		print( $"MenuButtonMediator.Awake, eventHub: {eventHub}" );
		button = GetComponent<Button>();
		print( $"Awake end, eventHub: {eventHub}" );
	}

	void OnEnable() {
		print( $"OnEnable, eventHub: {eventHub}" );
		button.onClick.AddListener( OnClick );
	}

	void OnClick() {
		print( $"OnClick, eventHub: {eventHub}" );
		Emit( new ButtonClickEvent() );
	}
}

public struct ButtonClickEvent {
}
