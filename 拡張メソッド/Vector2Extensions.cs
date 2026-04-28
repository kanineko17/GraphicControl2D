using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// Vector2 型の拡張メソッドを提供します。
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Vector2 を PointF に変換します。
        /// </summary>
        /// <param name="v">変換対象の Vector2。</param>
        /// <returns>変換後の PointF。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ToPointF(this Vector2 v)
        {
            return new PointF(v.X, v.Y);
        }

        /// <summary>
        /// Vector2 を Point に変換します。
        /// </summary>
        /// <param name="v">変換対象の Vector2。</param>
        /// <returns>変換後の Point。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point ToPoint(this Vector2 v)
        {
            return new Point((int)v.X, (int)v.Y);
        }

        /// <summary>
        /// Vector2 を SKPoint に変換する
        /// </summary>
        /// <param name="v">変換元のベクトル</param>
        /// <returns>SKPoint</returns>
        internal static SKPoint ToSKPoint(this Vector2 v)
        {
            return new SKPoint(v.X, v.Y);
        }

    }
}
