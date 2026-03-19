using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 円弧図形クラス
    /// </summary>
    public class Arc2D_Document : Circle2D_Document
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Arc;

        /// <summary>
        /// 開始角度（度数法）
        /// </summary>
        public float StartAngle { get; set; } = 0.0f;

        /// <summary>
        /// 終了角度（度数法）
        /// </summary>
        public float EndAngle { get; set; } = 0.0f;

        /// <summary>
        /// 円弧の両サイドの線を描画するかどうか
        /// </summary>
        public bool IsDrawSideLines { get; set; } = false;
    }
}
