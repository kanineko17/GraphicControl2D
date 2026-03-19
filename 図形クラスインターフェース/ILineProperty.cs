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
    /// ライン図形インターフェース
    /// </summary>
    public interface ILineProperty
    {
        /// <summary>
        /// 線の太さ
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// 線の種類
        /// </summary>
        LineStyle Style { get; set; }

        /// <summary>
        /// カスタムの線のパターン
        /// ※StyleプロパティがLineStyle.Customの場合に有効
        /// </summary>
        float[] CustomLineStyle { get; set; }

        /// <summary>
        /// カスタムの線の位相
        /// ※StyleプロパティがLineStyle.Customの場合に有効
        /// </summary>
        float CustomDashPhase { get; set; }

        /// <summary>
        /// 線の色
        /// </summary>
        Color Color { get; set; }
    }
}
