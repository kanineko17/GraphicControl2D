using graphicbox2d.グラフィック計算;
using SkiaSharp;
using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace graphicbox2d
{
    /// <summary>
    /// 画像図形クラス
    /// </summary>
    public class Image2D : Object2D
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Image;

        /// <summary>
        /// 左上X座標
        /// </summary>
        public float X { get; set; } = 0;

        /// <summary>
        /// 左上Y座標
        /// </summary>
        public float Y { get; set; } = 0;

        /// <summary>
        /// 幅
        /// </summary>
        public float Width { get; set; } = 100;

        /// <summary>
        /// 高さ
        /// </summary>
        public float Height { get; set; } = 100;

        /// <summary>
        /// 回転角度（度）
        /// </summary>
        public float Angle { get; set; } = 0;

        /// <summary>
        /// 描画する画像
        /// </summary>
        public SKBitmap Bitmap { get; set; } = null;


        // ===============================================================================
        // 非公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の中心点
        /// </summary>
        internal override Vector2 CenterPoint
        {
            get { return new Vector2(X + Width / 2f, Y + Height / 2f); }
        }


        // ===============================================================================
        // 公開メソッド
        // ===============================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Image2D()
        {
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns>コピーしたオブジェクト</returns>
        public override Object2D Clone()
        {
            Image2D clone = new Image2D();

            // 基底クラスのデータをコピー
            this.BaseCopyDataTo(clone);

            // 派生クラスのデータをコピー
            clone.X = this.X;
            clone.Y = this.Y;
            clone.Width = this.Width;
            clone.Height = this.Height;
            clone.Angle = this.Angle;

            // Bitmap は参照コピー（必要なら DeepCopy に変更）
            clone.Bitmap = this.Bitmap;

            return clone;
        }


        // ===============================================================================
        // 非公開メソッド
        // ===============================================================================

        /// <summary>
        /// マウスヒット中の図形（拡大した図形）を返す。
        /// </summary>
        internal override Object2D GetHitObject()
        {
            Image2D img = (Image2D)this.Clone();

            return img;
        }

        /// <summary>
        /// マウスポイントがこの図形にヒットしているか判定する。
        /// </summary>
        internal override eMouseHitType IsHitMousePoint(PointF MousePoint, float MusePointRange)
        {
            eMouseHitType eMouseHitType;

            Vector2 _CenterPoint;
            float _CircumCircleR;

            PointF[] Points = GetBoundingBox();

            GraphicCaluculate.GetCenterPointAndCircumCircleR(Points, out _CenterPoint, out _CircumCircleR);

            eMouseHitType = CalIsHit.IsHitMouseRangeFillPolygon(_CenterPoint.ToPoint(), _CircumCircleR, Points.ToList(), MousePoint, MusePointRange);

            return eMouseHitType;
        }

        /// <summary>
        /// マウスポイントとこの図形の距離を取得する
        /// </summary>
        internal override float GetDistanceHitMousePoint(float X, float Y)
        {
            Vector2 p = new Vector2(X, Y);
            return Vector2.Distance(p, CenterPoint);
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
        internal override void Move(PointF Movement)
        {
            X += Movement.X;
            Y += Movement.Y;
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
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
            return CalBoundBox.GetBoundingBox(
                X, Y, Width, Height, Angle, eCalculateType.Grid, eRotateType.Center);
        }

        /// <summary>
        /// コピー元のObject2Dデータをコピー先にコピーする。
        /// </summary>
        protected new void BaseCopyDataTo(Object2D target)
        {
            base.BaseCopyDataTo(target);

            Image2D img = (Image2D)target;

            img.X = this.X;
            img.Y = this.Y;
            img.Width = this.Width;
            img.Height = this.Height;
            img.Angle = this.Angle;
            img.Bitmap = this.Bitmap;
        }
    }
}