using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace graphicbox2d
{
    /// <summary>
    /// ポリゴン図形クラス
    /// </summary>
    public class Polygon2D_Document : FillObject2D_Document
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Polygon;

        /// <summary>
        /// ポリゴンの頂点リスト
        /// </summary>
        public List<PointF> Points { get; set; } = new List<PointF>();
    }
}
