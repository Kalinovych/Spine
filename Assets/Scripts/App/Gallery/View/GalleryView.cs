using System;
using App;
using UnityEngine;

public class GalleryView : MonoBehaviour {
	public GalleryItemView GalleryItemPrefab;

	[SerializeField] int maxCols = 1;
	[SerializeField] float itemWidth = 1f;
	[SerializeField] float itemHeight = 1f;
	[SerializeField] float itemSpacing = 0f;

	GalleryItemView[] views = Array.Empty<GalleryItemView>();

	public int MaxCols {
		get => maxCols;
		set {
			if (value != maxCols) {
				maxCols = value;
				LayoutItems();
			}
		}
	}

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

	public void LayoutItems() {
		var viewCount = views.Length;
		var leftOrigin = -(MaxCols - 1) * (itemWidth + itemSpacing) * 0.5f;
		for (int i = 0; i < viewCount; i++) {
			var col = i % MaxCols;
			var row = (int)i / MaxCols;
			var left = col * (itemWidth + itemSpacing);

			views[i].transform.localPosition = new Vector3( leftOrigin + left, -(itemHeight + itemSpacing) * row, 0f );
		}
	}

	void OnValidate() {
		LayoutItems();
	}
}
