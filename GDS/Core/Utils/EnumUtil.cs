using UnityEngine;

namespace GDS.Core {

    public static class EnumUtil {
        public static T GetRandomEnumValue<T>() where T : System.Enum {
            var values = System.Enum.GetValues(typeof(T));
            return (T)values.GetValue(Random.Range(0, values.Length))!;
        }

        public static T[] GetAllEnumValues<T>() where T : System.Enum => (T[])System.Enum.GetValues(typeof(T));
    }

}