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
    /// グラフ図形クラス
    /// </summary>
    public class Graph2D : Object2D, ILineProperty
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Graph;

        /// <summary>
        /// グラフの点リスト
        /// </summary>
        public List<PointF> Points { get; set; } = new List<PointF>();

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
        public float CustomDashPhase { get; set; } = 0f;

        /// <summary>
        /// 線の色
        /// </summary>
        public Color Color { get; set; } = Color.White;

        // ===============================================================================
        // 非公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の中心点
        /// </summary>
        internal override Vector2 CenterPoint { get { return GetCenterPoint(); } }

        /// <summary>
        /// 原点とXの値が最も近い点のインデックス値
        /// </summary>
        private int ZeroXPointIndex = -1;

        // ===============================================================================
        // 公開メソッド
        // ===============================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Graph2D() 
        {
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns>コピーしたオブジェクト</returns>
        public override Object2D Clone()
        {
            Graph2D clone = new Graph2D();

            // 基底クラスのデータをコピー
            this.BaseCopyDataTo(clone);

            clone.Points = new List<PointF>(this.Points);
            clone.Width = this.Width;
            clone.Style = this.Style;
            clone.CustomLineStyle = this.CustomLineStyle;
            clone.Color = this.Color;

            return clone;
        }

        // ===============================================================================
        // 非公開メソッド
        // ===============================================================================

        /// <summary>
        /// コピー元のObject2Dデータをコピー先にコピーする。
        /// </summary>
        /// <param name="target">コピー先</param>
        protected new void BaseCopyDataTo(Object2D target)
        {
            base.BaseCopyDataTo(target);

            Graph2D targetGraph2D = target as Graph2D;

            if (targetGraph2D != null)
            {
                targetGraph2D.Points = new List<PointF>(this.Points);
                targetGraph2D.Width = this.Width;
                targetGraph2D.Style = this.Style;
                targetGraph2D.CustomLineStyle = this.CustomLineStyle;
                targetGraph2D.Color = this.Color;
            }
        }

        /// <summary>
        /// マウスポイントがこの図形にヒットしているか判定する。
        /// </summary>
        /// <param name="MousePoint">マウスポイント</param>
        /// <param name="MusePointRange">マウスヒット半径</param>
        /// <returns></returns>
        internal override eMouseHitType IsHitMousePoint(PointF MousePoint, float MusePointRange)
        {
            eMouseHitType eMouseHitType;

            eMouseHitType = CalIsHit.IsHitMouseRangeLineGraph(Points, Width, MousePoint, MusePointRange);

            return eMouseHitType;
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
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = new PointF(Points[i].X + Movement.X, Points[i].Y + Movement.Y);
            }
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
        /// <param name="X">移動量X</param>
        /// <param name="Y">移動量Y</param>
        internal override void Move(float X, float Y)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = new PointF(Points[i].X + X, Points[i].Y + Y);
            }
        }

        /// <summary>
        /// バウンディングボックスを取得する
        /// </summary>
        internal override PointF[] GetBoundingBox()
        {
            return CalBoundBox.GetBoundingBoxPolygon(Points.ToArray());
        }

        /// <summary>
        /// 中心点を取得する
        /// </summary>
        /// <returns></returns>
        internal Vector2 GetCenterPoint()
        {
            if (ZeroXPointIndex == -1 || Points == null || Points.Count == 0)
            {
                return new Vector2(0, 0);
            }

            return Points[ZeroXPointIndex].ToVector2();
        }

        /// <summary>
        /// 描画に必要な情報をまとめたクラスを返す。
        /// </summary>
        /// <param name="type">描画タイプ</param>
        /// <returns>描画用のデータをまとめたクラス</returns>
        internal override object GetDrawFigure(eDrawFigureType type)
        {
            if (Points.Count == 0)
            {
                return null;
            }

            Graph2D_DrawFigure figure = new Graph2D_DrawFigure();

            figure.Points = Points.Select(pt => CalConvert.ConvertDisplayGridPointToClientPoint(pt)).ToArray();

            return figure;
        }

        /// <summary>
        /// ドキュメントデータを書き出す
        /// </summary>
        /// <param name="doc">出力先ドキュメント</param>
        public override void OutDocument(ref Object2D_Document doc)
        {
            if (doc == null)
            {
                doc = new Graph2D_Document();
            }

            base.OutDocument(ref doc);

            Graph2D_Document graphDoc = (Graph2D_Document)doc;
            graphDoc.Points = this.Points.ToList();
            graphDoc.Width = this.Width;
            graphDoc.Style = this.Style;
            graphDoc.CustomLineStyle = this.CustomLineStyle?.Clone() as float[];
            graphDoc.CustomDashPhase = this.CustomDashPhase;
            graphDoc.Color = this.Color;
        }

        /// <summary>
        /// ドキュメントデータを取り込む
        /// </summary>
        /// <param name="doc">取り込むドキュメント</param>
        public override void ImportDocument(in Object2D_Document doc)
        {
            base.ImportDocument(doc);

            Graph2D_Document graphDoc = (Graph2D_Document)doc;
            this.Points = graphDoc.Points.ToList();
            this.Width = graphDoc.Width;
            this.Style = graphDoc.Style;
            this.CustomLineStyle = graphDoc.CustomLineStyle?.Clone() as float[];
            this.CustomDashPhase = graphDoc.CustomDashPhase;
            this.Color = graphDoc.Color;
        }
    }
}
