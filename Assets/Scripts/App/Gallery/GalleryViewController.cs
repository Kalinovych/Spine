using System;
using Spine;
using Spine.DI;

namespace App {
	public class GalleryViewController : Mediator {
		[Inject] GalleryView view;
		[Inject] GalleryModel model;

		void Start() {
			UpdateView();
		}

		void UpdateView() {
			view.SetGallery( model.gallery );
		}
	}
}
