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
    public class Text2D_Document : Object2D_Document, ITextProperty
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
    }
}
