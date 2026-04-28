using graphicbox2d.グラフィック計算;
using graphicbox2d.グローバル変数;
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
    /// 円図形クラス
    /// </summary>
    public class Circle2D : FillObject2D
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Circle;

        /// <summary>
        /// X座標
        /// </summary>
        public float X { get; set; } = 0;

        /// <summary>
        /// Y座標
        /// </summary>
        public float Y { get; set; } = 0;

        /// <summary>
        /// 円の半径
        /// </summary>
        public float R { get; set; } = 0;


        // ===============================================================================
        // 非公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の中心点
        /// </summary>
        internal override Vector2 CenterPoint { get { return new Vector2(X, Y); } }

        // ===============================================================================
        // 公開メソッド
        // ===============================================================================


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Circle2D()
        {
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns>コピーしたオブジェクト</returns>
        public override Object2D Clone()
        {
            Circle2D clone = new Circle2D();

            // 基底クラスのデータをコピー
            this.BaseCopyDataTo(clone);

            // 派生クラスのデータをコピー
            clone.X = this.X;
            clone.Y = this.Y;
            clone.R = this.R;

            return clone;
        }

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
                eMouseHitType = CalIsHit.IsHitMouseRangeFillCircle(this.X, this.Y, this.R, MousePoint, MusePointRange);
            }
            else
            {
                eMouseHitType = CalIsHit.IsHitMouseRangeLineCircle(this.X, this.Y, this.R, this.LineWidth, MousePoint, MusePointRange);
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
            X += Movement.X;
            Y += Movement.Y;
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
        /// <param name="X">移動量X</param>
        /// <param name="Y">移動量Y</param>
        internal override void Move(float X, float Y)
        {
            this.X += X;
            this.Y += Y;
        }

        /// <summary>
        /// バウンディングボックスを取得する
        /// </summary>
        internal override PointF[] GetBoundingBox()
        {
            return CalBoundBox.GetBoundingBoxCircle(X, Y, R);
        }

        /// <summary>
        /// 描画に必要な情報をまとめたクラスを返す。
        /// </summary>
        /// <param name="type">描画タイプ</param>
        /// <returns>描画用のデータをまとめたクラス</returns>
        internal override object GetDrawFigure(eDrawFigureType type)
        {
            Circle2D_DrawFigure figure = new Circle2D_DrawFigure();

            SKPoint clientPoint = CalConvert.ConvertDisplayGridPointToClientPoint(new PointF(X, Y));

            figure.X = clientPoint.X;
            figure.Y = clientPoint.Y;
            figure.R = R * Global.Graphic2DControl.DisplayGridWidth;

            if(type == eDrawFigureType.Hit && IsFilled == true)
            {
                figure.R *= MouseHitPolyOffset;
            }

            return figure;
        }

        /// <summary>
        /// コピー元のObject2Dデータをコピー先にコピーする。
        /// </summary>
        /// <param name="target">コピー先</param>
        protected new void BaseCopyDataTo(Object2D target)
        {
            base.BaseCopyDataTo(target);

            Circle2D circle = (Circle2D)target;

            circle.X = this.X;
            circle.Y = this.Y;
            circle.R = this.R;
        }

        /// <summary>
        /// ドキュメントデータを書き出す
        /// </summary>
        /// <param name="doc">出力先ドキュメント</param>
        public override void OutDocument(ref Object2D_Document doc)
        {
            if (doc == null)
            {
                doc = new Circle2D_Document();
            }

            base.OutDocument(ref doc);

            Circle2D_Document circleDoc = (Circle2D_Document)doc;
            circleDoc.X = this.X;
            circleDoc.Y = this.Y;
            circleDoc.R = this.R;
        }

        /// <summary>
        /// ドキュメントデータを取り込む
        /// </summary>
        /// <param name="doc">取り込むドキュメント</param>
        public override void ImportDocument(in Object2D_Document doc)
        {
            base.ImportDocument(doc);

            Circle2D_Document circleDoc = (Circle2D_Document)doc;
            this.X = circleDoc.X;
            this.Y = circleDoc.Y;
            this.R = circleDoc.R;
        }
    }
}
