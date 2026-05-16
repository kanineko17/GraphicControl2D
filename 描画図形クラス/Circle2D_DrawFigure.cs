using graphicbox2d.グラフィック計算;
using graphicbox2d.図形クラスインターフェース;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.描画図形クラス
{
    /// <summary>
    /// 円図形描画に必要な情報をまとめたクラス
    /// </summary>
    internal class Circle2D_DrawFigure : Object2D_DrawFigure, IDrawFigure
    {
        /// <summary>
        /// X座標（クライアント座標）
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y座標（クライアント座標）
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// 半径（クライアント座標）
        /// </summary>
        public float R { get; set; }

        /// <summary>
        /// クライアント座標のバウンディングボックス
        /// </summary>
        public SKPoint[] BoundingBox => GetBoundingBox();

        /// <summary>
        /// クライアント座標の再描画領域を表す矩形
        /// </summary>
        public Rectangle InvalidateRect => GetInvalidateRect();

        /// <summary>
        /// クライアント座標のバウンディングボックスを取得する
        /// </summary>
        /// <returns></returns>
        public SKPoint[] GetBoundingBox()
        {
            return CalBoundBox.GetBoundingBoxCircleSK(X, Y, R);
        }

        /// <summary>
        /// クライアント座標の再描画領域を表す矩形
        /// </summary>
        /// <returns></returns>
        public Rectangle GetInvalidateRect()
        {
            return CalBoundBox.ConvertBoundingBoxToRect(BoundingBox, DEF_INVALIDATE_OFFSET);
        }
    }
}
