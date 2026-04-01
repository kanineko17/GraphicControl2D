using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.描画図形クラス
{
    /// <summary>
    /// テキスト図形描画に必要な情報をまとめたクラス
    /// </summary>
    internal class Text2D_DrawFigure
    {
        /// <summary>
        /// 描画座標（クライアント座標）
        /// </summary>
        public SKPoint ClientPoint { get; set; }

        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// テキストの角度（度数法）
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// フォント名
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// 描画時に使用するフォントサイズ
        /// </summary>
        public float FontSize { get; set; }
    }
}
