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
            bag.Consume(0);
            bag.Consume(1);
            bag.Consume(2);
            bag.Data.Notify();
            return true;
        }

        public static CustomEvent PickItem(Bag bag, Item item, Slot slot, Observable<Item> dragged) {
            var (replaced, @event) = (bag, slot) switch {
                (Vendor b, _) when b.Infinite => BuyAndCloneItem(b, item),
                (Vendor b, _) when !b.Infinite => BuyItem(b, item),
                (CraftingBench b, CraftingOutcomeSlot) => CraftItem(b, item),
                _ => PickItem(bag, item, slot)
            };
            dragged.SetValue(replaced);
            return @event;
        }

        public static (Item, CustomEvent) PlaceItem(Bag bag, Item item, Slot slot, Observable<Item> dragged) {
            var (success, replaced, ev) = (bag, slot) switch {
                // (Vendor b, _) => b.BuyItem(item),
                // (CraftingBench b, CraftingOutcomeSlot) => b.CraftItem(item),
                _ => PlaceItem(bag, item, slot)
            };
            if (success) dragged.SetValue(replaced);
            return (replaced, ev);
        }

        public static CustomEvent MoveItem(Bag bag, Bag toBag, Item item, Slot slot) {
            if (!toBag.Accepts(item)) return new ActionFail("Does not accept item", Severity.Warning);
            if (toBag.IsFull()) return new ActionFail("Inventory full", Severity.Warning);
            var ev = (bag, slot, toBag) switch {
                (Vendor b, _, Main m) when b.Infinite => b.BuyAndCloneItem(m, item),
                (Vendor b, _, Main m) when !b.Infinite => b.BuyAndMoveItem(m, item),
                (CraftingBench b, CraftingOutcomeSlot s, Main m) => CraftAndMoveItem(b, m, item, s),
                (Main m, _, Vendor) => SellItem(m, item),
                _ => bag.MoveItem(toBag, item)
            };
            return ev;
        }

        public static CustomEvent UnstackItem(Bag bag, Item item, Slot slot, Observable<Item> dragged) => bag.UnstackItem(item, slot, dragged) ? new PickItemSuccess(item) : CustomEvent.Fail;
        public static (Item, CustomEvent) UnstackDraggedItem(Bag bag, Slot slot, Observable<Item> dragged) => InventoryExtensions.UnstackDraggedItem(bag, slot, dragged) ? (Item.NoItem, new PlaceItemSuccess(dragged.Value)) : (Item.NoItem, CustomEvent.Fail);


        static (Item, CustomEvent) CraftItem(this CraftingBench bag, Item item) {
            bag.ConsumeOneOfEachMaterials();
            return (item.Clone(), new CraftItemSuccess(item));
        }

        static CustomEvent CraftAndMoveItem(this CraftingBench bag, Bag toBag, Item item, CraftingOutcomeSlot slot) {
            var craftSuccess = bag.ConsumeOneOfEachMaterials();
            if (!craftSuccess) return new ActionFail("Could not craft for some reason...");
            return toBag.AddItem(item.Clone()) ? new CraftItemSuccess(item) : CustomEvent.Fail;
        }

        static (Item, CustomEvent) PickItem(this Bag bag, Item item, Slot slot) {
            var (success, replaced) = Core.InventoryExtensions.PickItem(bag, item, slot);
            return success ? (replaced, new PickItemSuccess(item)) : (replaced, CustomEvent.Fail);
        }

        static (bool, Item, CustomEvent) PlaceItem(this Bag bag, Item item, Slot slot) {
            var (success, replaced) = Core.InventoryExtensions.PlaceItem(bag, item, slot);
            return success ? (success, replaced, new PlaceItemSuccess(item)) : (success, replaced, CustomEvent.Fail);
        }

        static (Item, CustomEvent) BuyAndCloneItem(this Vendor bag, Item item) => (item.Clone(), new BuyItemSuccess(item));
        static (Item, CustomEvent) BuyItem(this Vendor bag, Item item) => bag.RemoveItem(item) ? (item, new BuyItemSuccess(item)) : (item, CustomEvent.Fail);

        static CustomEvent BuyAndMoveItem(this Vendor bag, Bag toBag, Item item) => InventoryExtensions.MoveItem(bag, toBag, item) ? new BuyItemSuccess(item) : CustomEvent.Fail;
        static CustomEvent BuyAndCloneItem(this Vendor bag, Bag toBag, Item item) => toBag.AddItem(item.Clone()) ? new BuyItemSuccess(item) : CustomEvent.Fail;
        static CustomEvent SellItem(this Main bag, Item item) => bag.RemoveItem(item) ? new SellItemSuccess(item) : CustomEvent.Fail;
        static CustomEvent MoveItem(this Bag bag, Bag toBag, Item item) => InventoryExtensions.MoveItem(bag, toBag, item) ? new MoveItemSuccess(item) : CustomEvent.Fail;

        public static int Cost(this Item item) {
            var baseCost = item.ItemBase() switch {
                WeaponItemBase => 40,
                ArmorItemBase => 60,
                ItemBase b when b == Bases.Steel => 8,
                ItemBase b when b == Bases.Wood => 4,
                _ => 2
            };
            var rarityMult = item.ItemData().Rarity switch {
                Basic.Rarity.Magic => 1.5,
                Basic.Rarity.Rare => 2,
                Basic.Rarity.Unique => 3,
                _ => 1
            };


            return (int)(baseCost * rarityMult * item.ItemData().Quant);
        }

        public static int SellValue(this Item item) => item.Cost() / 2;
    }
}