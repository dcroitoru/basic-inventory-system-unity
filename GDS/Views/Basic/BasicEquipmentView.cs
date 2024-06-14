using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;
using static GDS.SlotType;
using static GDS.Dom;
namespace GDS {

    public class BasicEquipmentView : SmartComponent<Equipment> {

        public BasicEquipmentView(Observable<Equipment> equipment) : base(equipment) {
            this.equipment = equipment.Value;

            slots = equipment.Value.Slots.ToDictionary(kv => kv.Key, kv => createSlot(kv.Key));
            slotContainer = Div("slot-container", slots.Values.ToArray());
            this.Div("equipment", slotContainer);
        }

        Equipment equipment;
        VisualElement slotContainer;
        Dictionary<SlotType, SlotView> slots;
        Dictionary<SlotType, string> classNames = new() {
            {Helmet,"slot-helmet"},
            {BodyArmor,"slot-body-armor"},
            {Boots,"slot-boots"},
            {Weapon1, "slot-weapon"},
        };

        override public void Render(Equipment data) {
            // Util.Log("should render equipment slots".blue(), data.ToString().green());
            foreach (var slot in Data.Slots) { slots[slot.Key].Data = slot.Value; };
        }

        SlotView createSlot(SlotType slotType) =>
            new SlotView() {
                Bag = equipment,
                Data = equipment.Slots[slotType]
            }
            .WithClass("slot-equipment")
            .WithClass(classNames.GetValueOrDefault(slotType)) as SlotView;

    }
}
