using System;
using System.Collections.Generic;
using System.Linq;
using GDS.Core;
using GDS.Core.Events;
using UnityEngine;
using static GDS.Basic.Factory;

namespace GDS.Basic {
    public static class Extensions {
        public static ItemData ItemData(this Item item) => item.ItemData as ItemData;
        public static ItemBase ItemBase(this Item item) => item.ItemBase as ItemBase;
        public static Rarity Rarity(this Item item) => item.ItemData is ItemData itemData ? itemData.Rarity : Basic.Rarity.NoRarity;
        public static ItemClass Class(this Item item) => item.ItemBase is ItemBase itemBase ? itemBase.Class : ItemClass.NoItemClass;

        public static SetSlot GetSlot(this SetBag bag, SlotType slotType) {
            var slot = bag.Slots.GetValueOrDefault(slotType.ToString());
            if (slot != null) return slot;
            Debug.LogWarning("Could not find slot " + slotType);
            return (SetSlot)Slot.NoSlot;
        }

        public static bool ConsumeOneOfEachMaterials(this CraftingBench bag) {
            Debug.Log($"should consume one of each items {bag.Slots.CommaJoin()}");
            bag.Consume(0);
            bag.Consume(1);
            bag.Consume(2);
            bag.Data.Notify();
            return true;
        }

        /// <summary>
        /// Pick item
        /// </summary>
        public static (bool, CustomEvent) PickItem(Bag bag, Item item, Slot slot, Observable<Item> dragged) {
            var (success, replaced, @event) = (bag, slot) switch {
                (Vendor b, _) => b.PickItem(item),
                (CraftingBench b, CraftingOutcomeSlot) => b.PickItem(item),
                _ => bag.PickItem(item, slot)
            };
            if (success) dragged.SetValue(replaced);
            return (success, @event);
        }

        // Vendor
        static (bool, Item, CustomEvent) PickItem(this Vendor bag, Item item) {
            return (true, item.Clone(), CustomEvent.NoEvent);
        }

        // Crafting outcome
        static (bool, Item, CustomEvent) PickItem(this CraftingBench bag, Item item) {
            bag.ConsumeOneOfEachMaterials();
            return (true, item.Clone(), CreateMessageEvent("Crafted", item));
        }

        static (bool, Item, CustomEvent) PickItem(this Bag bag, Item item, Slot slot) {
            var (success, replaced) = Core.InventoryExtensions.PickItem(bag, item, slot);
            return (success, replaced, CustomEvent.NoEvent);
        }

        /// <summary>
        /// Move item
        /// </summary>
        public static (bool success, CustomEvent resultEvent) MoveItem(Bag bag, Bag toBag, Item item, Slot slot) {
            if (toBag.IsFull()) return (false, new MessageEvent("Inventory full".Yellow()));
            var (success, @event) = (bag, slot, toBag) switch {
                (Vendor b, _, Main m) => b.BuyItem(m, item),
                (CraftingBench b, CraftingOutcomeSlot s, Main m) => b.CraftItem(m, item, s),
                (Main m, _, Vendor) => m.SellItem(item),
                _ => bag.MoveItem(toBag, item)
            };
            return (success, @event);
        }

        public static (bool, CustomEvent) BuyItem(this Vendor bag, Bag toBag, Item item) {
            toBag.AddItems(item.Clone());
            return (true, CreateMessageEvent("Bought", item));
        }

        public static (bool, CustomEvent) CraftItem(this CraftingBench bag, Bag toBag, Item item, CraftingOutcomeSlot slot) {
            var craftSuccess = bag.ConsumeOneOfEachMaterials();
            if (!craftSuccess) return (false, CreateMessageEvent("Could not craft", item));
            toBag.AddItems(item.Clone());
            return (true, CreateMessageEvent("Crafted", item));
        }

        public static (bool, CustomEvent) SellItem(this Main bag, Item item) {
            bag.RemoveItem(item);
            return (true, CreateMessageEvent("Sold", item));
        }

        static (bool, CustomEvent) MoveItem(this Bag bag, Bag toBag, Item item) {
            var success = Core.InventoryExtensions.MoveItem(bag, toBag, item);
            return (success, CustomEvent.NoEvent);
        }

        // TODO: should this be Core?
        public static (bool success, CustomEvent resultEvent) UnstackItem(Bag bag, Item item, Slot slot, Observable<Item> dragged) {
            Debug.Log("should unstack".Yellow());
            if (!item.ItemBase.Stackable || item.ItemData.Quant <= 1) return (false, CustomEvent.NoEvent);
            var Quant = item.ItemData.Quant;
            var half = (int)Math.Floor(item.ItemData.Quant / 2f);
            var newDraggedItem = item.Clone(item.ItemData with { Quant = half });
            var newBagItem = item.Clone(item.ItemData with { Quant = Quant - half });
            dragged.SetValue(newDraggedItem);
            bag.SetItem(slot, newBagItem);
            return (false, CustomEvent.NoEvent);
        }

        // TODO: Should this be Core?
        public static (bool success, CustomEvent resultEvent) UnstackAndMoveItem(Bag bag, Bag toBag, Item item, Slot slot) {
            Debug.Log("Not implemented - should unstack then move".Yellow());
            return (false, CustomEvent.NoEvent);
        }

    }
}