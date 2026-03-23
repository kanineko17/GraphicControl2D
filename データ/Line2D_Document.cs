using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 線図形クラス
    /// </summary>
    public class Line2D_Document : Object2D_Document, ILineProperty
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Line;

        /// <summary>
        /// 始点座標
        /// </summary>
        public PointF Start { get; set; } = new Point(0, 0);

        /// <summary>
        /// 終点座標
        /// </summary>
        public PointF End { get; set; } = new Point(0, 0);

        /// <summary>
        /// 線の太さ
        /// </summary>
        public int Width { get; set; } = 1;

        /// <summary>
        /// 線の種類
        /// </summary>
        public LineStyle Style { get; set; } = LineStyle.Solid;

        /// <summary>
        /// カスタムの線のパターン
        /// ※StyleプロパティがLineStyle.Customの場合に有効
        /// </summary>
        public float[] CustomLineStyle { get; set; } = null;

        /// <summary>
        /// カスタムの線の位相
        /// ※StyleプロパティがLineStyle.Customの場合に有効
        /// </summary>
        public float CustomDashPhase { get; set; } = 0;

        /// <summary>
        /// 図形の色
        /// </summary>
        public Color Color { get; set; } = Color.White;
    }
}
