using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    internal interface ITextProperty
    {
        /// <summary>
        /// テキストの角度（度数法）
        /// </summary>
        float Angle { get; set; }

        /// <summary>
        /// フォント名
        /// </summary>
        string FontName { get; set; }

        /// <summary>
        /// フォントサイズ
        /// </summary>
        float FontSize { get; set; }

        /// <summary>
        /// フォントカラー
        /// </summary>
        Color Color { get; set; }
    }
}
