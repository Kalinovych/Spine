using Spine;
using Spine.DI;
using Spine.Signals;
using UnityEngine;

namespace App {
	public struct LoadDemoGalleryCommand : ICommand {
		[Inject] GalleryModel model;

		[Inject] EventHub eventHub;

		public void Execute() {
			model.SetGallery( DemoGalleryProvider() );
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
