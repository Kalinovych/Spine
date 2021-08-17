using System;
using Spine;
using UnityEngine;

namespace App {
	public class GalleryModel : ModelBase {
		public struct GalleryChanged {}
		
		public Gallery CurrentGallery { get; set; } = new Gallery( Array.Empty<GalleryItem>() );

		public void SetGallery(Gallery gallery) {
			CurrentGallery = gallery;
			Send( new GalleryChanged() );
		}
	}

	public readonly struct Gallery {
		public readonly GalleryItem[] items;

		public Gallery(GalleryItem[] items) {
			this.items = items;
		}
	}

	public struct GalleryItem {
		public Color color;
	}
}
