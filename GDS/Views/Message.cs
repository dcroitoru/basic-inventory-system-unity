using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static GDS.Factory;
using static GDS.Dom;
using System.Threading.Tasks;
namespace GDS {

    public class MessageView : VisualElement {

        public MessageView() {

            this.Div("message");

            Global.GlobalBus.Subscribe<MessageEvent>(e => AddMessage((e as MessageEvent).message));


        }

        // Label message;

        async void AddMessage(string message) {
            var label = Label("", message);
            Add(label);
            await RemoveElement(label);


        }

        private async Task RemoveElement(VisualElement element) {
            await Task.Delay(3000); // 3 seconds
            Remove(element);
        }




    }
}
