using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.グローバル変数
{
    internal static class Global
    {
        /// <summary>
        /// グラフィック2Dコントロールオブジェクト
        /// </summary>
        public static Graphic2DControl Graphic2DControl { get; set; }

        /// <summary>
        /// グラフィック描画エンジンオブジェクト
        /// </summary>
        public static GraphicDrawEngine GraphicDrawEngine { get; set; }
    }
}
