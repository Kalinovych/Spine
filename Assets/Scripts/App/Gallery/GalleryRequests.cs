namespace App {
	public readonly struct OpenDemoGallery {
	}

	public readonly struct OpenGallery {
		public readonly Gallery gallery;

		public OpenGallery(Gallery gallery) {
			this.gallery = gallery;
		}
	}

	public readonly struct ClearGallery {
	}
}
