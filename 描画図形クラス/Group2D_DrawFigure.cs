using graphicbox2d.グラフィック計算;
using graphicbox2d.図形クラスインターフェース;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.描画図形クラス
{
    /// <summary>
    /// グループ図形描画に必要な情報をまとめたクラス
    /// </summary>
    internal class Group2D_DrawFigure : IDrawFigure
    {
        /// <summary>
        /// 選択ボックスの頂点リスト（クライアント座標）
        /// </summary>
        public SKPoint[] BoundingBoxPoints { get; set; }

        /// <summary>
        /// クライアント座標のバウンディングボックス
        /// </summary>
        public SKPoint[] BoundingBox => GetBoundingBox();

        /// <summary>
        /// クライアント座標のバウンディングボックスを表す矩形
        /// </summary>
        public Rectangle BoundingBoxRect => GetBoundingBoxRect();

        /// <summary>
        /// クライアント座標のバウンディングボックスを取得する
        /// </summary>
        /// <returns></returns>
        public SKPoint[] GetBoundingBox()
        {
            return BoundingBoxPoints;
        }

        /// <summary>
        /// クライアント座標のバウンディングボックスを表す矩形
        /// </summary>
        /// <returns></returns>
        public Rectangle GetBoundingBoxRect()
        {
            return CalBoundBox.ConvertBoundingBoxToRect(BoundingBox);
        }
    }
}
