using Spine.DI;
using Spine.Signals;

readonly struct StartupCmd : ICommand {
	[Inject]
	readonly LaunchEvent @event;

	[Inject]
	readonly LogAction Log;

	public void Execute() {
		Log( $"Startup: {@event.msg}" );
	}
}
