using System;
using Spine.DI;
using Spine.Signals;

namespace App {
	public struct ClearGalleryCommand : ICommand {
		[Inject] EventHub eventHub;

		public void Execute() {
			eventHub.Send( new OpenGallery( new Gallery( Array.Empty<GalleryItem>() ) ) );
		}
	}
}
