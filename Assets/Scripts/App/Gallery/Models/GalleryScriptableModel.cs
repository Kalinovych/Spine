using System;
using Spine.Integration;
using UnityEngine;

namespace App {
	[CreateAssetMenu]
	public class GalleryScriptableModel : ScriptableModelBase {
		public Gallery CurrentGallery { get; set; } = new Gallery( Array.Empty<GalleryItem>() );

		public void SetGallery(Gallery gallery) {
			CurrentGallery = gallery;
			Send( new GalleryModel.GalleryChanged() );
		}
	}
}
