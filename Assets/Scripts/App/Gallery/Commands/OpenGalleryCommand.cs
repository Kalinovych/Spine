using Spine;
using Spine.DI;

namespace App {
	public readonly struct OpenGalleryCommand : ICommand<OpenGallery> {
		[Inject] readonly GalleryModel model;

		public void Execute(OpenGallery request) {
			model.SetGallery( request.gallery );
		}
	}
}
