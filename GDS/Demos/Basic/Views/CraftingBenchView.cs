using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GDS.Basic.Views;
using GDS.Core;
using GDS.Core.Views;
using UnityEngine.UIElements;
namespace GDS.Basic {
    public class CraftingBenchView : SmartComponent<CraftingBench> {

        public CraftingBenchView(CraftingBench bag) : base(bag) {
            slots = bag.Slots.Select(x => new BasicSlotView(x, bag)).ToArray();
            result = new BasicSlotView(bag.OutcomeSlot, bag);

            this.Add("window gap-v-20",
                Dom.Title("Crafting bench"),
                result.WithClass("crafting-bench__result-slot"),
                Dom.Div("slot-container", slots)
            ).WithWindowBehavior(bag.Id, Store.Instance.sideWindowId, Store.Bus);
        }

        BasicSlotView[] slots;
        BasicSlotView result;

        override public void Render(CraftingBench bag) {
            for (var i = 0; i < bag.Slots.Count; i++) slots[i].Data = bag.Slots[i];

            // TODO: move this to a dedicated inventory function - a view should only represent the state
            // Creating the result in the view prevents a debug view from seeing the result
            var input = bag.Slots.Select(slot => slot.Item.BaseId()).ToList();
            var recipe = new Recipe(input[0], input[1], input[2]);
            if (!DB.Recipes.ContainsKey(recipe)) {
                result.Data = result.Data.Clear();
                return;
            }

            var output = DB.Recipes.GetValueOrDefault(recipe);
            result.Data = result.Data with { Item = BasicItemFactory.Create(output, Rarity.Common) };
        }
    }
}
