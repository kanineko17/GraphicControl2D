using System;
using System.Collections.Generic;
using SkiaSharp;

namespace graphicbox2d
{
    /// <summary>
    /// SKTypeface をキャッシュして再利用するためのマネージャークラス。
    /// SKTypeface は生成コストが高いため、同一フォント名・スタイルの組み合わせを
    /// キャッシュして使い回すことでパフォーマンスを向上させる。
    /// </summary>
    internal class SKTypefaceManager : IDisposable
    {
        /// <summary>
        /// Typeface のキャッシュ辞書。
        /// キーは <see cref="TypefaceKey"/>、値は生成済みの <see cref="SKTypeface"/>。
        /// </summary>
        private readonly Dictionary<TypefaceKey, SKTypeface> _TypefaceCache = new Dictionary<TypefaceKey, SKTypeface>();

        /// <summary>
        /// Dispose 済みかどうかを示すフラグ。
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 指定されたフォント名とスタイルに対応する <see cref="SKTypeface"/> を取得する。
        /// 既にキャッシュされている場合はキャッシュを返し、未生成の場合は新規作成してキャッシュに追加する。
        /// </summary>
        /// <param name="fontName">フォント名（例: "Meiryo"）</param>
        /// <param name="style">フォントスタイル（省略可）。null の場合はデフォルトスタイル。</param>
        /// <returns>キャッシュまたは新規生成された <see cref="SKTypeface"/>。</returns>
        /// <exception cref="ObjectDisposedException">マネージャーが既に破棄されている場合。</exception>
        public SKTypeface GetSKTypeface(string fontName, SKFontStyle style = null)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(SKTypefaceManager));
            }


            var typefaceKey = new TypefaceKey(fontName, style);

            if (_TypefaceCache.TryGetValue(typefaceKey, out SKTypeface typeface) == false)
            {
                if (style == null)
                {
                    SKTypeface.FromFamilyName(fontName);
                }
                else
                {
                    SKTypeface.FromFamilyName(fontName, style);
                }
                
                _TypefaceCache[typefaceKey] = typeface;
            }

            return typeface;
        }

        /// <summary>
        /// キャッシュされているすべての <see cref="SKTypeface"/> を破棄し、
        /// キャッシュ辞書をクリアする。
        /// </summary>
        public void Clear()
        {
            foreach (var typeface in _TypefaceCache.Values)
            {
                typeface.Dispose();
            }

            _TypefaceCache.Clear();
        }

        /// <summary>
        /// マネージャーを破棄し、キャッシュされているすべてのリソースを解放する。
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            Clear();
            _disposed = true;
        }
    }

    /// <summary>
    /// <see cref="SKTypeface"/> をキャッシュする際のキーとして使用される構造体。
    /// フォント名とスタイルの組み合わせで一意に識別される。
    /// </summary>
    internal struct TypefaceKey
    {
        /// <summary>
        /// フォント名。
        /// </summary>
        public string FontName { get; }

        /// <summary>
        /// フォントスタイル（null の場合はデフォルトスタイル）。
        /// </summary>
        public SKFontStyle Style { get; }

        /// <summary>
        /// 新しいキーを生成する。
        /// </summary>
        /// <param name="fontName">フォント名。</param>
        /// <param name="style">フォントスタイル。</param>
        public TypefaceKey(string fontName, SKFontStyle style)
        {
            FontName = fontName;
            Style = style;
        }

        /// <summary>
        /// 他の <see cref="TypefaceKey"/> と等価かどうかを判定する。
        /// </summary>
        public bool Equals(TypefaceKey other)
        {
            return FontName == other.FontName && Equals(Style, other.Style);
        }

        /// <summary>
        /// オブジェクトが等しいか判定
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is TypefaceKey == false)
            {
                return false;
            }

            if (Equals((TypefaceKey)obj) == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Hashコード生成
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(FontName, Style);
        }

    }
}