using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.描画図形クラス
{
    /// <summary>
    /// 円図形描画に必要な情報をまとめたクラス
    /// </summary>
    internal class Circle2D_DrawFigure
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
    }
}
