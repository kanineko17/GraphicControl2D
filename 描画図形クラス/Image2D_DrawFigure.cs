using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.描画図形クラス
{
    /// <summary>
    /// イメージ図形描画に必要な情報をまとめたクラス
    /// </summary>
    internal class Image2D_DrawFigure
    {
        /// <summary>
        /// X座標
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y座標
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// 回転角度（度）
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// 描画用ビットマップ
        /// </summary>
        public SKBitmap Bitmap { get; set; }
    }
}
