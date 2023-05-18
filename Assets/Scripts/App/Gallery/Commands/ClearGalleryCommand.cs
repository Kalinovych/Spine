using System;
using Spine;
using Spine.DI;
using Spine.Signals;

namespace App {
	public struct ClearGalleryCommand : ICommand<ClearGallery> {
		[Inject] EventHub eventHub;

		public void Execute(ClearGallery _) {
			eventHub.Send( new OpenGallery( new Gallery( Array.Empty<GalleryItem>() ) ) );
		}
	}
}
