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

        // ===============================================================================
        // 非公開メソッド
        // ===============================================================================

        /// <summary>
        /// マウスヒット中の図形（拡大した図形）を返す。
        /// </summary>
        /// <returns>拡張された図形</returns>
        internal override Object2D GetHitObject()
        {
            Arc2D circle = (Arc2D)this.Clone();

            if (circle.IsFilled == true)
            {
                circle.R *= MouseHitPolyOffset;
            }
            else
            {
                circle.LineWidth += MouseHitLineOffset;
            }

            return circle;
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

            if (IsFilled == true)
            {
                eMouseHitType = GraphicCaluculate.IsHitMouseRangeFillArc(this.X, this.Y, this.R, this.StartAngle, this.EndAngle, MousePoint, MusePointRange);
            }
            else
            {
                eMouseHitType = GraphicCaluculate.IsHitMouseRangeLineArc(this.X, this.Y, this.R, this.StartAngle, this.EndAngle, this.LineWidth, this.IsDrawSideLines, MousePoint, MusePointRange);
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
        /// 円弧の開始角度の点を取得する。
        /// </summary>
        /// <returns></returns>
        internal PointF GetArcStartAnglePoint()
        {
            PointF StartPoint = new PointF(X, Y);

            float startRad = GraphicCaluculate.DegreeToRadian(StartAngle);
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

            float endRad = GraphicCaluculate.DegreeToRadian(EndAngle);
            PointF EndPoint = new PointF(
                X + R * (float)Math.Cos(endRad),
                Y + R * (float)Math.Sin(endRad)
                );

            return EndPoint;
        }
    }
}
