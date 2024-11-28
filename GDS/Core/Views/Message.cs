using UnityEngine.UIElements;
using System.Threading.Tasks;
using GDS.Core.Events;
namespace GDS.Core.Views {

    /// <summary>
    /// Displays a message queue in the bottom left corner
    /// Removes message after a delay (3 seconds)
    /// Removes itself if queue is empty
    /// </summary>
    public class MessageView : VisualElement {

        public MessageView() {
            this.WithClass("message-container").Hide();
            EventBus.GlobalBus.Subscribe<MessageEvent>(e => AddMessage((e as MessageEvent).Message));
        }

        async void AddMessage(string message) {
            this.Show();
            var label = Dom.Label("message", message);
            Add(label);
            await RemoveElement(label);
        }

        private async Task RemoveElement(VisualElement element) {
            // remove after 3 seconds
            await Task.Delay(3000);
            Remove(element);

            var count = this.Query<Label>().Build().ToList().Count;
            if (count == 0) this.Hide();
        }




    }
}
