using System;
using System.Collections.Generic;
using System.Reflection;

namespace graphicbox2d
{
    /// <summary>
    /// 構造体を文字列に変換したり、文字列から構造体を復元するためのユーティリティクラス。
    /// グラフィック2Dコントロールのデータ保存・読み込み処理に利用できる。
    /// </summary>
    internal static class StructConvert
    {

        /// <summary>
        /// 構造体を文字列に変換する。
        /// </summary>
        /// <param name="obj">構造体</param>
        /// <returns></returns>
        public static string ToString(object obj)
        {
            Type type = obj.GetType();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var parts = new List<string>();

            foreach (var f in fields)
            {
                var value = f.GetValue(obj);
                parts.Add($"{f.Name}={value}");
            }

            return "(" + string.Join(",", parts) + ")";
        }

        /// <summary>
        /// 文字列化した構造体を元に戻す。
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="targetType">構造体Type</param>
        /// <returns></returns>
        public static object FromString(string str, Type targetType)
        {
            str = str.Trim('(', ')');

            var dict = new Dictionary<string, string>();
            foreach (var pair in str.Split(','))
            {
                var kv = pair.Split('=');
                if (kv.Length == 2)
                    dict[kv[0].Trim()] = kv[1].Trim();
            }

            // 新しい構造体を生成
            object obj = Activator.CreateInstance(targetType);
            var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var f in fields)
            {
                if (dict.TryGetValue(f.Name, out string rawValue))
                {
                    object converted = Convert.ChangeType(rawValue, f.FieldType);
                    f.SetValue(obj, converted);
                }
            }

            return obj;
        }
    }
}