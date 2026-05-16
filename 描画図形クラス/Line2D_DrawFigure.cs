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
    /// 線図形描画に必要な情報をまとめたクラス
    /// </summary>
    internal class Line2D_DrawFigure : Object2D_DrawFigure, IDrawFigure
    {
        /// <summary>
        /// 始点
        /// </summary>
        public SKPoint Start { get; set; }

        /// <summary>
        /// 終点
        /// </summary>
        public SKPoint End { get; set; }

        /// <summary>
        /// 線幅
        /// </summary>
        public int Width { get; set; }

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
            return CalBoundBox.GetBoundingBoxLineSK(Start, End, Width);
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
