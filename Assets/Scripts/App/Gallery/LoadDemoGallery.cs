using Spine.DI;
using Spine.Signals;
using UnityEngine;

namespace App {
	public struct LoadDemoGallery : ICommand {
		[Inject] GalleryModel model;

		public void Execute() {
			model.gallery = DemoGalleryProvider();
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
