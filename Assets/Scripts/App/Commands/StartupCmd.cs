using Spine;
using Spine.DI;

readonly struct StartupCmd : ICommand {
	[Inject]
	readonly LaunchEvent @event;

	[Inject]
	readonly LogAction Log;

	public void Execute() {
		Log( $"Startup: {@event.msg}" );
	}
}
