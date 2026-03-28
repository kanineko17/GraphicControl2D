using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.オブジェクトマネージャー_旧
{
    /// <summary>
    /// 矢印キャップのキャッシュを管理するクラス
    /// </summary>
    public class ArrowCapManager : IDisposable
    {
        /// <summary>
        /// ArrowCap キャッシュ
        /// </summary>
        private readonly Dictionary<ArrowCapKey, AdjustableArrowCap> cache = new Dictionary<ArrowCapKey, AdjustableArrowCap>();

        /// <summary>
        /// 破棄済フラグ
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 指定サイズの矢印キャップを取得する。
        /// キャッシュに存在しなければ新規生成して保存。
        /// </summary>
        public AdjustableArrowCap GetArrowCap(int width, int height)
        {
            var key = new ArrowCapKey(width, height);

            if (!cache.TryGetValue(key, out var cap))
            {
                cap = new AdjustableArrowCap(width, height);
                cache[key] = cap;
            }

            return cap;
        }

        /// <summary>
        /// キャッシュを破棄する（Dispose呼び出し）
        /// </summary>
        public void Clear()
        {
            foreach (var cap in cache.Values)
            {
                cap.Dispose();
            }
            cache.Clear();
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
    /// ArrowCapKey 構造体は Dictionary のキーとして利用されるため、
    /// 値の等価比較とハッシュコード生成をオーバーライドしています。
    /// 幅と高さが同じ ArrowCapKey は同一キーとして扱われ、
    /// キャッシュされた AdjustableArrowCap を再利用できるようになります。
    /// </summary>
    internal struct ArrowCapKey
    {
        /// <summary>
        /// 幅
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// 高さ
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        public ArrowCapKey(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Dictionary のキー比較に利用される等価判定。
        /// 幅と高さが一致すれば同一キーとみなします。
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is ArrowCapKey)
            {
                ArrowCapKey other = (ArrowCapKey)obj;

                return Width == other.Width && Height == other.Height;
            }

            return false;
        }

        /// <summary>
        /// Dictionary のキー検索に利用されるハッシュコード。
        /// 幅と高さを組み合わせて一意の値を生成します。
        /// </summary>
        public override int GetHashCode()
        {
            int Up16bit = ComonUtil.BitShiftLeft(Width, 16);

            int low16bit = Height;

            // 幅と高さを組み合わせてハッシュ化
            return Up16bit | low16bit;
        }
    }
}
