using UnityEngine.UIElements;
using GDS.Core;

namespace GDS.Basic {
    public class EquipmentView : VisualElement {
        public EquipmentView(SetBag equipment) {
            var helmet = CreateSlotView(SlotType.Helmet, equipment);
            var weapon = CreateSlotView(SlotType.Weapon, equipment);
            var gloves = CreateSlotView(SlotType.Gloves, equipment);
            var boots = CreateSlotView(SlotType.Boots, equipment);
            var bodyArmor = CreateSlotView(SlotType.BodyArmor, equipment);

            this.Add("equipment slot-container",
                helmet.WithClass("equipment-slot helmet"),
                bodyArmor.WithClass("equipment-slot body-armor"),
                gloves.WithClass("equipment-slot gloves"),
                boots.WithClass("equipment-slot boots"),
                weapon.WithClass("equipment-slot weapon"));

            this.Observe(equipment.Data, (data) => {
                helmet.Data = equipment.GetSlot(SlotType.Helmet);
                weapon.Data = equipment.GetSlot(SlotType.Weapon);
                gloves.Data = equipment.GetSlot(SlotType.Gloves);
                boots.Data = equipment.GetSlot(SlotType.Boots);
                bodyArmor.Data = equipment.GetSlot(SlotType.BodyArmor);
            });
        }

        BasicSlotView CreateSlotView(SlotType slotType, SetBag bag) => new BasicSlotView(bag.GetSlot(slotType), bag);
    }
}
