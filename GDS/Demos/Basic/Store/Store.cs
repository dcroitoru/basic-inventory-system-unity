using System;
using System.Collections.Generic;
using System.Linq;
using static GDS.Util;
using static GDS.Factory;
using UnityEditor;
using UnityEngine;

namespace GDS.Demos.Basic {
    public class Store {
        public Store() {
            GDS.Global.GlobalBus.Subscribe<ResetEvent>(e => OnReset(e as ResetEvent));
            Bus.Subscribe<ResetEvent>(e => OnReset(e as ResetEvent));
            Bus.Subscribe<AddItemEvent>(e => OnAddItem(e as AddItemEvent));
            Bus.Subscribe<PickItemEvent>(e => OnPickItem(e as PickItemEvent));
            Bus.Subscribe<PlaceItemEvent>(e => OnPlaceItem(e as PlaceItemEvent));
            Bus.Subscribe<CollectAllEvent>(e => OnCollectAll(e as CollectAllEvent));
            Bus.Subscribe<HotbarUseEvent>(e => OnHotbarUse(e as HotbarUseEvent));
            Bus.Subscribe<ToggleInventoryEvent>(e => OnToggleInventory(e as ToggleInventoryEvent));
            Bus.Subscribe<CloseInventoryEvent>(e => OnCloseInventory(e as CloseInventoryEvent));
            Bus.Subscribe<ToggleWindowEvent>(e => OnToggleWindow(e as ToggleWindowEvent));

            Reset();
        }
        static SlotType[] EquipmentSlots = new SlotType[] { SlotType.Helmet, SlotType.BodyArmor, SlotType.Gloves, SlotType.Boots, SlotType.Weapon1 };

        public static readonly EventBus Bus = new();
        public static readonly Store Instance = new();
        public readonly Inventory inventory = CreateInventory(40);
        public readonly Equipment equipment = CreateEquipment(EquipmentSlots);
        public readonly Hotbar hotbar = CreateHotbar();
        public readonly Chest chest1 = CreateChest("chest1");
        public readonly Chest chest2 = CreateChest("chest2");
        public readonly Observable<Item> draggedItem = new(NoItem);
        public readonly Observable<bool> isInventoryOpen = new(true);
        public readonly Observable<string> sideWindowId = new("");
        public readonly Vendor vendor = CreateVendor("vendor1", CreateAllItems());

        void Reset() {
            var state = Data.InitialState;
            equipment.SetState(state.equipment).Next();
            inventory.SetState(state.inventory).Next();
            hotbar.SetState(state.hotbar).Next();
            chest1.SetState(state.chests[0].items).Next();
        }

        void OnReset(ResetEvent e) {
            Log("OnReset".yellow());
            Reset();
        }

        void OnAddItem(AddItemEvent e) {
            Log("OnAddItem".yellow(), e);
            var pos = inventory.AddItems(e.item);
            if (pos == null) return;
            inventory.Next();
        }

        void OnPickItem(PickItemEvent e) {
            Log("OnPickItem".yellow(), e);
            var Bag = e.Bag;
            if (Bag == vendor) {
                var newItem = CreateItem(e.Item.Type);
                draggedItem.Next(newItem);
                return;
            }
            var (success, replacedItem) = Bag.PickItem(e.Item);
            if (!success) return;
            draggedItem.Next(replacedItem);
            Bag.Next();
            Global.GlobalBus.Publish(new Invalidate());
        }

        void OnPlaceItem(PlaceItemEvent e) {
            Log("OnPlaceItem".yellow(), e);
            var Bag = e.Bag;
            var (didPlace, replacedItem) = Bag.PlaceItem(e.Item, e.Slot);
            if (!didPlace) return;
            draggedItem.Next(replacedItem);
            Bag.Next();
            Global.GlobalBus.Publish(new Invalidate());
        }

        void OnCollectAll(CollectAllEvent e) {
            Log("onCollectAll".yellow(), e);
            var Bag = e.Bag;
            var chest = (Chest)Bag;
            var didNotFit = Bag switch {
                Chest => inventory.AddItems(e.Items),
                _ => e.Items
            };

            chest.SetState(didNotFit.ToList());

            inventory.Next();
            chest.Next();

        }

        void OnHotbarUse(HotbarUseEvent e) {
            Log("onHotbarUse".yellow(), e);
            var index = e.indexPlusOne - 1;
            var (didConsume, item) = hotbar.Consume(index);
            if (!didConsume) return;

            hotbar.Next();
            Global.GlobalBus.Publish(new MessageEvent($"Consumed <color=#e45490>[{item.Type}]"));
            Global.GlobalBus.Publish(new Invalidate());

        }

        void OnToggleInventory(ToggleInventoryEvent e) {
            Log("onToggleInventory".yellow(), e);
            isInventoryOpen.Next(!isInventoryOpen.Value);
            if (isInventoryOpen.Value == true) return;
            sideWindowId.Next("");
        }

        void OnCloseInventory(CloseInventoryEvent e) {
            Log("onCloseInventory".yellow(), e);
            isInventoryOpen.Next(false);
            if (sideWindowId.Value == "") return;
            sideWindowId.Next("");

        }

        void OnToggleWindow(ToggleWindowEvent e) {
            Log("onToggleWindow".yellow(), e);
            var nextWindowId = sideWindowId.Value == e.id ? "" : e.id;
            sideWindowId.Next(nextWindowId);
            if (sideWindowId.Value == "" || isInventoryOpen.Value == true) return;
            isInventoryOpen.Next(true);

        }

    }
}