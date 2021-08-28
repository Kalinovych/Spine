using Spine;
using Spine.DI;
using Spine.Signals;
using UnityEngine;

namespace App {
	public struct LoadDemoGalleryCommand : ICommand<OpenDemoGallery> {
		[Inject] EventHub eventHub;

		public void Execute(OpenDemoGallery _) {
			eventHub.Send( new OpenGallery( DemoGalleryProvider() ) );
		}

		Gallery DemoGalleryProvider() {
			const int ItemCount = 12;
			var items = new GalleryItem[ItemCount];
			for (int i = 0; i < ItemCount; i++) {
				items[i].color = Color.black;
			}

			return new Gallery( items );
		}
	}
}
