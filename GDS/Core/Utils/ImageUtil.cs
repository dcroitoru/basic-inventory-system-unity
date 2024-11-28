using System.Collections.Generic;
using GDS.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDS.Core {

    public static class ImageUtil {
        public static Sprite Image(this Item item) => Resources.Load<Sprite>(item.ItemBase.IconPath);
        public static Image Image(Sprite sprite) => new Image(sprite);
    }

    // TODO: Do we need to use this or can we use the built-in Image?
    public class Image : VisualElement {
        public Image(Sprite sprite) { style.backgroundImage = new StyleBackground(sprite); }
    }
}