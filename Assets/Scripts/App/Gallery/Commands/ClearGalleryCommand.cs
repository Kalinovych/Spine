using System;
using Spine;
using Spine.DI;
using Spine.Signals;

namespace App {
	public struct ClearGalleryCommand : ICommand {
		//[Inject] EventHub eventHub;

		[Inject] GalleryModel model;

		public void Execute() {
			//eventHub.Send( new OpenGallery( new Gallery( Array.Empty<GalleryItem>() ) ) );
			model.SetGallery( new Gallery( Array.Empty<GalleryItem>() ) );
		}
	}
}
