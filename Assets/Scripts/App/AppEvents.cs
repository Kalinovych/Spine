/**
 * System launch start event
 */
readonly struct LaunchEvent {
	public readonly string msg;

	public LaunchEvent(string msg) {
		this.msg = msg;
	}
}
