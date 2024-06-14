using System;
using System.Collections.Generic;
using System.Linq;
using static GDS.Util;
using static GDS.Factory;
using UnityEditor;
using UnityEngine;

namespace GDS {
    public class BasicInventoryStore {
        public BasicInventoryStore() {

            Global.GlobalBus.Subscribe<ResetEvent>(e => OnReset(e as ResetEvent));
            Bus.Subscribe<ResetEvent>(e => OnReset(e as ResetEvent));
            Bus.Subscribe<AddItemEvent>(e => OnAddItem(e as AddItemEvent));
            Bus.Subscribe<PickItemEvent>(e => OnPickItem(e as PickItemEvent));
            Bus.Subscribe<PlaceItemEvent>(e => OnPlaceItem(e as PlaceItemEvent));
            Bus.Subscribe<CollectAllEvent>(e => OnCollectAll(e as CollectAllEvent));
            Bus.Subscribe<HotbarUseEvent>(e => OnHotbarUse(e as HotbarUseEvent));
            Bus.Subscribe<ToggleInventoryEvent>(e => OnToggleInventory(e as ToggleInventoryEvent));
            Bus.Subscribe<CloseInventoryEvent>(e => OnCloseInventory(e as CloseInventoryEvent));
            Bus.Subscribe<ToggleWindowEvent>(e => OnToggleWindow(e as ToggleWindowEvent));

            BagDictionary = new() {
                {equipment.Value.Id, equipment.Value},
                {inventory.Value.Id, inventory.Value},
                {hotbar.Value.Id, hotbar.Value},
                {chest1.Value.Id, chest1.Value},
                {vendor.Id, vendor},
            };
            Reset();
        }

        public readonly EventBus Bus = Global.BasicBus;
        public readonly Observable<Inventory> inventory = new(CreateInventory(40));
        public readonly Observable<Equipment> equipment = new(CreateEquipment(Data.BasicEquipmentSlots));
        public readonly Observable<Hotbar> hotbar = new(CreateHotbar());
        public readonly Observable<Chest> chest1 = new(CreateChest("chest1"));
        public readonly Observable<Chest> chest2 = new(CreateChest("chest2"));
        public readonly Observable<Item> draggedItem = new(NoItem);
        public readonly Observable<bool> isInventoryOpen = new(true);
        public readonly Observable<string> sideWindowId = new("");
        public readonly Vendor vendor = CreateVendor("vendor1", CreateAllItems());
        public readonly Dictionary<string, Bag> BagDictionary;

        Bag findBag(string id) => BagDictionary.GetValueOrDefault(id) ?? Factory.NoBag;

        void publishUpdate(Bag Bag) {
            if (Bag == equipment.Value) equipment.Next((Equipment)Bag);
            if (Bag == inventory.Value) inventory.Next((Inventory)Bag);
            if (Bag == hotbar.Value) hotbar.Next((Hotbar)Bag);
            if (Bag == chest1.Value) chest1.Next((Chest)Bag);
            if (Bag == chest2.Value) chest2.Next((Chest)Bag);
        }

        void Reset() {
            var state = Data.InitialState;
            equipment.Value.SetState(state.equipment);
            equipment.Next(equipment.Value);

            inventory.Value.SetState(state.inventory);
            inventory.Next(inventory.Value);

            hotbar.Value.SetState(state.hotbar);
            hotbar.Next(hotbar.Value);

            chest1.Value.SetState(state.chests[0].items);
            chest1.Next(chest1.Value);

        }

        void OnReset(ResetEvent e) {
            Log("OnReset".yellow());
            Reset();
        }

        void OnAddItem(AddItemEvent e) {
            var pos = inventory.Value.AddItems(e.item);
            if (pos == null) return;
            inventory.Next(inventory.Value);
        }

        void OnPickItem(PickItemEvent e) {
            Log("OnPickItem".yellow(), e);
            var Bag = findBag(e.Bag.Id);
            if (Bag == vendor) {
                var newItem = CreateItem(e.Item.Type);
                draggedItem.Next(newItem);
                return;
            }
            var (success, replacedItem) = Bag.PickItem(e.Item);
            if (!success) return;
            draggedItem.Next(replacedItem);
            publishUpdate(Bag);
        }

        void OnPlaceItem(PlaceItemEvent e) {
            Log("OnPlaceItem".yellow(), e);
            var Bag = findBag(e.Bag.Id);
            var (didPlace, replacedItem) = Bag.PlaceItem(e.Item, e.Slot);
            if (!didPlace) return;
            draggedItem.Next(replacedItem);
            publishUpdate(Bag);
        }

        void OnCollectAll(CollectAllEvent e) {
            Log("onCollectAll".yellow(), e);
            var Bag = findBag(e.Bag.Id);
            var chest = (Chest)Bag;
            var didNotFit = Bag switch {
                Chest => inventory.Value.AddItems(e.Items),
                _ => e.Items
            };

            chest.SetState(didNotFit.ToList());

            publishUpdate(inventory.Value);
            publishUpdate(chest);


        }

        void OnHotbarUse(HotbarUseEvent e) {
            Log("onHotbarUse".yellow(), e);
            var index = e.indexPlusOne - 1;
            var (didConsume, item) = hotbar.Value.Consume(index);
            if (!didConsume) return;

            hotbar.Next(hotbar.Value);
            Global.GlobalBus.Publish(new MessageEvent($"Consumed one <color=#e45490>[{item.Type}]"));
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