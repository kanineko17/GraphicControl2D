using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 塗りつぶし図形インターフェース
    /// </summary>
    internal interface IFillProperty
    {
        /// <summary>
        /// 線の太さ
        /// </summary>
        int LineWidth { get; set; }

        /// <summary>
        /// 線の種類
        /// </summary>
        LineStyle LineStyle { get; set; }

        /// <summary>
        /// カスタムの線のパターン
        /// ※StyleプロパティがLineStyle.Customの場合に有効
        /// </summary>
        float[] LineCustomLineStyle { get; set; }

        /// <summary>
        /// カスタムの線の位相
        /// ※StyleプロパティがLineStyle.Customの場合に有効
        /// </summary>
        float LineCustomDashPhase { get; set; }

        /// <summary>
        /// 線の色
        /// </summary>
        Color LineColor { get; set; }

        /// <summary>
        /// 塗りつぶしの色
        /// </summary>
        Color FillColor { get; set; }
    }
}
