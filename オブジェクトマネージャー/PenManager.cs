using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace graphicbox2d
{
    /// <summary>
    /// ペンマネージャー
    /// ペンは色や太さを指定するたびに新規作成が必要になる。
    /// 毎回生成・破棄すると描画処理に負荷がかかるため、
    /// 一度作ったペンをキャッシュして再利用できるようにしたクラス。
    /// </summary>
    internal class PenManager : IDisposable
    {
        /// <summary>
        /// ペンキャッシュ
        /// </summary>
        private readonly Dictionary<PenKey, Pen> _PenCache = new Dictionary<PenKey, Pen>();

        /// <summary>
        /// 破棄済フラグ
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// ペンを取得する
        /// </summary>
        /// <param name="color">色</param>
        /// <param name="width">太さ</param>
        /// <param name="dashStyle">線種</param>
        /// <param name="dashPattern">カスタムパターン（LineStyle.Custom時のみ有効）</param>
        /// <returns>ペン</returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public Pen GetPen(Color color, float width = 1.0f,
                          DashStyle dashStyle = DashStyle.Solid,
                          float[] dashPattern = null)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(PenManager));

            var key = new PenKey(color, width, dashStyle, dashPattern);

            if (_PenCache.TryGetValue(key, out Pen pen) == false)
            {
                pen = new Pen(color, width)
                {
                    DashStyle = dashStyle
                };

                if (dashStyle == DashStyle.Custom && dashPattern != null)
                {
                    pen.DashPattern = dashPattern;
                }

                _PenCache[key] = pen;
            }

            return pen;
        }

        /// <summary>
        /// 全ペン破棄
        /// </summary>
        public void Clear()
        {
            foreach (Pen pen in _PenCache.Values)
            {
                pen.Dispose();
            }
            _PenCache.Clear();
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
    /// ペンキャッシュ用キー
    /// </summary>
    internal struct PenKey : IEquatable<PenKey>
    {
        public Color Color { get; }
        public float Width { get; }
        public DashStyle DashStyle { get; }
        public float[] DashPattern { get; }

        public PenKey(Color color, float width, DashStyle dashStyle, float[] dashPattern)
        {
            Color = color;
            Width = width;
            DashStyle = dashStyle;
            DashPattern = dashPattern ?? Array.Empty<float>();
        }

        public bool Equals(PenKey other)
        {
            if (!Color.Equals(other.Color) ||
                !Width.Equals(other.Width) ||
                !DashStyle.Equals(other.DashStyle))
                return false;

            if (DashPattern.Length != other.DashPattern.Length)
                return false;

            for (int i = 0; i < DashPattern.Length; i++)
            {
                if (!DashPattern[i].Equals(other.DashPattern[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is PenKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            int hash = HashCode.Combine(Color, Width, DashStyle);
            foreach (var v in DashPattern)
            {
                hash = HashCode.Combine(hash, v);
            }
            return hash;
        }
    }
}