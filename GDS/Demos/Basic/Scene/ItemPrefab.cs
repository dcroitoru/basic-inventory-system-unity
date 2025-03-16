using UnityEngine;
using GDS.Core;
namespace GDS.Basic {
    public class ItemPrefab : MonoBehaviour {
        public Item Item;
        public SpriteRenderer spriteRenderer;

        public void SetItem(Item item) {
            Item = item;
            var sprite = Resources.Load<Sprite>(item.ItemBase.IconPath);
            spriteRenderer.sprite = sprite;
        }
    }
}