using GDS.Core.Views;

namespace GDS.Core {
    public static class DomFactory {
        public static BaseSlotView BaseSlotView(Slot slot, Bag bag) => new BaseSlotView(slot, bag);
        public static SlotView<ItemView> SlotView(Slot slot, Bag bag) => new SlotView<ItemView>(slot, bag);

        public static ListBagView<SlotView<ItemView>> ListBagView(ListBag bag) => new ListBagView<SlotView<ItemView>>(bag);
        public static ListBagView<TSlotView> ListBagView<TSlotView>(ListBag bag) where TSlotView : BaseSlotView => new ListBagView<TSlotView>(bag);

    }
}