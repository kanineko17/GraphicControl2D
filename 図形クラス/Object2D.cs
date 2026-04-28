using graphicbox2d.その他;
using graphicbox2d.描画図形クラス;
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
    public class Object2D : IComparable<Object2D>, IDisposable
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public virtual eObject2DType m_Type => eObject2DType.None;

        /// <summary>
        /// 図形の名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

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
        internal virtual List<PointF> SnapPoints { get { return null; } }

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
        /// グラフィック２Dコントロール内部で使用するためのオブジェクト名
        /// </summary>
        internal string SystemName { get; set; } = string.Empty;

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
                IsVisible = this.IsVisible,
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
            target.IsVisible = this.IsVisible;
            target.IsSelect = this.IsSelect;
            target.ZOrder = this.ZOrder;
            target.MouseHitLineOffset = this.MouseHitLineOffset;
            target.MouseHitPolyOffset = this.MouseHitPolyOffset;
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

        /// <summary>
        /// 描画に必要な情報をまとめたクラスを返す。
        /// </summary>
        /// <param name="type">描画タイプ</param>
        /// <returns>描画用のデータをまとめたクラス</returns>
        internal virtual object GetDrawFigure(eDrawFigureType type)
        {
            return null;
        }


        /// <summary>
        /// ドキュメントデータを取り込む
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        public virtual void ImportDocument(in Object2D_Document doc)
        {
            IsVisible = doc.IsVisible;
            IsSelect = doc.IsSelect;
            ZOrder = doc.ZOrder;
        }

        /// <summary>
        /// ドキュメントデータを書き出す
        /// </summary>
        /// <param name="target">ドキュメント</param>
        public virtual void OutDocument(ref Object2D_Document target)
        {
            if(target == null)
            {
                target = new Object2D_Document();
            }

            target.IsVisible = this.IsVisible;
            target.IsSelect = this.IsSelect;
            target.ZOrder = this.ZOrder;
        }
    }
}
