using GDS.Basic.Views;
using GDS.Core;
using GDS.Core.Views;
using GDS.Sample;
namespace GDS.Basic {
    // public class BasicSlotView : SlotView<BasicItemView> { public BasicSlotView(Slot slot, Bag bag) : base(slot, bag) { } }
    public class BasicSetSlotView : SetSlotView<BasicItemView> { public BasicSetSlotView(SetSlot slot, Bag bag) : base(slot, bag) { } }

    public class EquipmentView : SmartComponent<SetBag> {

        public EquipmentView(SetBag equipment) : base(equipment) {

            helmet = new BasicSetSlotView(equipment.GetSlot(SlotType.Helmet), equipment);
            weapon = new BasicSetSlotView(equipment.GetSlot(SlotType.Weapon), equipment);
            gloves = new BasicSetSlotView(equipment.GetSlot(SlotType.Gloves), equipment);
            boots = new BasicSetSlotView(equipment.GetSlot(SlotType.Boots), equipment);
            bodyArmor = new BasicSetSlotView(equipment.GetSlot(SlotType.BodyArmor), equipment);

            this.Div("equipment slot-container",
                helmet.WithClass("equipment-slot helmet"),
                bodyArmor.WithClass("equipment-slot body-armor"),
                gloves.WithClass("equipment-slot gloves"),
                boots.WithClass("equipment-slot boots"),
                weapon.WithClass("equipment-slot weapon"));
        }

        BasicSetSlotView helmet;
        BasicSetSlotView bodyArmor;
        BasicSetSlotView gloves;
        BasicSetSlotView boots;
        BasicSetSlotView weapon;

        override public void Render(SetBag equipment) {
            helmet.Data = equipment.GetSlot(SlotType.Helmet);
            weapon.Data = equipment.GetSlot(SlotType.Weapon);
            gloves.Data = equipment.GetSlot(SlotType.Gloves);
            boots.Data = equipment.GetSlot(SlotType.Boots);
            bodyArmor.Data = equipment.GetSlot(SlotType.BodyArmor);
        }
    }
}
