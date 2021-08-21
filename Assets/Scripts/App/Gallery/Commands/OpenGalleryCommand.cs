using Spine;
using Spine.DI;

namespace App {
	public struct OpenGalleryCommand : ICommand {
		[Inject] OpenGallery request;
		[Inject] GalleryModel model;

		public void Execute() {
			model.SetGallery( request.gallery );
		}
	}
}
