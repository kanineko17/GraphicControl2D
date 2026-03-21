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
    /// 円図形クラス
    /// </summary>
    public class Circle2D_Document : FillObject2D_Document
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Circle;

        /// <summary>
        /// X座標
        /// </summary>
        public float X { get; set; } = 0;

        /// <summary>
        /// Y座標
        /// </summary>
        public float Y { get; set; } = 0;

        /// <summary>
        /// 円の半径
        /// </summary>
        public float R { get; set; } = 0;
    }
}
