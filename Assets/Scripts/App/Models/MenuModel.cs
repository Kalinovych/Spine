using Spine;

namespace App.Models {
    public class MenuModel : ModelBase {
        int screenIndex;

        public int ScreenIndex {
            get => screenIndex;
            set {
                if (value == screenIndex) return;

                screenIndex = value;

                Emit(new ScreenChanged(screenIndex));
            }
        }

        public readonly struct ScreenChanged {
            public readonly int itemIndex;

            public ScreenChanged(int itemIndex) {
                this.itemIndex = itemIndex;
            }
        }
    }
}
