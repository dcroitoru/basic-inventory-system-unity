using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace GDS {

    public static class Dom {
        public static VisualElement Div(string className, params VisualElement[] collection) {
            var element = new VisualElement();
            element.Div(className, collection);
            return element;
        }
        public static VisualElement Div() => new VisualElement();
        public static VisualElement Div(params VisualElement[] children) => new VisualElement().Div(children);
        public static Button Button(string className, string text, Action clickEvent) => new Button(clickEvent) { text = text }.WithClass(className) as Button;
        public static Button Button(string text, Action clickEvent) => new(clickEvent) { text = text };
        public static Label Label(string className, string text) => new Label(text).WithClass(className) as Label;
        public static Label Label(string text) => new(text);
        public static Label Title(string text) => new Label(text).WithClass("title") as Label;
    }

    public static class DomExt {

        public static VisualElement WithClass(this VisualElement element, string className) {
            if (className.Contains(" ")) {
                var classNames = className.Split(' ');
                for (var i = 0; i < classNames.Length; i++) element.AddToClassList(classNames[i]);
                return element;
            }

            element.AddToClassList(className);
            return element;
        }

        public static VisualElement WithoutClass(this VisualElement element, string className) {
            if (className.Contains(" ")) {
                var classNames = className.Split(' ');
                for (var i = 0; i < classNames.Length; i++) element.RemoveFromClassList(classNames[i]);
                return element;
            }

            element.RemoveFromClassList(className);
            return element;
        }

        public static VisualElement WithPickIgnore(this VisualElement element) {
            element.pickingMode = PickingMode.Ignore;
            return element;
        }

        public static VisualElement Div(this VisualElement element, params VisualElement[] collection) {
            foreach (var item in collection) element.Add(item);
            return element;
        }

        public static VisualElement Div(this VisualElement element, string className, params VisualElement[] collection) {
            foreach (var item in collection) element.Add(item);
            return element.WithClass(className);
        }

        public static VisualElement Hide(this VisualElement element) => element.WithClass("display-none");

        public static VisualElement Show(this VisualElement element) => element.WithoutClass("display-none");

        public static VisualElement SetVisible(this VisualElement element, bool visible) => visible ? element.WithoutClass("display-none") : element.WithClass("display-none");



        public static VisualElement WithoutPointerEvents(this VisualElement element, bool childrenOnly = false) {
            if (childrenOnly == false) element.pickingMode = PickingMode.Ignore;
            foreach (var child in element.Children()) child.WithoutPointerEvents();
            return element;
        }

        public static VisualElement SetSize(this VisualElement element, Size size, int scale = 1) {
            element.style.width = size.W * scale;
            element.style.height = size.H * scale;
            return element;
        }

        public static VisualElement Translate(this VisualElement element, Pos pos, int scale = 1) {
            element.style.translate = new Translate(pos.X * scale, pos.Y * scale);
            return element;
        }

        public static VisualElement Translate(this VisualElement element, Size pos, int scale = 1) {
            element.style.translate = new Translate(pos.W * scale, pos.H * scale);
            return element;
        }





    }
}
