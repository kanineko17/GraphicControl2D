using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.描画図形クラス
{
    /// <summary>
    /// グラフ図形描画に必要な情報をまとめたクラス
    /// </summary>
    internal class Graph2D_DrawFigure
    {
        /// <summary>
        /// 点リスト（クライアント座標）
        /// </summary>
        public SKPoint[] Points { get; set; }
    }
}
