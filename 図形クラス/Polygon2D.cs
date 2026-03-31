using graphicbox2d.グラフィック計算;
using graphicbox2d.描画図形クラス;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace graphicbox2d
{
    /// <summary>
    /// ポリゴン図形クラス
    /// </summary>
    public class Polygon2D : FillObject2D
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Polygon;

        /// <summary>
        /// ポリゴンの頂点リスト
        /// </summary>
        public List<PointF> Points { get; set; } = new List<PointF>();


        // ===============================================================================
        // 非公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の中心点
        /// </summary>
        internal override Vector2 CenterPoint { get { return GetCenterPoint(); } }

        /// <summary>
        /// ポリゴン図形を完全内包する円（外接円）の半径
        /// </summary>
        internal float CircumCircleR { get { return GetCircumCircleR(); } }


        // ===============================================================================
        // 公開メソッド
        // ===============================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Polygon2D()
        {
            Points = new List<PointF>();
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~Polygon2D()
        {
            Points.Clear();
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns>コピーしたオブジェクト</returns>
        public override Object2D Clone()
        {
            Polygon2D polygon = new Polygon2D();

            // 基底クラスのデータをコピーする
            this.BaseCopyDataTo(polygon);

            foreach (var pt in this.Points)
            {
                polygon.Points.Add(new PointF(pt.X, pt.Y));
            }

            return polygon;
        }

        // ===============================================================================
        // 非公開メソッド
        // ===============================================================================

        /// <summary>
        /// ポリゴン座標無しでコピーを作成する
        /// </summary>
        /// <returns>コピーしたオブジェクト</returns>
        internal Object2D CloneWithoutPoints()
        {
            Polygon2D polygon = new Polygon2D();

            // 基底クラスのデータをコピーする
            this.BaseCopyDataTo(polygon);

            return polygon;
        }

        /// <summary>
        /// マウスヒット中の図形（拡大した図形）を返す。
        /// </summary>
        /// <returns>拡大された図形</returns>
        internal override Object2D GetHitObject()
        {
            List<PointF>  OutPoints;
            GraphicCaluculate.GetScalingPolygon(this.Points, CenterPoint.ToPointF(), MouseHitPolyOffset, out OutPoints);

            // ポイントを除いたポリゴンをコピーする
            Polygon2D out_polygon = (Polygon2D)this.CloneWithoutPoints();

            // 拡大したポイントを設定する
            out_polygon.Points.AddRange(OutPoints);

            return out_polygon;
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

            Vector2 _CenterPoint;
            float   _CircumCircleR;

            GraphicCaluculate.GetCenterPointAndCircumCircleR(Points, out _CenterPoint, out _CircumCircleR);

            if (IsFilled == true)
            {
                eMouseHitType = CalIsHit.IsHitMouseRangeFillPolygon(_CenterPoint.ToPoint(), _CircumCircleR, Points, MousePoint, MusePointRange);
            }
            else
            {
                eMouseHitType = CalIsHit.IsHitMouseRangeLinePolygon(_CenterPoint.ToPoint(), _CircumCircleR, Points, LineWidth, MousePoint, MusePointRange);
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
            return GraphicCaluculate.CaluculateCenterPoint(Points);
        }

        /// <summary>
        /// 外接円を取得する
        /// </summary>
        /// <returns></returns>
        internal float GetCircumCircleR()
        {
            Vector2 centerPoint = GraphicCaluculate.CaluculateCenterPoint(Points);

            return GraphicCaluculate.CaluculateCircumCircleR(centerPoint, Points);
        }

        /// <summary>
        /// 描画に必要な情報をまとめたクラスを返す。
        /// </summary>
        /// <param name="type">描画タイプ</param>
        /// <returns>描画用のデータをまとめたクラス</returns>
        internal override object GetDrawFigure(eDrawFigureType type)
        {
            Polygon2D_DrawFigure figure = new Polygon2D_DrawFigure();

            figure.Points = Points.Select(pt => CalConvert.ConvertDisplayGridPointToClientPoint(pt)).ToArray();

            return figure;
        }
    }
}
