using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using GDS.Basic.Views;
using GDS.Core;

namespace GDS.Basic {

    [CustomPropertyDrawer(typeof(ItemInfo))]
    public class MyItemDrawerDrawer : PropertyDrawer {

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {

            if (property.managedReferenceValue == null) {
                property.managedReferenceValue = new ItemInfo() { BaseId = Bases.Mushroom.Id };
                property.serializedObject.ApplyModifiedProperties();
            }

            var baseIdProp = property.FindPropertyRelative("BaseId");
            var rarityProp = property.FindPropertyRelative("Rarity");
            var quantProp = property.FindPropertyRelative("Quant");


            var itemView = new BasicItemView() { Data = Factory.CreateItem(Bases.Get(baseIdProp.stringValue), (Rarity)rarityProp.enumValueFlag) };
            var quantField = new IntegerField("Quantity", 3) { bindingPath = quantProp.propertyPath };
            quantField.RegisterValueChangedCallback(evt => {
                if (evt.newValue < 1) { quantField.value = 1; }
                itemView.Data = itemView.Data with { ItemData = new GDS.Basic.ItemData(itemView.Data.Rarity(), evt.newValue) };
            });

            var rarityField = new EnumField("Rarity") { bindingPath = rarityProp.propertyPath };
            rarityField.RegisterValueChangedCallback(evt => {
                if (evt.newValue == null) return;
                itemView.Data = itemView.Data with { ItemData = new GDS.Basic.ItemData((Rarity)evt.newValue, itemView.Data.Quant()) };
            });

            var baseField = new PopupField<string>("Base", Bases.AllIds.ToList(), 0) { bindingPath = baseIdProp.propertyPath };
            baseField.RegisterValueChangedCallback(evt => {
                if (evt.newValue == null) return;
                var itemBase = Bases.Get(evt.newValue);
                if (itemBase.Stackable) {
                    quantField.SetEnabled(true);
                } else {
                    quantField.SetEnabled(false);
                    quantField.value = 1;
                }

                if (itemBase.Class == ItemClass.Consumable || itemBase.Class == ItemClass.Material) {
                    rarityField.SetEnabled(false);
                    rarityField.value = Rarity.NoRarity;
                } else {
                    rarityField.SetEnabled(true);
                }

                itemView.Data = itemView.Data with { ItemBase = itemBase };

            });


            var container = Dom.Div("row",
                itemView,
                Dom.Div("flex-grow-1",
                    baseField,
                    rarityField,
                    quantField
                )
            );

            container.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/InventorySystem/GDS/Demos/Basic/Editor/ItemInfoDrawer.uss"));
            container.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/InventorySystem/GDS/Demos/Basic/Resources/Basic/BasicStyles.uss"));

            return container;
        }
    }
}