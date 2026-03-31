using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.描画図形クラス
{
    /// <summary>
    /// 線図形描画に必要な情報をまとめたクラス
    /// </summary>
    internal class Line2D_DrawFigure
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
    }
}
