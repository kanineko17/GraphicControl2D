using graphicbox2d.オブジェクトマネージャー_旧;
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
        /// SKTypefaceマネージャー
        /// </summary>
        internal static readonly SKTypefaceManager SKTypeface = new SKTypefaceManager();

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
        /// 指定したフォント名・サイズ・スタイルで SKFont を生成します。
        /// </summary>
        /// <param name="FontName">フォント名。</param>
        /// <param name="FontSize">フォントサイズ。</param>
        /// <param name="sKFontStyle">フォントスタイル。</param>
        /// <returns>設定済みの SKFont オブジェクト。</returns>
        internal static SKFont GetSKFont(string FontName, float FontSize, SKFontStyle sKFontStyle = null)
        {
            SKTypeface typeface = SKTypeface.GetSKTypeface(FontName, sKFontStyle);

            SKFont skFont = new SKFont(typeface, FontSize);

            return skFont;
        }

        /// <summary>
        /// フォントをSKFontに変換する
        /// ※SkiaSharpのフォントサイズはポイントではなくピクセルで指定する必要があるため、1.33倍している（一般的なDPI設定での換算）。必要に応じて調整してください。
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        internal static SKFont ConvertFontToSKFont(Font font)
        {
            SKTypeface typeface = SKTypeface.GetSKTypeface(font.Name, font.Style.ToSKFontStyle());

            SKFont skFont = new SKFont(typeface, font.Size * 1.33f);

            return skFont;
        }
    }
}