using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.オブジェクトマネージャー_旧
{
    /// <summary>
    /// ブラシマネージャー
    /// ブラシは、色を指定するたびに、オブジェクトを新規作成しなければならない。
    /// 描画のたびに指定の色のブラシオブジェクトを新規作成・解放すると、描画処理に非常に負荷がかかってしまう。
    /// このクラスは、一度作ったブラシはキャッシュとして保存して再利用できるようにしたクラスである。
    /// </summary>
    internal class BrushManager : IDisposable
    {
        /// <summary>
        /// ソリッドブラシキャッシュ
        /// </summary>
        private readonly Dictionary<SolidBrushKey, Brush> _SolidBrushCache = new Dictionary<SolidBrushKey, Brush>();

        /// <summary>
        /// ハッチブラシキャッシュ
        /// </summary>
        private readonly Dictionary<HatchBrushKey, Brush> _HatchBrushCache = new Dictionary<HatchBrushKey, Brush>();

        /// <summary>
        /// 破棄済フラグ
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// ソリッドブラシを取得する
        /// </summary>
        /// <param name="color"></param>
        /// <returns>ソリッドブラシ</returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public SolidBrush GetSolidBrush(Color color)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(BrushManager));

            var key = new SolidBrushKey(color);

            if (_SolidBrushCache.TryGetValue(key, out Brush brush) == false)
            {
                brush = new SolidBrush(color);
                _SolidBrushCache[key] = brush;
            }

            return (SolidBrush)brush;
        }

        /// <summary>
        /// ハッチブラシを取得する
        /// </summary>
        /// <param name="style">ハッチスタイル</param>
        /// <param name="foreColor">カラー</param>
        /// <param name="backColor">背景色</param>
        /// <returns>ハッチブラシ</returns>
        public HatchBrush GetHatchBrush(HatchStyle style, Color foreColor, Color backColor)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(BrushManager));

            var key = new HatchBrushKey(style, foreColor, backColor);

            if (_HatchBrushCache.TryGetValue(key, out Brush brush) == false)
            {
                brush = new HatchBrush(style, foreColor, backColor);
                _HatchBrushCache[key] = brush;
            }

            return (HatchBrush)brush;
        }

        /// <summary>
        /// 全ブラシ破棄
        /// </summary>
        public void Clear()
        {
            foreach (Brush brush in _SolidBrushCache.Values)
            {
                brush.Dispose();
            }

            _SolidBrushCache.Clear();

            foreach (Brush brush in _HatchBrushCache.Values)
            {
                brush.Dispose();
            }

            _HatchBrushCache.Clear();
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
    /// Solid のキャッシュ識別用キー
    /// </summary>
    internal struct SolidBrushKey
    {
        /// <summary>
        /// カラー
        /// </summary>
        public readonly Color Color;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="color">カラー</param>
        public SolidBrushKey(Color color)
        {
            Color = color;
        }

        /// <summary>
        /// SolidBrushKey の等価比較を行う
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト</param>
        /// <returns>
        /// true : Color が一致している場合  
        /// false: それ以外の場合
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is SolidBrushKey other)
            {
                return Color.Equals(other.Color);
            }
            return false;
        }

        /// <summary>
        /// SolidBrushKey のハッシュコードを生成する
        /// </summary>
        /// <returns>
        /// Color を基にした一意なハッシュ値  
        /// （キャッシュのキーとして利用可能）
        /// </returns>
        public override int GetHashCode()
        {
            // Color 構造体は ARGB 値を持っているので、そのハッシュを利用
            return Color.GetHashCode();
        }
    }

    /// <summary>
    /// HatchBrush のキャッシュ識別用キー
    /// </summary>
    internal struct HatchBrushKey
    {
        /// <summary>
        /// ハッチスタイル
        /// </summary>
        public readonly HatchStyle HatchStyle;

        /// <summary>
        /// カラー
        /// </summary>
        public readonly Color ForeColor;

        /// <summary>
        /// 背景色
        /// </summary>
        public readonly Color BackColor;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="style">ハッチスタイル</param>
        /// <param name="fore">カラー</param>
        /// <param name="back">背景色</param>
        public HatchBrushKey(HatchStyle style, Color fore, Color back)
        {
            HatchStyle = style;
            ForeColor = fore;
            BackColor = back;
        }

        /// <summary>
        /// HatchBrushKey の等価比較を行う
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト</param>
        /// <returns>
        /// true : HatchStyle, ForeColor, BackColor がすべて一致している場合  
        /// false: それ以外の場合
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is HatchBrushKey other)
            {
                return HatchStyle == other.HatchStyle &&
                       ForeColor.Equals(other.ForeColor) == true &&
                       BackColor.Equals(other.BackColor) == true;
            }

            return false;
        }

        /// <summary>
        /// HatchBrushKey のハッシュコードを生成する
        /// </summary>
        /// <returns>
        /// HatchStyle, ForeColor, BackColor を組み合わせた一意なハッシュ値  
        /// （キャッシュのキーとして利用可能）
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(HatchStyle, ForeColor, BackColor);
        }
    }
}
