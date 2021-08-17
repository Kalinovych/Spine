using App;
using UnityEngine;

public class GalleryItemView : MonoBehaviour {
	public MeshRenderer meshRenderer;

	public void SetItem(GalleryItem item) {
		SetColor( item.color );
	}

	public void SetColor(Color color) {
		meshRenderer.material.color = color;
	}
}
