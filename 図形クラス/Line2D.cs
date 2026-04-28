using graphicbox2d.グラフィック計算;
using graphicbox2d.描画図形クラス;
using SkiaSharp;
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

        /// <summary>
        /// 図形の選択ポイント
        /// </summary>
        internal override List<PointF> SnapPoints => GetSnapPoints();


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
            return CalIsHit.IsHitMouseRangeLine(Start, End, Width, MousePoint, MusePointRange);
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
            return CalBoundBox.GetBoundingBoxLine(Start, End, Width, eCalculateType.Grid);
        }

        /// <summary>
        /// 描画に必要な情報をまとめたクラスを返す。
        /// </summary>
        /// <param name="type">描画タイプ</param>
        /// <returns>描画用のデータをまとめたクラス</returns>
        internal override object GetDrawFigure(eDrawFigureType type)
        {
            Line2D_DrawFigure figure = new Line2D_DrawFigure();

            // グリッド座標をクライアント座標に変換
            figure.Start = CalConvert.ConvertDisplayGridPointToClientPoint(Start);
            figure.End = CalConvert.ConvertDisplayGridPointToClientPoint(End);

            if (type == eDrawFigureType.Normal)
            {
                figure.Width = Width;
            }
            else if (type == eDrawFigureType.Hit)
            {
                figure.Width = Width + MouseHitLineOffset;
            }

            return figure;
        }

        /// <summary>
        /// スナップポイントを取得する
        /// </summary>
        /// <returns>スナップポイントのリスト</returns>
        private List<PointF> GetSnapPoints()
        {
            return new List<PointF> { Start, CenterPoint.ToPointF(), End };
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
