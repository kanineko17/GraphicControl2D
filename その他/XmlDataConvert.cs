using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace graphicbox2d.その他
{
    /// <summary>
    /// Xmlデータ変換クラス
    /// </summary>
    static internal class XmlDataConvert
    {
        /// <summary>
        /// 配列データ区切り文字
        /// </summary>
        const char LIST_SPLIT_CHAR = ';';

        /// <summary>
        /// 配列データ開始文字
        /// </summary>
        const char LIST_START = '{';

        /// <summary>
        /// 配列データ終了文字
        /// </summary>
        const char LIST_END   = '}';

        /// <summary>
        /// 任意のデータをXML要素文字列に変換する。
        /// - string型はそのまま返す
        /// - IEnumerable型はリストとして展開して返す
        /// - その他は単一データとして処理する
        /// </summary>
        /// <param name="Data">変換対象のデータオブジェクト</param>
        /// <returns>XML要素文字列</returns>
        public static string DataToElementString(object Data)
        {
            if (Data == null)
            {
                return string.Empty;
            }

            // 文字列型の場合、そのまま返す
            if (typeof(String) == Data.GetType() || typeof(string) == Data.GetType())
            {
                return (string)Data;
            }

            // IEnumerable 型の場合、リストデータとして処理する
            if (Data is IEnumerable enumerable)
            {
                return ListDataToElementString((IEnumerable)Data);
            }

            // 単一データとして処理する
            return ItemDataToElementString(Data);
        }

        /// <summary>
        /// XML要素文字列を指定された型のデータに変換する。
        /// - IEnumerable型の場合はリストに復元
        /// - 単一データの場合は型に応じて復元
        /// </summary>
        /// <param name="ElementString">XML要素文字列</param>
        /// <param name="type">復元対象の型情報</param>
        /// <returns>復元されたオブジェクト</returns>
        public static object ElementStringToData(string ElementString, Type type)
        {
            object RetObject;
            Type elementType = GetEnumerableElementType(type);

            // IEnumerable 型の場合、リストデータとして処理する
            if (elementType != null)
            {
                RetObject = ListElementStringToListData(ElementString, type);
            }
            // 単一データとして処理する
            else
            {
                RetObject = ItemElementStringToData(ElementString, type);
            }
            return RetObject;
        }

        /// <summary>
        /// 単一データをXML要素文字列に変換する。
        /// Color, PointF, List＜PointF＞ は専用処理、それ以外は文字列化。
        /// </summary>
        /// <param name="Data">変換対象の単一データ</param>
        /// <returns>XML要素文字列</returns>
        private static string ItemDataToElementString(object Data)
        {
            Type targetType = Data.GetType();

            string RetString;

            switch (targetType)
            {
                // 構造体型
                case Type type when IsStruct(type) == true:
                    RetString = StructConvert.ToString(Data);
                    break;
                // Enum型
                case Type type when type.IsEnum == true:
                    RetString = Data.ToString();
                    break;
                // その他
                default:
                    // 既定の変換処理を実行
                    RetString = Convert.ToString(Data);
                    break;
            }
            return RetString;
        }

        /// <summary>
        /// IEnumerable型をXML要素文字列に変換する。
        /// 各要素をセミコロン区切りで連結し、波括弧で囲む。
        /// </summary>
        /// <param name="Data">変換対象の列挙可能データ</param>
        /// <returns>リスト形式のXML要素文字列</returns>
        private static string ListDataToElementString(IEnumerable Data)
        {
            string ListDataString = "";

            foreach (var item in Data)
            {
                ListDataString += ItemDataToElementString(item) + LIST_SPLIT_CHAR;
            }

            // 末尾のセミコロンを削除
            ListDataString = ListDataString.Substring(0, ListDataString.Length - 1);

            return string.Format("{0}{1}{2}", LIST_START, ListDataString, LIST_END);
        }

        /// <summary>
        /// XML要素文字列を単一データに変換する。
        /// Color, PointF は専用処理、それ以外は型変換。
        /// </summary>
        /// <param name="ElementString">XML要素文字列</param>
        /// <param name="type">復元対象の型情報</param>
        /// <returns>復元されたオブジェクト</returns>
        private static object ItemElementStringToData(string ElementString, Type type)
        {
            object RetObject;
            switch (type)
            {
                // Color型
                case Type t when IsStruct(t) == true:
                    RetObject = StructConvert.FromString(ElementString, type);
                    break;
                // Enum型
                case Type t when t.IsEnum == true:
                    RetObject = Enum.Parse(t, ElementString);
                    break;
                default:
                    RetObject = Convert.ChangeType(ElementString, type);
                    break;
            }

            return RetObject;
        }

        /// <summary>
        /// XML要素文字列をリストデータに変換する。
        /// 要素型を判定し、各要素を復元してリストに追加。
        /// </summary>
        /// <param name="ElementString">リスト形式のXML要素文字列</param>
        /// <param name="type">復元対象のリスト型情報</param>
        /// <returns>復元されたリストオブジェクト</returns>
        private static object ListElementStringToListData(string ElementString, Type type)
        {
            try
            {
                // リストオブジェクトを生成
                Type elementType = GetEnumerableElementType(type);
                Type listType    = typeof(List<>).MakeGenericType(elementType);
                var list         = (IList)Activator.CreateInstance(listType);

                string trimmed = ElementString.Trim(new char[] { LIST_START, LIST_END });
                string[] parts = trimmed.Split(LIST_SPLIT_CHAR);

                foreach (var part in parts)
                {
                    var element = ItemElementStringToData(part, elementType);
                    list.Add(element);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new FormatException("Invalid PointF format", ex);
            }
        }

        /// <summary>
        /// 指定された型が配列なら要素型を返す。
        /// List&lt;T&gt;など IEnumerable&lt;T&gt; の場合はジェネリック引数を返す。
        /// それ以外は null を返す。
        /// </summary>
        /// <param name="type">判定対象の型情報</param>
        /// <returns>要素型、または null</returns>
        private static Type GetEnumerableElementType(Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (type == typeof(string) || type == typeof(String))
            {
                return null;
            }

            // 配列の場合
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            // ジェネリック IEnumerable<T> の場合
            var enumerableInterface = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType &&
                                     i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (enumerableInterface != null)
            {
                return enumerableInterface.GetGenericArguments()[0];
            }

            return null;
        }

        /// <summary>
        /// 構造体であるか判定する。
        /// </summary>
        /// <param name="type">構造体Type</param>
        /// <returns>true:構造体 false:構造体でない</returns>
        static bool IsStruct(Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
        }
    }
}
