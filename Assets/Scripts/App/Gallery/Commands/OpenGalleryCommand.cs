using Spine;
using Spine.DI;

namespace App {
	public struct OpenGalleryCommand : ICommand<OpenGallery> {
		[Inject] GalleryModel model;

		public void Execute(OpenGallery request) {
			model.SetGallery( request.gallery );
		}
	}
}
