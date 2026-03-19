using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graphicbox2d
{
    /// <summary>
    /// マウスカーソルクラス
    /// </summary>
    internal static class MouseCursors
    {
        /// <summary>
        /// デフォルトモードカーソル
        /// </summary>
        public static Cursor NoneModeCursor = Cursors.Default;

        /// <summary>
        /// セレクトモードカーソル
        /// </summary>
        public static Cursor SelectModeCursor = Cursors.Default;

        /// <summary>
        /// セレクトドラッグ操作中カーソル
        /// </summary>
        public static Cursor SelectModeDraggingCursor = Cursors.Hand;
    }
}
