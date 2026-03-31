using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.描画図形クラス
{
    /// <summary>
    /// グループ図形描画に必要な情報をまとめたクラス
    /// </summary>
    internal class Group2D_DrawFigure
    {
        /// <summary>
        /// 選択ボックスの頂点リスト（クライアント座標）
        /// </summary>
        public SKPoint[] BoundingBoxPoints { get; set; }
    }
}
