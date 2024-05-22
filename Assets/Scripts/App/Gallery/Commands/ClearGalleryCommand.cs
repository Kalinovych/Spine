using System;
using Spine;
using Spine.DI;
using Spine.Signals;

namespace App {
	public struct ClearGalleryCommand : ICommand<ClearGallery> {
		[Inject] EventHub eventHub;

		public void Execute(ClearGallery _)
		{
			var emptyGallery = new Gallery( Array.Empty<GalleryItem>() );
			eventHub.Send( new OpenGallery( emptyGallery ) );
		}
	}
}
