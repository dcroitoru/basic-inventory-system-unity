using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GDS {
    public static class Util {

        public static Func<Pos, Pos> Translate(Pos pos) => (Pos translateBy) => new Pos(pos.X + translateBy.X, pos.Y + translateBy.Y);

        public static int Area(this Size size) => size.W * size.H;

        public static string colorize(this string input, string pattern = @"[{}]") => Regex.Replace(input, pattern, match => $"{Yellow}{match.Value}{Reset}");

        public static void Log(IEnumerable<object> obj) => Debug.Log(obj.Count() == 0 ? $"[Collection is empty]" : string.Join("\n", obj).colorize());

        public static void Log(params object[] args) => Debug.Log(string.Join(" ", args).colorize());

        public static void LogWargning(params object[] args) => Debug.Log($"{Red}WARNING!{Reset}{string.Join(" ", args)}");

        public static string AsString<T>(this List<T> list) => string.Join("\n", list);

        public static string AsString<T, V>(this Dictionary<T, V> list) => string.Join("\n", list);

        public static string Print<T>(this IEnumerable<T> element) => string.Join(", ", element);

        public const string Reset = "</color>";
        public const string Red = "<color='red'>";
        public const string Yellow = "<color='yellow'>";
        public const string Green = "<color='green'>";
        public const string Pink = "<color='#e665df'>";
        public const string Orange = "<color='orange'>";
        public const string Blue = "<color='#3881d4'>";
        public const string Brown = "<color='brown'>";
        public const string Black = "<color='black'>";
        public static string pink(this string str) => $"{Pink}{str}{Reset}";
        public static string green(this string str) => $"{Green}{str}{Reset}";
        public static string orange(this string str) => $"{Orange}{str}{Reset}";
        public static string yellow(this string str) => $"{Yellow}{str}{Reset}";
        public static string blue(this string str) => $"{Blue}{str}{Reset}";
        public static string brown(this string str) => $"{Brown}{str}{Reset}";
        public static string black(this string str) => $"{Black}{str}{Reset}";


    }

}