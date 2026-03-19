using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace graphicbox2d.オブジェクトマネージャー
{
    internal static class DrawManager
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

        internal static readonly SKFontManager SKFontManager = new SKFontManager();

        /// <summary>
        /// 線種パターンをまとめたディクショナリ
        /// </summary>
        private static readonly Dictionary<LineStyle, float[]> DashPatterns = new Dictionary<LineStyle, float[]>
        {
            { LineStyle.Dash,           new float[] { 8, 4 } },             // ダッシュ
            { LineStyle.Dot,            new float[] { 1, 3 } },             // ドット
            { LineStyle.DashDot,        new float[] { 8, 4, 1, 4 } },       // ダッシュドット
            { LineStyle.DashDotDot,     new float[] { 8, 4, 1, 2, 1, 4 } }, // ダッシュドットドット
            { LineStyle.SelectBoxDash,  new float[] { 3, 5 } },             // セレクトボックス線
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static DrawManager()
        {



        }

        /// <summary>
        /// 指定した色・太さ・線種に基づいて、線描画用の SKPaint を生成します。
        /// </summary>
        /// <param name="color">線の色。</param>
        /// <param name="width">線の太さ（デフォルト: 1.0f）。</param>
        /// <param name="IsAntiAlias">アンチエイリアスを有効にするかどうか。</param>
        /// <param name="LineStyle">線種（実線・破線・カスタムパターンなど）。</param>
        /// <param name="customLineStyle">
        /// カスタム線種を使用する場合のパターン配列。null または空の場合は無視されます。
        /// </param>
        /// <param name="customDashPhase">カスタム線種の開始位置（位相）。</param>
        /// <returns>設定済みの SKPaint オブジェクト。</returns>

        internal static SKPaint GetLineSkPaint(Color color, float width = 1.0f, bool IsAntiAlias = true,
            LineStyle LineStyle = LineStyle.Solid,
            float[] customLineStyle = default,
            float customDashPhase = 0)
        {
            SKPaint sKPaint = new SKPaint();
            sKPaint.Style = SKPaintStyle.Stroke;
            sKPaint.IsStroke = true;

            // 基本設定
            sKPaint.Color = new SKColor(color.R, color.G, color.B, color.A);
            sKPaint.StrokeWidth = width;
            sKPaint.IsAntialias = IsAntiAlias;

            switch (LineStyle)
            {
                // 実線
                case LineStyle.Solid:
                    sKPaint.PathEffect = null;
                    break;
                // カスタムパターン
                case LineStyle.Custom:
                    if (customLineStyle != null && customLineStyle.Length > 0)
                        sKPaint.PathEffect = SKPathEffect.CreateDash(customLineStyle, customDashPhase);
                    else
                        sKPaint.PathEffect = null;
                    break;
                // 既定のパターン
                default:
                    if (DashPatterns.TryGetValue(LineStyle, out var pattern) == true)
                    {
                        sKPaint.PathEffect = SKPathEffect.CreateDash(pattern, 0);
                    }
                    else
                    {
                        sKPaint.PathEffect = null;
                    }
                    break;
            }

            return sKPaint;
        }

        /// <summary>
        /// 指定した色で塗りつぶし用の SKPaint を生成します。
        /// </summary>
        /// <param name="color">塗りつぶし色。</param>
        /// <returns>設定済みの SKPaint オブジェクト。</returns>

        internal static SKPaint GetFillSkPaint(Color color)
        {
            SKPaint sKPaint = new SKPaint();
            sKPaint.Style = SKPaintStyle.Fill;
            sKPaint.IsStroke = false;

            // 基本設定
            sKPaint.Color = new SKColor(color.R, color.G, color.B, color.A);

            return sKPaint;
        }

        internal static SKPaint GetTextSkPaint(Color color)
        {
            SKPaint TextSkPaint = new SKPaint();
            TextSkPaint.IsAntialias = true;
            TextSkPaint.Style = SKPaintStyle.Fill;
            TextSkPaint.IsStroke = false;

            // 基本設定
            TextSkPaint.Color = new SKColor(color.R, color.G, color.B, color.A);

            return TextSkPaint;
        }

        /// <summary>
        /// 指定した色で文字描画用の SKPaint を生成します。
        /// </summary>
        /// <param name="color">文字色。</param>
        /// <returns>設定済みの SKPaint オブジェクト。</returns>

        internal static SKFont GetSKFont(string FontName, float FontSize, SKFontStyle sKFontStyle = null)
        {
            return SKFontManager.GetFont(FontName, FontSize, sKFontStyle);
        }

        /// <summary>
        /// フォントをSKFontに変換する
        /// ※SkiaSharpのフォントサイズはポイントではなくピクセルで指定する必要があるため、1.33倍している（一般的なDPI設定での換算）。必要に応じて調整してください。
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        internal static SKFont ConvertFontToSKFont(Font font)
        {
            return SKFontManager.GetFont(font.Name, font.Size * 1.33f, font.Style.ToSKFontStyle());
        }

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