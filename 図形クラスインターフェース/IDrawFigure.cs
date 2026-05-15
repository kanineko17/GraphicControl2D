using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.図形クラスインターフェース
{
    internal interface IDrawFigure
    {
        /// <summary>
        /// クライアント座標のバウンディングボックス
        /// </summary>
        SKPoint[] BoundingBox { get;}

        /// <summary>
        /// クライアント座標のバウンディングボックスを表す矩形
        /// </summary>
        Rectangle BoundingBoxRect { get;}
    }
}
