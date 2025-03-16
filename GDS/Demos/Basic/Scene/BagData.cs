using System;
using System.Collections.Generic;

using System.Linq;
using GDS.Core;
using GDS.Core.Events;
using UnityEngine;



namespace GDS.Basic {
    public enum BagType { Chest, Stash, Shop }
    public class BagData : MonoBehaviour {
        [Tooltip("Must be unique")]
        [SerializeField] string Id;
        [SerializeField] BagType BagType = BagType.Chest;
        [SerializeField][Range(1, 80)] int MaxSize = 10;
        [Space(10)]
        [SerializeReference] List<ItemInfo> Items;
        public Bag Bag { get; private set; }

        void OnEnable() {
            Store.Bus.Subscribe<ResetEvent>(OnReset);
        }
        void OnDisable() {
            Store.Bus.Unsubscribe<ResetEvent>(OnReset);
        }


        void Awake() {
            if (Id == "") Id = "Interactible" + UnityEngine.Random.Range(0, 10000);
            Bag = CreateBag(BagType, Id, MaxSize);
            var state = Items.Select(item => Factory.CreateItem(Bases.Get(item.BaseId), item.Rarity, item.Quant));
            Bag.SetState(state.ToArray());
        }

        void OnReset(CustomEvent e) {
            var state = Items.Select(item => Factory.CreateItem(Bases.Get(item.BaseId), item.Rarity, item.Quant));
            Bag.SetState(state.ToArray());
        }

        Bag CreateBag(BagType bagType, string id, int size) => bagType switch {
            BagType.Chest => Factory.CreateChest(id, size),
            BagType.Stash => Factory.CreateStash(id, size),
            BagType.Shop => Factory.CreateShop(id, size),
            _ => Bag.NoBag
        };
    }
}
