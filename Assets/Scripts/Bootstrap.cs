using Spine;
using UnityEngine;

#pragma warning disable 649
#pragma warning disable 169

public class Bootstrap : MonoBehaviour {
	Context _context;

	void Awake() {
		Configure();
	}

	void Start() {
		_context.Emit( new LaunchEvent( "Hello Spine!" ) );
	}

	void Configure() {
		_context = new Context()
			.Configure<ContextConfig>()
			.Initialize();
	}
}
