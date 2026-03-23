using graphicbox2d.オブジェクトマネージャー_旧;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    internal static class DrawManager_Old
    {
        /// <summary>
        /// ペンマネージャー
        /// </summary>
        internal static readonly PenManager Pen = new PenManager();

        /// <summary>
        /// 矢印ペンマネージャー
        /// </summary>
        internal static readonly ArrowPenManager ArrowPen = new ArrowPenManager();

        /// <summary>
        /// ブラシマネージャー
        /// </summary>
        internal static readonly BrushManager Brush = new BrushManager();

        /// <summary>
        /// フォントマネージャー
        /// </summary>
        internal static readonly FontManager Font = new FontManager();

        // 線種パターンをまとめたディクショナリ
        private static readonly Dictionary<LineStyle, float[]> DashPatterns = new Dictionary<LineStyle, float[]>
        {
            { LineStyle.Dash,       new float[] { 8, 4 } },
            { LineStyle.Dot,        new float[] { 1, 3 } },
            { LineStyle.DashDot,    new float[] { 8, 4, 1, 4 } },
            { LineStyle.DashDotDot, new float[] { 8, 4, 1, 2, 1, 4 } }
        };

        /// <summary>
        /// ソリッドブラシを取得する
        /// </summary>
        /// <param name="color"></param>
        /// <returns>ソリッドブラシ</returns>
        /// <exception cref="ObjectDisposedException"></exception>
        internal static SolidBrush GetSolidBrush(Color color)
        {
            return Brush.GetSolidBrush(color);
        }

        /// <summary>
        /// ペンを取得する
        /// </summary>
        /// <param name="color">色</param>
        /// <param name="width">太さ</param>
        /// <param name="dashStyle">線種</param>
        /// <param name="dashPattern">カスタムパターン（LineStyle.Custom時のみ有効）</param>
        /// <returns>ペン</returns>
        internal static Pen GetPen(Color color, float width = 1.0f, DashStyle dashStyle = DashStyle.Solid, float[] dashPattern = null)
        {
            return Pen.GetPen(color, width, dashStyle, dashPattern);
        }

        /// <summary>
        /// ペンを取得する
        /// </summary>
        /// <param name="color">色</param>
        /// <param name="width">太さ</param>
        /// <param name="dashStyle">線種</param>
        /// <param name="startCap">始点キャップ</param>
        /// <param name="endCap">終点キャップ</param>
        /// <param name="dashPattern">カスタムパターン（LineStyle.Custom時のみ有効）</param>
        /// <returns>ペン</returns>
        internal static Pen GetArrowPen(Color color, float width = 1.0f,
                  DashStyle dashStyle = DashStyle.Solid,
                  Arrow2DLineCapType startCap = Arrow2DLineCapType.None,
                  Arrow2DLineCapType endCap = Arrow2DLineCapType.None,
                  float[] dashPattern = null)
        {
            return ArrowPen.GetPen(color, width, dashStyle, startCap, endCap, dashPattern);
        }

        /// <summary>
        /// フォントを取得する
        /// </summary>
        /// <param name="FontName">フォント名</param>
        /// <param name="FontSize">サイズ</param>
        /// <param name="style">スタイル（省略可）</param>
        /// <returns>フォント</returns>
        internal static Font GetFont(string FontName, float FontSize, FontStyle style = FontStyle.Regular)
        {
            return Font.GetFont(FontName, FontSize, style);
        }
    }
}
