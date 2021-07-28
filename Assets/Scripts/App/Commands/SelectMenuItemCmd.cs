using Spine.DI;
using Spine.Signals;
using UnityEngine.SceneManagement;

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

			if (menuModel.screenIndex == 4)
				SceneManager.UnloadSceneAsync( "Additional" );
		}
	}
}
