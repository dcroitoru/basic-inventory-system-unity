using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using GDS.Core;
using GDS.Basic.Views;
using UnityEngine;

namespace GDS.Basic {
    public class CraftingBenchView : VisualElement {

        public CraftingBenchView(CraftingBench bag) {
            var Slots = bag.Slots.Select(x => new BasicSlotView(x, bag)).ToArray();
            var Result = new BasicSlotView(bag.OutcomeSlot.Value, bag);

            this.Add("window gap-v-20",
                Components.CloseButton(bag),
                Dom.Title("Crafting bench"),
                Result.WithClass("crafting-bench__result-slot"),
                Dom.Div("slot-container", Slots),
                Hints()
            );

            this.Observe(bag.Data, (p) => {

                for (var i = 0; i < bag.Slots.Count; i++) Slots[i].Data = bag.Slots[i];
                // TODO: move this to a dedicated inventory function - a view should only represent the state
                // Creating the result in the view prevents a debug view from seeing the result
                var input = bag.Slots.Select(slot => slot.Item.ItemBase()).ToList();
                var recipe = new Recipe(input[0], input[1], input[2]);
                // Debug.Log($"recipe\n {recipe.one}\n {recipe.two}\n {recipe.three}");
                if (!Recipes.All.ContainsKey(recipe)) {
                    Result.Data = Result.Data.Clear();
                    return;
                }

                var output = Recipes.All.GetValueOrDefault(recipe);
                Result.Data = Result.Data with { Item = Factory.CreateItem(output, Rarity.Common) };
            });
        }

        VisualElement Hints() {
            var recipes = Recipes.All.Keys.ToArray();
            System.Func<Recipe, string> hintStr = (Recipe r) => r.Item1?.Name + " - " + r.Item2?.Name + " - " + r.Item3?.Name;
            var el = Dom.Div(Dom.Label("Try:"));
            el.Add(Dom.Div("hints", recipes.Select(r => Dom.Label(hintStr(r))).ToArray()));
            return el;
        }
    }
}
