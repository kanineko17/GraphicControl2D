using System;
using System.Collections.Generic;
using System.Drawing;

namespace graphicbox2d.オブジェクトマネージャー_旧
{
    /// <summary>
    /// フォントマネージャー
    /// フォントは毎回 new すると負荷がかかるため、
    /// 一度作ったフォントをキャッシュして再利用できるようにしたクラス。
    /// </summary>
    internal class FontManager : IDisposable
    {
        /// <summary>
        /// フォントキャッシュ
        /// </summary>
        private readonly Dictionary<FontKey, Font> _FontCache = new Dictionary<FontKey, Font>();

        /// <summary>
        /// 破棄済フラグ
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// フォントを取得する
        /// </summary>
        /// <param name="FontName">フォント名</param>
        /// <param name="FontSize">サイズ</param>
        /// <param name="style">スタイル（省略可）</param>
        /// <returns>フォント</returns>
        public Font GetFont(string FontName, float FontSize, FontStyle style = FontStyle.Regular)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FontManager));

            var key = new FontKey(FontName, FontSize, style);

            if (_FontCache.TryGetValue(key, out Font font) == false)
            {
                font = new Font(FontName, FontSize, style);
                _FontCache[key] = font;
            }

            return font;
        }

        /// <summary>
        /// 全フォント破棄
        /// </summary>
        public void Clear()
        {
            foreach (Font font in _FontCache.Values)
            {
                font.Dispose();
            }
            _FontCache.Clear();
        }

        /// <summary>
        /// Disposable 実装
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            Clear();
            _disposed = true;
        }
    }

    /// <summary>
    /// フォントキャッシュ用のキーを表す構造体。
    /// フォント名・サイズ・スタイルの組み合わせで一意に識別する。
    /// </summary>
    internal struct FontKey
    {
        /// <summary>
        /// フォント名（例: "Arial", "MS Gothic"）
        /// </summary>
        public string FontName { get; }

        /// <summary>
        /// フォントサイズ（ポイント単位）
        /// </summary>
        public float FontSize { get; }

        /// <summary>
        /// フォントスタイル（Bold, Italicなど）
        /// </summary>
        public FontStyle Style { get; }

        /// <summary>
        /// コンストラクタ。フォント名・サイズ・スタイルを指定して初期化する。
        /// </summary>
        public FontKey(string familyName, float emSize, FontStyle style)
        {
            FontName = familyName;
            FontSize = emSize;
            Style = style;
        }

        /// <summary>
        /// 他の FontKey と比較し、同じフォント名・サイズ・スタイルなら true を返す。
        /// </summary>
        public bool Equals(FontKey other)
        {
            return FontName == other.FontName &&
                   FontSize.Equals(other.FontSize) &&
                   Style == other.Style;
        }

        /// <summary>
        /// object 型からキャストして比較するオーバーライド。
        /// FontKey 型であれば Equals(FontKey) を利用する。
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is FontKey other && Equals(other);
        }

        /// <summary>
        /// ハッシュコードを生成する。
        /// フォント名・サイズ・スタイルを組み合わせて一意性を確保。
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(FontName, FontSize, Style);
        }
    }
}