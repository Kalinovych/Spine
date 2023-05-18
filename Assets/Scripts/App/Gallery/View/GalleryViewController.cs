using App.Models;
using Spine;
using Spine.DI;
using UnityEngine;

namespace App {
    public class GalleryViewController : ControllerBase {
		[Inject] GalleryView view;
		[Inject] public GalleryModel model;

		void Start() {
			UpdateView();
			
			OnEvent<GalleryModel.GalleryChanged>( changed => UpdateView() );
			
			OnEvent<MenuModel.ScreenChanged>( menu => view.MaxCols = menu.itemIndex );
		}

		void UpdateView() {
			view.SetGallery( model.CurrentGallery );
		}
	}
}
