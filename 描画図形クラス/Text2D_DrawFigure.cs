using graphicbox2d.グラフィック計算;
using graphicbox2d.オブジェクトマネージャー;
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
    /// テキスト図形描画に必要な情報をまとめたクラス
    /// </summary>
    internal class Text2D_DrawFigure : IDrawFigure
    {
        /// <summary>
        /// 描画座標（クライアント座標）
        /// </summary>
        public SKPoint ClientPoint { get; set; }

        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// テキストの角度（度数法）
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// フォント名
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// 描画時に使用するフォントサイズ
        /// </summary>
        public float FontSize { get; set; }

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
            using SKFont font = DrawManager.GetSKFont(FontName, FontSize);
            return CalBoundBox.GetBoundingBoxTextSK(
                ClientPoint.X,
                ClientPoint.Y,
                font,
                Text,
                Angle,
                eCalculateType.Client);
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
