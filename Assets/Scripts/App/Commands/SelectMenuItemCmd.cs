using Spine.DI;
using Spine.Signals;

namespace App.Commands {
	public struct SelectMenuItemCmd : ICommand {
		[Inject]
		MenuItemSelect selectEvent;
		
		[Inject]
		MenuModel menuModel;

		[Inject]
		LogAction Log;

		public void Execute() {
			Log( $"SelectMenuItemCmd: {selectEvent.index}" );
			menuModel.screenIndex = selectEvent.index;
		}
	}
}
