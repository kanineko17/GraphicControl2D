using System;
using System.Collections.Generic;
using SkiaSharp;

namespace graphicbox2d
{
    /// <summary>
    /// SKFont マネージャー
    /// Typeface と Font をキャッシュして再利用できるようにしたクラス。
    /// </summary>
    internal class SKFontManager : IDisposable
    {
        private readonly Dictionary<TypefaceKey, SKTypeface> _TypefaceCache = new Dictionary<TypefaceKey, SKTypeface>();
        private readonly Dictionary<SKFontKey, SKFont> _FontCache = new Dictionary<SKFontKey, SKFont>();
        private bool _disposed = false;

        /// <summary>
        /// SKFont を取得する
        /// </summary>
        /// <param name="fontName">フォント名</param>
        /// <param name="fontSize">サイズ</param>
        /// <param name="style">スタイル（省略可）</param>
        /// <returns>SKFont</returns>
        public SKFont GetFont(string fontName, float fontSize, SKFontStyle style = null)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(SKFontManager));

            var typefaceKey = new TypefaceKey(fontName, style);
            if (!_TypefaceCache.TryGetValue(typefaceKey, out SKTypeface typeface))
            {
                typeface = style == null
                    ? SKTypeface.FromFamilyName(fontName)
                    : SKTypeface.FromFamilyName(fontName, style);

                _TypefaceCache[typefaceKey] = typeface;
            }

            var fontKey = new SKFontKey(typefaceKey, fontSize);
            if (!_FontCache.TryGetValue(fontKey, out SKFont font))
            {
                font = new SKFont(typeface, fontSize);
                _FontCache[fontKey] = font;
            }

            return font;
        }

        /// <summary>
        /// 全キャッシュ破棄
        /// </summary>
        public void Clear()
        {
            foreach (var font in _FontCache.Values)
            {
                font.Dispose();
            }
            _FontCache.Clear();

            foreach (var typeface in _TypefaceCache.Values)
            {
                typeface.Dispose();
            }
            _TypefaceCache.Clear();
        }

        public void Dispose()
        {
            if (_disposed) return;
            Clear();
            _disposed = true;
        }
    }

    /// <summary>
    /// Typeface キャッシュ用キー
    /// </summary>
    internal struct TypefaceKey
    {
        public string FontName { get; }
        public SKFontStyle Style { get; }

        public TypefaceKey(string fontName, SKFontStyle style)
        {
            FontName = fontName;
            Style = style;
        }

        public bool Equals(TypefaceKey other)
        {
            return FontName == other.FontName && Equals(Style, other.Style);
        }

        public override bool Equals(object obj) => obj is TypefaceKey other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(FontName, Style);
    }

    /// <summary>
    /// Font キャッシュ用キー
    /// </summary>
    internal struct SKFontKey
    {
        public TypefaceKey TypefaceKey { get; }
        public float FontSize { get; }

        public SKFontKey(TypefaceKey typefaceKey, float fontSize)
        {
            TypefaceKey = typefaceKey;
            FontSize = fontSize;
        }

        public bool Equals(SKFontKey other)
        {
            return TypefaceKey.Equals(other.TypefaceKey) && FontSize.Equals(other.FontSize);
        }

        public override bool Equals(object obj) => obj is SKFontKey other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(TypefaceKey, FontSize);
    }
}