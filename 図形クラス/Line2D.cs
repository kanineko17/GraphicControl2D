using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 線図形クラス
    /// </summary>
    public class Line2D : Object2D, ILineProperty
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Line;

        /// <summary>
        /// 始点座標
        /// </summary>
        public PointF Start { get; set; } = new Point(0, 0);

        /// <summary>
        /// 終点座標
        /// </summary>
        public PointF End { get; set; } = new Point(0, 0);

        /// <summary>
        /// 線の太さ
        /// </summary>
        public int Width { get; set; } = 1;

        /// <summary>
        /// 線の種類
        /// </summary>
        public LineStyle Style { get; set; } = LineStyle.Solid;

        /// <summary>
        /// カスタムの線のパターン
        /// ※StyleプロパティがLineStyle.Customの場合に有効
        /// </summary>
        public float[] CustomLineStyle { get; set; } = null;

        /// <summary>
        /// カスタムの線の位相
        /// ※StyleプロパティがLineStyle.Customの場合に有効
        /// </summary>
        public float CustomDashPhase { get; set; } = 0;

        /// <summary>
        /// 図形の色
        /// </summary>
        public Color Color { get; set; } = Color.White;


        // ===============================================================================
        // 非公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の中心点
        /// </summary>
        internal override Vector2 CenterPoint { get { return GeCentertPoint(); } }

        // ===============================================================================
        // 公開メソッド
        // ===============================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Line2D() 
        {
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns>コピーしたオブジェクト</returns>
        public override Object2D Clone()
        {
            Line2D clone = new Line2D();

            // 基底クラスのデータをコピー
            this.BaseCopyDataTo(clone);

            // 自クラスのデータをコピー
            clone.Start           = this.Start;
            clone.End             = this.End;
            clone.Width           = this.Width;
            clone.Style           = this.Style;
            clone.CustomLineStyle = this.CustomLineStyle;
            clone.Color           = this.Color;

            return clone;
        }

        // ===============================================================================
        // 非公開メソッド
        // ===============================================================================

        /// <summary>
        /// マウスヒット中の図形（拡大した図形）を返す。
        /// </summary>
        /// <returns>拡張された図形</returns>
        internal override Object2D GetHitObject()
        {
            Line2D line2D = (Line2D)this.Clone();

            line2D.Width += MouseHitLineOffset;

            return line2D;
        }

        /// <summary>
        /// 線の中心点を更新する
        /// </summary>
        protected Vector2 GeCentertPoint()
        {
            return (Start.ToVector2() + End.ToVector2()) / 2;
        }

        /// <summary>
        /// マウスポイントがこの図形にヒットしているか判定する。
        /// </summary>
        /// <param name="MousePoint">マウスポイント</param>
        /// <param name="MusePointRange">マウスヒット半径</param>
        /// <returns></returns>
        internal override eMouseHitType IsHitMousePoint(PointF MousePoint, float MusePointRange)
        {
            return GraphicCaluculate.IsHitMouseRangeLine(Start, End, Width, MousePoint, MusePointRange);
        }

        /// <summary>
        /// マウスポイントとこの図形の距離を取得する
        /// </summary>
        /// <param name="X">マウスポイントX座標</param>
        /// <param name="Y">マウスポイントY座標</param>
        /// <returns>距離</returns>
        internal override float GetDistanceHitMousePoint(float X, float Y)
        {
            Vector2 MousePoint = new Vector2(X, Y);

            return Vector2.Distance(MousePoint, CenterPoint);
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
        /// <param name="Movement">移動量</param>
        internal override void Move(PointF Movement)
        {
            Start = new PointF(Start.X + Movement.X, Start.Y + Movement.Y);
            End   = new PointF(End.X   + Movement.X, End.Y   + Movement.Y);
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
        /// <param name="X">移動量X</param>
        /// <param name="Y">移動量Y</param>
        internal override void Move(float X, float Y)
        {
            Start = new PointF(Start.X + X, Start.Y + Y);
            End   = new PointF(End.X   + X, End.Y   + Y);
        }

        /// <summary>
        /// バウンディングボックスを取得する
        /// </summary>
        internal override PointF [] GetBoundingBox()
        {
            return GraphicCaluculate.GetBoundingBoxLine(Start, End, Width, eCalculateType.Grid);
        }

        /// <summary>
        /// コピー元のObject2Dデータをコピー先にコピーする。
        /// </summary>
        /// <param name="target">コピー先</param>
        protected new void BaseCopyDataTo(Object2D target)
        {
            base.BaseCopyDataTo(target);

            Line2D line2D = (Line2D)target;
            line2D.Start = this.Start;
            line2D.End   = this.End;
            line2D.Width = this.Width;
            line2D.Style = this.Style;
            line2D.Color = this.Color;
        }
    }
}
