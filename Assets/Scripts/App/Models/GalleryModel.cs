using System;
using UnityEngine;

namespace App {
	public class GalleryModel {
		public Gallery gallery = new Gallery( Array.Empty<GalleryItem>() );
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
