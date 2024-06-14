using System.Collections.Generic;
using UnityEngine;

namespace GDS {

    public static class ItemExt {
        public static Sprite Image(this Item item) => DB.Icons.GetValueOrDefault(item.Type);
    }
}