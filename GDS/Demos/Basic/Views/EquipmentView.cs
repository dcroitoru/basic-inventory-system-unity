using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;
using static GDS.SlotType;
using static GDS.Dom;
namespace GDS.Demos.Basic {

    public class EquipmentView : SmartComponent<Equipment> {

        public EquipmentView(Equipment equipment) : base(equipment) {
            this.equipment = equipment;

            var keys = new List<SlotType> { Helmet, BodyArmor, Gloves, Boots, Weapon1 };
            slots = keys.ToDictionary(key => key, key => createSlot(key));

            this.Div("equipment slot-container",
                slots[Helmet].WithClass("equipment-slot helmet"),
                slots[BodyArmor].WithClass("equipment-slot body-armor"),
                slots[Gloves].WithClass("equipment-slot gloves"),
                slots[Boots].WithClass("equipment-slot boots"),
                slots[Weapon1].WithClass("equipment-slot weapon"));
        }

        Equipment equipment;
        Dictionary<SlotType, SlotView> slots;

        SlotView createSlot(SlotType slotType) => new SlotView() { Bag = equipment, Data = equipment.Slots[slotType] };
        void SetSlotViewData(SlotView slotView, Slot data) => slotView.Data = data;

        override public void Render(Equipment data) {
            foreach (var (key, value) in Data.Slots) SetSlotViewData(slots[key], value);
        }
    }
}
