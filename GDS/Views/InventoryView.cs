using UnityEngine;
using UnityEngine.UIElements;

namespace GDS
{

    public class InventoryView : VisualElement
    {

        VisualElement textContainer = new() { name = "textcontainer" };
        VisualElement slotContainer = new() { name = "slotContainer" };

        public InventoryView()
        {
            var stylesheet = Resources.Load<StyleSheet>("Inventory");
            styleSheets.Add(stylesheet);

            // setup
            var l = new Label("Inventory");
            l.AddToClassList("header");
            Add(l);

            // Add(textContainer);
            Add(slotContainer);

            slotContainer.AddToClassList("slot-container");
        }

        // public VisualElement Init(EventBus bus, Inventory initialState) {

        //     Debug.Log(Col.blue("inventory init"));
        //     bus.Subscribe<InventoryChanged>(e => {
        //         var state = (e as InventoryChanged).state;
        //         if (state.id == initialState.id) Render(state as Inventory);
        //     });
        //     // RegisterCallback<DetachFromPanelEvent>(e => bus.Unsubscribe<InventoryChangedEvent>(e => Render((e as InventoryChangedEvent).state)));
        //     Render(initialState);
        //     return this;
        // }

        // private void Render(Inventory state) {

        //     // Debug.Log($"should render inventory: {state}");
        //     textContainer.Clear();
        //     state.slots.ForEach((slot) => textContainer.Add(createItemRenderer(slot)));


        //     slotContainer.Clear();
        //     state.slots.ForEach(slot => {
        //         var s = new SlotView();
        //         s.RegisterCallback<ClickEvent>((e) => OnSlotCick(s));
        //         s.Init(slot);
        //         slotContainer.Add(s);
        //     });
        // }

        // VisualElement createItemRenderer(Slot slot) {
        //     var r = new VisualElement().WithClass("slot-text-renderer");
        //     var str = $"{slot.id}: {slot.data.type} ({slot.data.quantity})";
        //     var lbl = new Label(str);
        //     lbl.style.flexGrow = 1;
        //     r.Add(lbl);
        //     r.Add(new Button(() => OnDelete(slot)) { text = "Del" });
        //     r.Add(new Button(() => OnUse(slot)) { text = "Use" });

        //     return r;
        // }

        // void OnDelete(Slot slot) {
        //     GlobalBus.Bus.Publish(new InventoryRemoveEvent(slot));
        // }
        // void OnUse(Slot slot) {
        //     GlobalBus.Bus.Publish(new InventoryUseEvent(slot));
        // }

        // void OnSlotCick(SlotView slot) {

        //     // Debug.Log(Col.red($"{slot}"));

        //     // if (selected != null) {

        //     //     var from = selected.slotData;
        //     //     var to = slot.slotData;
        //     //     GlobalBus.Bus.Publish(new InventorySwapEvent(from, to));

        //     //     selected.RemoveFromClassList("selected");
        //     //     selected = null;
        //     //     return;
        //     // }

        //     // selected = slot;
        //     // selected.AddToClassList("selected");
        // }
    }
}
