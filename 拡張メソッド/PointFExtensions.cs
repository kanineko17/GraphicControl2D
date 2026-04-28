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
    /// PointF 型の拡張メソッドを提供します。
    /// </summary>
    public static class PointFExtensions
    {
        /// <summary>
        /// PointF を Vector2 に変換します。
        /// </summary>
        /// <param name="p">変換対象の PointF。</param>
        /// <returns>変換後の Vector2。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this PointF p)
        {
            return new Vector2(p.X, p.Y);
        }

        /// <summary>
        /// 2つの PointF 間の距離を計算します。
        /// </summary>
        /// <param name="p1">距離を計算する最初の点。</param>
        /// <param name="p2">距離を計算する2番目の点。</param>
        /// <returns>2つの点間の距離。</returns>
        public static float DistanceTo(this PointF p1, PointF p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// PointF と指定された座標 (x, y) 間の距離を計算します。
        /// </summary>
        /// <param name="p1">距離を計算する最初の点。</param>
        /// <param name="x">距離を計算する2番目の点のX座標。</param>
        /// <param name="y">距離を計算する2番目の点のY座標。</param>
        /// <returns>2つの点間の距離。</returns>
        public static float DistanceTo(this PointF p1, float x, float y)
        {
            float dx = x - p1.X;
            float dy = y - p1.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
