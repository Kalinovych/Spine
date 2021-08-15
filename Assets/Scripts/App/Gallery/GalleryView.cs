using System;
using App;
using UnityEngine;

public class GalleryView : MonoBehaviour {
	public GalleryItemView GalleryItemPrefab;

	public int MaxCols = 1;
	public float ItemWidth = 1f;
	public float ItemHeight = 1f;
	public float ItemSpacing = 0f;

	GalleryItemView[] views = Array.Empty<GalleryItemView>();

	public void SetGallery(Gallery gallery) {
		foreach (var view in views) {
			Destroy( view.gameObject );
		}

		views = new GalleryItemView[gallery.items.Length];

		for (var i = 0; i < gallery.items.Length; i++) {
			var galleryItem = gallery.items[i];
			views[i] = Instantiate( GalleryItemPrefab, transform );
			views[i].SetItem( galleryItem );
		}
		
		LayoutItems();
	}

	void LayoutItems() {
		var viewCount = views.Length;
		var leftOrigin = -(MaxCols - 1) * (ItemWidth + ItemSpacing) * 0.5f;
		for (int i = 0; i < viewCount; i++) {
			var col = i % MaxCols;
			var row = (int)i / MaxCols;
			var left = col * (ItemWidth + ItemSpacing);

			views[i].transform.localPosition = new Vector3( leftOrigin + left, -(ItemHeight + ItemSpacing) * row, 0f );
		}
	}

	void OnValidate() {
		LayoutItems();
	}
}
