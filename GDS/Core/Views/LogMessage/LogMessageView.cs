using UnityEngine.UIElements;
using GDS.Core.Events;

namespace GDS.Core.Views {

    /// <summary>
    /// Displays a message queue in the bottom left corner
    /// Removes message after a delay (3 seconds)
    /// Hides itself when queue is empty
    /// </summary>
    public class LogMessageView : VisualElement {

        public LogMessageView(EventBus bus) {
            this.WithClass("message-container").Hide();
            this.SubscribeTo<ActionFail>(bus, e => AddMessage(CreateFailMessage((ActionFail)e)));
            this.SubscribeTo<CraftItemSuccess>(bus, e => AddMessage("Crafted ".Green() + ((CraftItemSuccess)e).Item.Name()));
            this.SubscribeTo<DestroyItemSuccess>(bus, e => AddMessage("Destroyed ".Red() + ((DestroyItemSuccess)e).Item.Name() + "!"));
            this.SubscribeTo<ConsumeItemSuccess>(bus, e => AddMessage("Consumed ".Green() + ((ConsumeItemSuccess)e).Item.Name()));
            this.SubscribeTo<BuyItemSuccess>(bus, e => AddMessage("Bought ".Blue() + ((BuyItemSuccess)e).Item.Name()));
            this.SubscribeTo<SellItemSuccess>(bus, e => AddMessage("Sold ".Blue() + ((SellItemSuccess)e).Item.Name()));
        }

        string CreateFailMessage(ActionFail e) => e.Severity switch {
            Severity.Info => e.Reason.Blue(),
            Severity.Warning => e.Reason.Yellow(),
            Severity.Error => e.Reason.Red(),
            _ => e.Reason
        };

        void AddMessage(string message) {
            this.Show();
            var label = Dom.Label("message", message);
            Add(label);
            schedule.Execute(() => {
                Remove(label);
                if (childCount == 0) this.Hide();
            }).ExecuteLater(3000);
        }
    }
}
