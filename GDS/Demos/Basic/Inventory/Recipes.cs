using System.Collections.Generic;

namespace GDS.Basic {
    // Note: 
    // Here we are using `null` instead of a `Core.NoItemBase` because `Core.NoItemBase`
    // cannot be auto upcasted to `Basic.ItemBase` and defaults to `null`
    // This will change when the type system will be further improved (not sure how yet)
    public static class Recipes {
        public static readonly Dictionary<Recipe, ItemBase> All = new() {
            {new (Bases.Wood, Bases.Wood, Bases.Steel), Bases.Axe},
            {new (Bases.Wood, Bases.Steel, Bases.Steel), Bases.LongSword},
            {new (Bases.Gem, Bases.Wood, Bases.Steel), Bases.ShortSword},
            {new (Bases.Gem, Bases.Steel, null), Bases.BlueAmulet},
        };

    }
}