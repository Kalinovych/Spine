using Spine.DI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof(Button) )]
public class MenuButtonMediator : Mediator {
	Button button;

	[Inject]
	ILogger logger;

	[Inject]
	LogAction Log;

	protected override void Awake() {
		print( "MenuButtonMediator.Awake" );
		base.Awake();
		button = GetComponent<Button>();
		logger.Log( "Hello" );
		Log( "Hello from the LogAction" );
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
