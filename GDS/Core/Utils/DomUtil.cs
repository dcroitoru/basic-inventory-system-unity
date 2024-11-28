using System;
using UnityEngine;
using UnityEngine.UIElements;
namespace GDS.Core {
    /// <summary>
    /// Contains factory methods for creating various visual elements
    /// The terms Dom and Div come from the web
    /// <see cref="https://developer.mozilla.org/en-US/docs/Web/API/Document_Object_Model/Introduction"/>
    /// </summary>
    public static class Dom {
        public static VisualElement Div() => new VisualElement();
        public static VisualElement Div(params VisualElement[] children) => Div().Add(children);
        public static VisualElement Div(string className, params VisualElement[] children) => Div().Add(className, children);

        public static Button Button(string className, string text, Action clickEvent) => new Button(clickEvent) { text = text }.WithClass(className);
        public static Button Button(string text, Action clickEvent) => new(clickEvent) { text = text };
        public static Label Label(string className, string text) => new Label(text).WithClass(className);
        public static Label Label(string text) => new Label(text);
        public static Label Title(string text) => new Label(text).WithClass("title");

    }

    /// <summary>
    /// Contains extension methods for working with visual elements
    /// </summary>
    public static class DomExt {

        public static VisualElement Add(this VisualElement element, params VisualElement[] collection) {
            foreach (var item in collection) element.Add(item);
            return element;
        }

        public static VisualElement Add(this VisualElement element, string className, params VisualElement[] collection) {
            foreach (var item in collection) element.Add(item);
            return element.WithClass(className);
        }

        public static VisualElement Div(this VisualElement element, params VisualElement[] collection) {
            foreach (var item in collection) element.Add(item);
            return element;
        }

        public static VisualElement Div(this VisualElement element, string className, params VisualElement[] collection) {
            foreach (var item in collection) element.Add(item);
            return element.WithClass(className);
        }

        public static T WithEvents<T>(
            this T element,
            EventCallback<AttachToPanelEvent> onAttachCallback,
            EventCallback<AttachToPanelEvent> onDetachCallback
        ) where T : VisualElement {
            element.RegisterCallback(onAttachCallback);
            element.RegisterCallback(onDetachCallback);
            return element;
        }

        /// <summary>
        /// Adds a class or a list of classes (if separated by ' ') to the element
        /// </summary>
        /// <returns></returns>
        public static T WithClass<T>(this T element, string className) where T : VisualElement {
            if (className.Contains(" ")) {
                var classNames = className.Split(' ');
                for (var i = 0; i < classNames.Length; i++) element.AddToClassList(classNames[i]);
                return element;
            }

            element.AddToClassList(className);
            return element;
        }
        /// <summary>
        /// Removes a class or a list of classes (if separated by ' ') from the element
        /// </summary>
        public static T WithoutClass<T>(this T element, string className) where T : VisualElement {
            if (className.Contains(" ")) {
                var classNames = className.Split(' ');
                for (var i = 0; i < classNames.Length; i++) element.RemoveFromClassList(classNames[i]);
                return element;
            }

            element.RemoveFromClassList(className);
            return element;
        }
        /// <summary>
        /// Hides an element by appending a 'display-none' class. Requires said class be present in the stylesheet
        /// </summary>
        public static VisualElement Hide(this VisualElement element) => element.WithClass("display-none");

        /// <summary>
        /// Shows an element by removing the 'display-none' class.
        /// </summary>
        public static VisualElement Show(this VisualElement element) => element.WithoutClass("display-none");
        /// <summary>
        /// Toggles element visibility
        /// </summary>
        public static VisualElement SetVisible(this VisualElement element, bool visible) => visible ? element.Show() : element.Hide();


        public static VisualElement WithoutPointerEvents(this VisualElement element) {
            element.pickingMode = PickingMode.Ignore;
            return element;
        }

        public static VisualElement WithoutPointerEventsInChildren(this VisualElement element) {
            foreach (var child in element.Children()) child.WithoutPointerEvents();
            return element;
        }

        public static VisualElement WithoutPointerEventsInAll(this VisualElement element) {
            return element.WithoutPointerEvents().WithoutPointerEventsInChildren();
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

        // Q: Why is there a need to subscribe to attached to panel event anyway?
        // A: If the effect runs before component is attached there will be null referencing
        // TODO: Comment the shit out of this mind bending function
        public static TElement WithEffect<TElement, TParam1>(
            this TElement element,
            Observable<TParam1> obs,
            Func<TElement, Action<TParam1>> callback
        ) where TElement : VisualElement {
            Debug.Log($"attaching effects {element.name}");
            callback(element)(obs.Value);
            return element.WithEvents((_) => obs.OnChange += callback(element), (_) => obs.OnChange -= callback(element));
        }

        // public static TElement WithEffect<TElement, TParam1>(
        //     this TElement element,
        //     Observable<TParam1> obs,
        //     Func<TElement, Action<TParam1>> callback
        // ) where TElement : VisualElement {
        //     callback(element)(obs.Value);
        //     return element.WithEvents((_) => obs.OnChange += callback(element), (_) => obs.OnChange -= callback(element));
        // }






    }

    public static class DomExtensions {

        public static VisualElement PseudoFirstChild(this VisualElement element, string className = "first-child") {
            element.RegisterCallback((GeometryChangedEvent e) => {
                if (element.childCount == 0) return;
                foreach (var child in element.Children()) child.RemoveFromClassList(className);
                element[0].AddToClassList(className);
            });
            return element;
        }

        public static VisualElement PseudoLastChild(this VisualElement element, string className = "last-child") {
            element.RegisterCallback((GeometryChangedEvent e) => {
                if (element.childCount == 0) return;
                foreach (var child in element.Children()) child.RemoveFromClassList(className);
                element[element.childCount - 1].AddToClassList(className);
            });
            return element;
        }

        public static VisualElement PseudoEvenChild(this VisualElement element, string className = "even-child") {
            element.RegisterCallback((GeometryChangedEvent e) => {
                if (element.childCount == 0) return;
                for (int i = 0; i < element.childCount; i++)
                    if (i % 2 == 0) element[i].AddToClassList(className);
                    else element[i].RemoveFromClassList(className);
            });
            return element;
        }

        public static VisualElement PseudoOddChild(this VisualElement element, string className = "odd-child") {
            element.RegisterCallback((GeometryChangedEvent e) => {
                if (element.childCount == 0) return;
                for (int i = 0; i < element.childCount; i++)
                    if (i % 2 != 0) element[i].AddToClassList(className);
                    else element[i].RemoveFromClassList(className);
            });
            return element;
        }

        public static VisualElement Gap(this VisualElement element, int gap) {
            element.RegisterCallback((GeometryChangedEvent e) => {
                if (element.childCount == 0) return;
                var direction = element.resolvedStyle.flexDirection;
                for (int i = 0; i < element.childCount - 1; i++)
                    switch (direction) {
                        case FlexDirection.Column: element[i].style.marginBottom = gap; break;
                        case FlexDirection.ColumnReverse: element[i].style.marginTop = gap; break;
                        case FlexDirection.RowReverse: element[i].style.marginLeft = gap; break;
                        default: element[i].style.marginRight = gap; break;
                    }
            });
            return element;
        }
    }
}
