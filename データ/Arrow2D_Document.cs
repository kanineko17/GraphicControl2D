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
    /// 矢印図形クラス
    /// </summary>
    public class Arrow2D_Document : Line2D_Document, ILineProperty
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Arrow;

        /// <summary>
        /// 線分の始点の矢印スタイル
        /// </summary>
        public Arrow2DLineCapType StartCap { get; set; } = Arrow2DLineCapType.None;

        /// <summary>
        /// 線分の終点の矢印スタイル
        /// </summary>
        public Arrow2DLineCapType EndCap { get; set; } = Arrow2DLineCapType.Arrow;

        /// <summary>
        /// 線分の始点の矢印サイズ
        /// </summary>
        public CapSize StartCapSize { get; set; } = new CapSize() { Width = 4, Height = 4 };

        /// <summary>
        /// 線分の終点の矢印サイズ
        /// </summary>
        public CapSize EndCapSize { get; set; } = new CapSize() { Width = 4, Height = 4 };

    }
}
