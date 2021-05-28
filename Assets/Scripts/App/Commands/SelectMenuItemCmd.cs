using Spine.DI;
using Spine.Signals;

namespace App.Commands {
	public struct SelectMenuItemCmd : ICommand {
		[Inject]
		MenuModel menuModel;

		[Inject]
		MenuItemSelect selectEvent;

		[Inject]
		LogAction Log;

		public void Execute() {
			Log( $"SelectMenuItemCmd: {selectEvent.index}" );
			menuModel.screenIndex = selectEvent.index;
		}
	}
}
