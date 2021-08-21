using App.Models;
using Spine;
using Spine.DI;
using Spine.Signals;
using UnityEngine;

namespace App.Commands {
    public struct SelectMenuItemCmd : ICommand {
		[Inject]
		MenuItemSelect selectEvent;

		[Inject]
		MenuModel menuModel;

		public void Execute() {
			Debug.Log( $"SelectMenuItemCmd: {selectEvent.index}" );
			menuModel.ScreenIndex = selectEvent.index + 1;
		}
	}
}
