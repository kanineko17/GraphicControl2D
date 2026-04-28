using graphicbox2d.グラフィック計算;
using graphicbox2d.グローバル変数;
using graphicbox2d.描画図形クラス;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 円弧図形クラス
    /// </summary>
    public class Arc2D : Circle2D
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Arc;

        /// <summary>
        /// 開始角度（度数法）
        /// </summary>
        public float StartAngle { get; set; } = 0.0f;

        /// <summary>
        /// 終了角度（度数法）
        /// </summary>
        public float EndAngle { get; set; } = 0.0f;

        /// <summary>
        /// 円弧の両サイドの線を描画するかどうか
        /// </summary>
        public bool IsDrawSideLines { get; set; } = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Arc2D()
        {
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns>コピーしたオブジェクト</returns>
        public override Object2D Clone()
        {
            Arc2D clone = new Arc2D();

            // 基底クラスのデータをコピー
            this.BaseCopyDataTo(clone);

            // 派生クラスのデータをコピー
            clone.StartAngle = this.StartAngle;
            clone.EndAngle = this.EndAngle;
            clone.IsDrawSideLines = this.IsDrawSideLines;

            return clone;
        }

        // ===============================================================================
        // 非公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 円弧の開始角度の点
        /// </summary>
        internal PointF ArcStartAnglePoint => GetArcStartAnglePoint();

        /// <summary>
        /// 円弧の終了角度の点
        /// </summary>
        internal PointF ArcEndAnglePoint => GetArcEndAnglePoint();

        /// <summary>
        /// 図形の選択ポイント
        /// </summary>
        internal override List<PointF> SnapPoints => GetSnapPoints();

        // ===============================================================================
        // 非公開メソッド
        // ===============================================================================

        /// <summary>
        /// マウスポイントがこの図形にヒットしているか判定する。
        /// </summary>
        /// <param name="MousePoint">マウスポイント</param>
        /// <param name="MusePointRange">マウスヒット半径</param>
        /// <returns></returns>
        internal override eMouseHitType IsHitMousePoint(PointF MousePoint, float MusePointRange)
        {
            eMouseHitType eMouseHitType;

            if (IsFilled == true)
            {
                eMouseHitType = CalIsHit.IsHitMouseRangeFillArc(this.X, this.Y, this.R, this.StartAngle, this.EndAngle, MousePoint, MusePointRange);
            }
            else
            {
                eMouseHitType = CalIsHit.IsHitMouseRangeLineArc(this.X, this.Y, this.R, this.StartAngle, this.EndAngle, this.LineWidth, this.IsDrawSideLines, MousePoint, MusePointRange);
            }

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
        /// 描画に必要な情報をまとめたクラスを返す。
        /// </summary>
        /// <param name="type">描画タイプ</param>
        /// <returns>描画用のデータをまとめたクラス</returns>
        internal override object GetDrawFigure(eDrawFigureType type)
        {
            Arc2D_DrawFigure figure = new Arc2D_DrawFigure();

            SKPoint clientPoint = CalConvert.ConvertDisplayGridPointToClientPoint(new PointF(X, Y));

            figure.X = clientPoint.X;
            figure.Y = clientPoint.Y;
            figure.R = R * Global.Graphic2DControl.DisplayGridWidth;
            figure.StartAngle = StartAngle;
            figure.EndAngle = EndAngle;
            figure.IsDrawSideLines = IsDrawSideLines;

            return figure;
        }

        /// <summary>
        /// スナップポイントを取得する
        /// </summary>
        /// <returns>スナップポイントのリスト</returns>
        private List<PointF> GetSnapPoints()
        {
            return new List<PointF>
            {
                new PointF(X, Y),
                ArcStartAnglePoint,
                ArcEndAnglePoint,
            };
        }

        /// <summary>
        /// 円弧の開始角度の点を取得する。
        /// </summary>
        /// <returns></returns>
        internal PointF GetArcStartAnglePoint()
        {
            PointF StartPoint = new PointF(X, Y);

            float startRad = CalConvert.DegreeToRadian(StartAngle);
            PointF EndPoint = new PointF(
                X + R * (float)Math.Cos(startRad),
                Y + R * (float)Math.Sin(startRad)
                );

            return EndPoint;
        }

        /// <summary>
        /// 円弧の終了角度の点を取得する。
        /// </summary>
        /// <returns></returns>
        internal PointF GetArcEndAnglePoint()
        {
            PointF StartPoint = new PointF(X, Y);

            float endRad = CalConvert.DegreeToRadian(EndAngle);
            PointF EndPoint = new PointF(
                X + R * (float)Math.Cos(endRad),
                Y + R * (float)Math.Sin(endRad)
                );

            return EndPoint;
        }

        /// <summary>
        /// ドキュメントデータを書き出す
        /// </summary>
        /// <param name="doc">出力先ドキュメント</param>
        public override void OutDocument(ref Object2D_Document doc)
        {
            if (doc == null)
            {
                doc = new Arc2D_Document();
            }

            base.OutDocument(ref doc);

            Arc2D_Document arcDoc = (Arc2D_Document)doc;
            arcDoc.StartAngle = this.StartAngle;
            arcDoc.EndAngle = this.EndAngle;
            arcDoc.IsDrawSideLines = this.IsDrawSideLines;
        }

        /// <summary>
        /// ドキュメントデータを取り込む
        /// </summary>
        /// <param name="doc">取り込むドキュメント</param>
        public override void ImportDocument(in Object2D_Document doc)
        {
            base.ImportDocument(doc);

            Arc2D_Document arcDoc = (Arc2D_Document)doc;
            this.StartAngle = arcDoc.StartAngle;
            this.EndAngle = arcDoc.EndAngle;
            this.IsDrawSideLines = arcDoc.IsDrawSideLines;
        }
    }
}
