using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using GDS.Core;
using static GDS.Core.Dom;
using static GDS.Core.DebugUtil;

namespace GDS.Basic {

    public class DebugWindow : EditorWindow {
        [MenuItem("Tools/Basic/Debug")]
        public static void Open() {
            var wnd = GetWindow<DebugWindow>();
            wnd.titleContent = new GUIContent("Basic::Debug");
        }

        public void CreateGUI() {
            var root = rootVisualElement;
            root.styleSheets.Add(Resources.Load<StyleSheet>("Basic/BasicTheme"));
            root.Add("debug-window",
                Div("gap-v-20",
                    DraggedItemDebug(Store.Instance.DraggedItem, CustomItemText),
                    new ScrollView().Add(
                        SetBagDebug(Store.Instance.Equipment, CustomSetSlotText),
                        ListBagDebug(Store.Instance.Main, CustomSlotText),
                        ListBagDebug(Store.Instance.Hotbar, CustomSlotText),
                        ListBagDebug(Store.Instance.Chest, CustomSlotText),
                        ListBagDebug(Store.Instance.EquipmentShop, CustomSlotText)
                    )
                )
            );
        }

        string CustomItemText(Item item) => $"{item.Name()} ({item.ItemData.Quant}) [id: {item.Id}, {item.Rarity()}]";
        string CustomSlotText(ListSlot slot) => $"{slot.Index}: {CustomItemText(slot.Item)}";
        string CustomSetSlotText(SetSlot slot) => $"{slot.Key}: {CustomItemText(slot.Item)}";





    }



}