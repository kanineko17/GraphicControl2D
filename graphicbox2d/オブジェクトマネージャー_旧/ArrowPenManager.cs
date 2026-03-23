using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace graphicbox2d.オブジェクトマネージャー_旧
{
    /// <summary>
    /// ArrowPenManager
    /// ペンをキャッシュし、StartCap / EndCap も指定可能にしたマネージャー。
    /// 矢印付きの線などを効率的に再利用できる。
    /// </summary>
    internal class ArrowPenManager : IDisposable
    {
        /// <summary>
        /// キャッシュデータ
        /// </summary>
        private readonly Dictionary<ArrowPenKey, Pen> _PenCache = new Dictionary<ArrowPenKey, Pen>();

        /// <summary>
        /// 矢印キャップマネージャー
        /// </summary>
        private readonly ArrowCapManager _ArrowCapManager = new ArrowCapManager();

        /// <summary>
        /// 矢印キャップの基本サイズ
        /// </summary>
        const int ARROW_CAP_SIZE = 3;

        /// <summary>
        /// Disposable フラグ
        /// </summary>
        private bool _disposed = false;

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
        public Pen GetPen(Color color, float width = 1.0f,
                          DashStyle dashStyle = DashStyle.Solid,
                          Arrow2DLineCapType startCap = Arrow2DLineCapType.None,
                          Arrow2DLineCapType endCap = Arrow2DLineCapType.None,
                          float[] dashPattern = null)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ArrowPenManager));

            var key = new ArrowPenKey(color, width, dashStyle, startCap, endCap, dashPattern);

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

                if (startCap == Arrow2DLineCapType.Arrow)
                {
                    pen.CustomStartCap = _ArrowCapManager.GetArrowCap(
                        ARROW_CAP_SIZE + (int)width,
                        ARROW_CAP_SIZE + (int)width);
                }

                if (endCap == Arrow2DLineCapType.Arrow)
                {
                    pen.CustomEndCap = _ArrowCapManager.GetArrowCap(
                        ARROW_CAP_SIZE + (int)width,
                        ARROW_CAP_SIZE + (int)width);
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

            _ArrowCapManager.Dispose();

            _disposed = true;
        }
    }

    /// <summary>
    /// ArrowPen キャッシュ用キー
    /// </summary>
    internal struct ArrowPenKey : IEquatable<ArrowPenKey>
    {
        public Color Color { get; }
        public float Width { get; }
        public DashStyle DashStyle { get; }
        public Arrow2DLineCapType StartCap { get; }
        public Arrow2DLineCapType EndCap { get; }
        public float[] DashPattern { get; }

        public ArrowPenKey(Color color, float width, DashStyle dashStyle,
                           Arrow2DLineCapType startCap, Arrow2DLineCapType endCap,
                           float[] dashPattern)
        {
            Color = color;
            Width = width;
            DashStyle = dashStyle;
            StartCap = startCap;
            EndCap = endCap;
            DashPattern = dashPattern ?? Array.Empty<float>();
        }

        public bool Equals(ArrowPenKey other)
        {
            if (!Color.Equals(other.Color) ||
                !Width.Equals(other.Width) ||
                !DashStyle.Equals(other.DashStyle) ||
                !StartCap.Equals(other.StartCap) ||
                !EndCap.Equals(other.EndCap))
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
            return obj is ArrowPenKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            int hash = HashCode.Combine(Color, Width, DashStyle, StartCap, EndCap);
            foreach (var v in DashPattern)
            {
                hash = HashCode.Combine(hash, v);
            }
            return hash;
        }
    }
}