using System;

namespace GDS.Core {

    public static class Colors {

        public const string Red = "red";
        public const string Yellow = "yellow";
        public const string Green = "green";
        public const string Orange = "orange";
        public const string Brown = "brown";
        public const string Black = "black";
        public const string Blue = "#3881d4";
        public const string Pink = "#e665df";
        public const string Gray = "#999";
        public const string DarkGray = "#555";
    }

    public static class ColorUtil {
        public static Func<string, string> ColorTag(string color) => (string text) => $"<color={color}>{text}</color>";
        public static string Red(this string str) => ColorTag(Colors.Red)(str);
        public static string Pink(this string str) => ColorTag(Colors.Pink)(str);
        public static string Green(this string str) => ColorTag(Colors.Green)(str);
        public static string Orange(this string str) => ColorTag(Colors.Orange)(str);
        public static string Yellow(this string str) => ColorTag(Colors.Yellow)(str);
        public static string Blue(this string str) => ColorTag(Colors.Blue)(str);
        public static string Brown(this string str) => ColorTag(Colors.Brown)(str);
        public static string Black(this string str) => ColorTag(Colors.Black)(str);
        public static string Gray(this string str) => ColorTag(Colors.Gray)(str);
        public static string DarkGray(this string str) => ColorTag(Colors.DarkGray)(str);
    }
}