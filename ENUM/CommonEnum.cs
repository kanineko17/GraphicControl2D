using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.ENUM
{
    /// <summary>
    /// 矢印付き線分の線端タイプ
    /// </summary>
    public enum Arrow2DLineCapType
    {
        /// <summary>矢印なし</summary>
        None,
        /// <summary>通常の矢印</summary>
        Arrow,
    }

    /// <summary>
    /// 再描画の種類を表す列挙型
    /// </summary>
    public enum eInvalidateType
    {
        /// <summary>画面全体を再描画</summary>
        Full,
        /// <summary>指定した矩形領域を再描画</summary>
        Rect,
        /// <summary>再描画しない</summary>
        None
    }
}
