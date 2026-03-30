using graphicbox2d.その他;
using SkiaSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace graphicbox2d
{
    /// <summary>
    /// 全図形の基底クラス
    /// </summary>
    public class Object2D : IComparable<Object2D>, IXmlSerializable, IDisposable
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public virtual eObject2DType m_Type => eObject2DType.None;

        /// <summary>
        /// 図形の表示フラグ
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// 選択フラグ
        /// true:選択中　false:未選択
        /// </summary>
        public bool IsSelect { get; set; } = false;

        /// <summary>
        /// 描画の順番
        /// 値が大きいほど手前に描画される
        /// </summary>
        public float ZOrder { get; set; } = 0f;

        /// <summary>
        /// Disposeフラグ
        /// </summary>
        public bool IsDisposed { get; set; } = false;

        // ===============================================================================
        // 非公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の中心点
        /// </summary>
        internal virtual Vector2 CenterPoint { get { return new Vector2(float.NaN, float.NaN); } }

        /// <summary>
        /// 図形の選択ポイント
        /// </summary>
        internal virtual PointF[] SnapPoints { get { return null; } }

        /// <summary>
        /// マウスヒット中の図形の線の太さの加算量
        /// マウスヒット中はこの値の分だけ線が太くなる
        /// </summary>
        internal virtual int MouseHitLineOffset { get; set; } = 2;

        /// <summary>
        /// マウスヒット中の図形（円やポリゴンなど）の大きさの倍率
        /// マウスヒット中はこの値の分だけ図形を拡大する
        /// </summary>
        internal virtual float MouseHitPolyOffset { get; set; } = 1.05f;

        /// <summary>
        /// スナップポイントを表示するかどうかのフラグ
        /// </summary>
        internal bool IsShowSnapPoints { get; set; } = false;

        /// <summary>
        /// 格オブジェクトのインデックス番号
        /// 比較の時に使用する
        /// </summary>
        internal int _ID;
        internal static int counter = 0;

        // ===============================================================================
        // 公開メソッド
        // ===============================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Object2D()
        {
            // 一意のIDを割り当てる
            _ID = System.Threading.Interlocked.Increment(ref counter);
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns></returns>
        public virtual Object2D Clone()
        {
            return new Object2D()
            {
                IsSelect = this.IsSelect,
                ZOrder = this.ZOrder,
                MouseHitLineOffset = this.MouseHitLineOffset,
                MouseHitPolyOffset = this.MouseHitPolyOffset
            };
        }

        /// <summary>
        /// オブジェクト2Dの比較関数
        /// Listでソートする際に使用される
        /// ZOrderの値で比較を行う
        /// </summary>
        /// <param name="other">比較対象</param>
        /// <returns>
        /// 0:等しい
        /// 0より小さい値: this が other より前  
        /// 0より大きい値: this が other より後  
        /// </returns>
        public int CompareTo(Object2D other)
        {
            // ZOrder の比較を行う
            int ret = this.ZOrder.CompareTo(other.ZOrder);

            if (ret != 0)
            {
                return ret;
            }

            // ZOrder が一致している場合、IDで比較を行う
            return this._ID.CompareTo(other._ID);
        }

        /// <summary>
        /// /Dispose
        /// </summary>
        public virtual void Dispose()
        {
            IsDisposed = true;
        }

        // ===============================================================================
        // 非公開メソッド
        // ===============================================================================

        /// <summary>
        /// コピー元のObject2Dデータをコピー先にコピーする。
        /// </summary>
        /// <param name="target">コピー先</param>
        protected void BaseCopyDataTo(Object2D target)
        {
            target.IsSelect = this.IsSelect;
            target.ZOrder = this.ZOrder;
            target.MouseHitLineOffset = this.MouseHitLineOffset;
            target.MouseHitPolyOffset = this.MouseHitPolyOffset;
        }

        /// <summary>
        /// マウスヒット中の図形（拡大した図形）を返す。
        /// </summary>
        /// <returns>拡大された図形</returns>
        internal virtual Object2D GetHitObject()
        {
            return null;
        }

        /// <summary>
        /// マウスポイントがこの図形にヒットしているかどうかを判定する。
        /// </summary>
        /// <param name="MousePoint">マウス座標</param>
        /// <param name="MusePointRange">ヒット判定の許容範囲（半径などに利用）</param>
        /// <returns>ヒットしていれば true、そうでなければ false</returns>
        internal virtual eMouseHitType IsHitMousePoint(PointF MousePoint, float MusePointRange)
        {
            // 基底クラスでは常に eMouseHitType.None を返す。
            // 派生クラスで図形ごとの判定ロジックを実装する。
            return eMouseHitType.None;
        }

        /// <summary>
        /// マウスポイントとこの図形との距離を返す。
        /// </summary>
        /// <param name="X">マウス座標の X 値</param>
        /// <param name="Y">マウス座標の Y 値</param>
        internal virtual float GetDistanceHitMousePoint(float X, float Y)
        {
            // 基底クラスでは未実装 → NaN を返す。
            return float.NaN;
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
        /// <param name="Movement">移動量</param>
        internal virtual void Move(PointF Movement)
        {
            // 基底クラスでは何もしない。
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
        /// <param name="X">移動量X</param>
        /// <param name="Y">移動量Y</param>
        internal virtual void Move(float X, float Y)
        {
            // 基底クラスでは何もしない。
        }

        /// <summary>
        /// バウンディングボックスを取得する
        /// </summary>
        internal virtual PointF[] GetBoundingBox()
        {
            // 基底クラスでは何もしない。

            return null;
        }

        // ============================================================
        // IXmlSerializable 実装
        // ============================================================

        /// <summary>
        /// XML スキーマを取得する。
        /// </summary>
        /// <returns>null</returns>
        public XmlSchema GetSchema() => null;

        /// <summary>
        /// WriteXml実装
        /// XMLのデータ書き出しの方法を定義する
        /// </summary>
        /// <param name="writer">Writer</param>
        public void WriteXml(XmlWriter writer)
        {
            // 全ての公開プロパティを取得してループ
            foreach (var prop in this.GetType().GetProperties())
            {
                if(prop.CanRead == false)
                {
                    continue;
                }

                // プロパティの値を取得
                var value = prop.GetValue(this);

                if (value == null)
                {
                    // 値が null の場合はスキップ
                    continue;
                }

                // プロパティの値を文字列に変換
                string ElementString =  XmlDataConvert.DataToElementString(value);

                
                // XML に要素を書き出し
                writer.WriteElementString(prop.Name, ElementString);
            }
        }

        /// <summary>
        /// ReadXml実装
        /// データの読み込み方法を定義する
        /// </summary>
        /// <param name="reader">Reader</param>
        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            while (reader.NodeType == XmlNodeType.Element)
            {
                string name = reader.Name;
                string value = reader.ReadElementContentAsString();

                if (string.IsNullOrEmpty(value) == true)
                {
                    // 値が null の場合はスキップ
                    continue;
                }

                var prop = this.GetType().GetProperty(name);

                if (prop == null || prop.CanWrite == false)
                {
                    // プロパティが存在しない、または書き込み不可の場合はスキップ
                    continue;
                }

                // 文字列をデータに変換
                object converted = XmlDataConvert.ElementStringToData(value, prop.PropertyType);

                // プロパティに値を設定
                prop.SetValue(this, converted);
            }

            reader.ReadEndElement();
        }

        /// <summary>
        /// ドキュメントデータを取り込む
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        public virtual void ImportDocument(in Object2D_Document doc)
        {
            // ドキュメントのプロパティをこのオブジェクトにコピーする
            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.CanWrite == false)
                {
                    continue;
                }
                // パブリックメンバー出ない場合はスキップ
                if (prop.SetMethod.IsPublic == false)
                {
                    continue;
                }
                // ドキュメントからプロパティの値を取得
                var value = doc.GetType().GetProperty(prop.Name)?.GetValue(doc);
                if (value == null)
                {
                    // 値が null の場合はスキップ
                    continue;
                }
                // プロパティの値をこのオブジェクトに設定
                prop.SetValue(this, value);
            }
        }

        /// <summary>
        /// ドキュメントデータを書き出す
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        public virtual void OutDocument(out Object2D_Document doc)
        {
            switch (this.m_Type)
            {
                case eObject2DType.None:
                    doc = new Object2D_Document();
                    break;
                case eObject2DType.Point:
                    doc = new Point2D_Document();
                    break;
                case eObject2DType.Line:
                    doc = new Line2D_Document();
                    break;
                case eObject2DType.Circle:
                    doc = new Circle2D_Document();
                    break;
                case eObject2DType.Polygon:
                    doc = new Polygon2D_Document();
                    break;
                case eObject2DType.Arrow:
                    doc = new Arrow2D_Document();
                    break;
                case eObject2DType.Text:
                    doc = new Text2D_Document();
                    break;
                case eObject2DType.Arc:
                    doc = new Arc2D_Document();
                    break;
                case eObject2DType.Graph:
                    doc = new Graph2D_Document();
                    break;
                case eObject2DType.MathGraph:
                    doc = new MathGraph2D_Document();
                    break;
                case eObject2DType.Group:
                    doc = new Group2D_Document();
                    break;
                case eObject2DType.Image:
                    doc = new Image2D_Document();
                    break;
                default:
                    doc = null;
                    break;
            }

            // プロパティの値をドキュメントに設定
            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.CanRead == false)
                {
                    continue;
                }

                // パブリックメンバー出ない場合はスキップ
                if (prop.GetMethod.IsPublic == false)
                {
                    continue;
                }

                // プロパティの値を取得
                var value = prop.GetValue(this);

                if (value == null)
                {
                    // 値が null の場合はスキップ
                    continue;
                }

                var targetProp = doc.GetType().GetProperty(prop.Name);
                if (targetProp == null) continue;

                // setter が無いならスキップ
                if (targetProp.SetMethod == null || targetProp.SetMethod.IsPublic == false)
                {
                    continue;
                }

                // 型が一致しないならスキップ
                if (!targetProp.PropertyType.IsAssignableFrom(prop.PropertyType))
                {
                    continue;
                }

                // プロパティの値をドキュメントに設定
                doc.GetType().GetProperty(prop.Name).SetValue(doc, value);
            }
        }
    }
}
