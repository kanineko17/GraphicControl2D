using graphicbox2d.グラフィック計算;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace graphicbox2d
{
    /// <summary>
    /// テキスト図形クラス
    /// </summary>
    public class Text2D : Object2D, ITextProperty
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Text;

        /// <summary>
        /// X座標
        /// </summary>
        public float X { get; set; } = 0;

        /// <summary>
        /// Y座標
        /// </summary>
        public float Y { get; set; } = 0;

        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// テキストの角度（度数法）
        /// </summary>
        public float Angle { get; set; } = 0;

        /// <summary>
        /// フォント名
        /// </summary>
        public string FontName { get;set;} = "MS UI Gothic";

        /// <summary>
        /// フォントサイズ
        /// </summary>
        public float FontSize { get; set; } = 24f;

        /// <summary>
        /// フォントカラー
        /// </summary>
        public Color Color { get; set; } = Color.White;

        // ===============================================================================
        // 非公開プロパティ
        // ===============================================================================

        /// <summary>
        /// マウスヒット中のテキストのフォントサイズの加算値
        /// </summary>
        internal float MouseHitFontSizeOffset { get; set; } = 1.0f;

        /// <summary>
        /// 図形の中心点
        /// </summary>
        internal override Vector2 CenterPoint => GetCenterPoint();

        /// <summary>
        /// ポリゴン図形を完全内包する円（外接円）の半径
        /// </summary>
        internal float CircumCircleR => GetCircumCircleR();

        /// <summary>
        /// 描画時に使用するフォントサイズ
        /// </summary>
        internal float DrawFontSize => FontSize * Graphic2DControl.UserZoom;

        // ===============================================================================
        // 公開メソッド
        // ===============================================================================

        /// <summary>
        /// コンストラクタ（座標を初期化）
        /// </summary>
        public Text2D()
        {
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns>コピーしたオブジェクト</returns>
        public override Object2D Clone()
        {
            Text2D clone = new Text2D();

            // 基底クラスのデータをコピー
            this.BaseCopyDataTo(clone);

            // 派生クラスのデータをコピー
            clone.X = this.X;
            clone.Y = this.Y;
            clone.Text = this.Text;
            clone.Angle = this.Angle;
            clone.FontName = this.FontName;
            clone.FontSize = this.FontSize;
            clone.Color = this.Color;

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
            Text2D text = (Text2D)this.Clone();

            // マウスヒット用に半径を拡大
            text.FontSize += MouseHitFontSizeOffset;

            return text;
        }

        /// <summary>
        /// マウスポイントがこの図形にヒットしているか判定する。
        /// 判定は「マウスポイントが判定半径以内にあるか」で行う。
        /// </summary>
        /// <param name="X">マウスポイントX座標</param>
        /// <param name="Y">マウスポイントY座標</param>
        /// <param name="MusePointRange">マウスの有効誤差範囲</param>
        /// <returns>true = ヒットしている</returns>
        internal override eMouseHitType IsHitMousePoint(PointF MousePoint, float MusePointRange)
        {
            eMouseHitType eMouseHitType;

            PointF[] _BoundingBox;
            Vector2 _CenterPoint;
            float _CircumCircleR;

            GetBoundingBoxCenterPointAndCircumCircleR(out _BoundingBox, out _CenterPoint, out _CircumCircleR);

            eMouseHitType = CalIsHit.IsHitMouseRangeFillPolygon(_CenterPoint.ToPointF(), _CircumCircleR, _BoundingBox.ToList(), MousePoint, MusePointRange);

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
            return CalBoundBox.GetBoundingBoxText(X, Y, Text, DrawFontSize, FontName, Angle, eCalculateType.Grid);
        }

        /// <summary>
        /// ポリゴンの中心点と外接円の半径を同時に取得する
        /// </summary>
        /// <param name="_CenterPoint">ポリゴンの中心点</param>
        /// <param name="_CircumCircleR">外接円の半径</param>
        internal void GetBoundingBoxCenterPointAndCircumCircleR(out PointF[] _BoundingBox, out Vector2 _CenterPoint, out float _CircumCircleR)
        {
            _BoundingBox = GetBoundingBox();
            _CenterPoint = GraphicCaluculate.CaluculateCenterPoint(_BoundingBox);
            _CircumCircleR = GraphicCaluculate.CaluculateCircumCircleR(_CenterPoint, _BoundingBox);
        }

        /// <summary>
        /// 中心点を取得する
        /// </summary>
        /// <returns></returns>
        internal Vector2 GetCenterPoint()
        {
            PointF[] _BoundingBox;
            Vector2 _CenterPoint;

            _BoundingBox = GetBoundingBox();
            _CenterPoint = GraphicCaluculate.CaluculateCenterPoint(_BoundingBox);

            return _CenterPoint;
        }

        /// <summary>
        /// 外接円を取得する
        /// </summary>
        /// <returns></returns>
        internal float GetCircumCircleR()
        {
            PointF[] _BoundingBox;
            Vector2 _CenterPoint;
            float _CircumCircleR;

            _BoundingBox = GetBoundingBox();
            _CenterPoint = GraphicCaluculate.CaluculateCenterPoint(_BoundingBox);
            _CircumCircleR = GraphicCaluculate.CaluculateCircumCircleR(_CenterPoint, _BoundingBox);

            return _CircumCircleR;
        }
    }
}
