using System;
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
			menuModel.ScreenIndex = selectEvent.index;

			if (menuModel.ScreenIndex == 4 && SceneManager.GetSceneByName( "Additional" ).isLoaded)
				SceneManager.UnloadSceneAsync( "Additional" );
		}
	}
}
