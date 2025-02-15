using System;
using System.Linq;
using UnityEngine.UIElements;
using static GDS.Core.Dom;

namespace GDS.Core {
    public static class DebugUtil {
        public static Func<string, string> ColorFn(Slot slot) => slot.IsEmpty() ? ColorUtil.Gray : x => x;
        public static Func<string, string> ColorFn(Item item) => item is NoItem ? ColorUtil.Gray : x => x;
        public static string SlotText(ListSlot slot) => $"{slot.Index}: {ItemText(slot.Item)}";
        public static string SetSlotText(SetSlot slot) => $"[{slot.Key}]: {ItemText(slot.Item)}";
        public static string ItemText(Item item) => $"{item.Name()} ({item.ItemData.Quant}) [id: {item.Id}]";

        public static VisualElement ListBagDebug(ListBag bag) => ListBagDebug(bag, SlotText);
        public static VisualElement ListBagDebug(ListBag bag, Func<ListSlot, string> SlotTextFn) {
            var el = Div();
            return el.Observe(bag.Data, (_) => {
                el.Clear();
                el.Add(
                    Title(bag.Id),
                    Div(bag.Slots
                        .Select(slot => ColorFn(slot)(SlotTextFn(slot)))
                        .Select(Label)
                        .ToArray())
                );
            });
        }

        public static VisualElement SetBagDebug(SetBag bag, Func<SetSlot, string> SetSlotText) {
            var el = Div();
            return el.Observe(bag.Data, (_) => {
                el.Clear();
                el.Add(
                    Title(bag.Id),
                    Div(bag.Slots.Values
                        .Select(slot => ColorFn(slot)(SetSlotText(slot)))
                        .Select(Label)
                        .ToArray())
                );
            });
        }

        public static VisualElement DraggedItemDebug(Observable<Item> dragged) => DraggedItemDebug(dragged, ItemText);
        public static VisualElement DraggedItemDebug(Observable<Item> dragged, Func<Item, string> ItemTextFn) {
            var el = Div();
            return el.Observe(dragged, item => {
                el.Clear();
                el.Div(
                    Title("Dragged item"),
                    Label(ColorFn(item)(ItemTextFn(item)))
                );
            });
        }
    }
}