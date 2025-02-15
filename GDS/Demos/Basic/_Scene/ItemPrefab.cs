using GDS.Core;
using UnityEngine;

public class ItemPrefab : MonoBehaviour {


    public Item Item;
    public SpriteRenderer spriteRenderer;

    private void Awake() {
        Debug.Log("should create item");
    }

    public void SetItem(Item item) {
        Item = item;
        Debug.Log("should create a sprite from item " + item.ItemBase.Name);
        var sprite = Resources.Load<Sprite>(item.ItemBase.IconPath);
        Debug.Log($"sprite {sprite}");
        spriteRenderer.sprite = sprite;
        // Image.sprite
    }

}
