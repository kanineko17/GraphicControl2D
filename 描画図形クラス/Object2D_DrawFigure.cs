using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.描画図形クラス
{
    /// <summary>
    /// 描画図形クラスの基底クラス
    /// </summary>
    internal class Object2D_DrawFigure
    {
        /// <summary>
        /// 再描画領域のオフセット値（ピクセル単位）
        /// </summary>
        public const int DEF_INVALIDATE_OFFSET = 25;
    }
}
