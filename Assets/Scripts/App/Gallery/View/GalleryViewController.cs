using Spine;
using Spine.DI;
using UnityEngine;

namespace App {
	public class GalleryViewController : Mediator {
		[Inject] GalleryView view;
		[Inject] GalleryModel model;

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
