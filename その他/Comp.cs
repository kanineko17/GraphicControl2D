using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 小数型の比較クラス。
    /// </summary>
    internal class Comp
    {
        /// <summary>
        /// double 型の許容誤差
        /// </summary>
        private const double DEFAULT_EPSILON_DOUBLE = 1e-10;
        private const double DEFAULT_POW_EPSILON_DOUBLE = 1e-5;

        /// <summary>
        /// float 型の許容誤差
        /// </summary>
        private const float DEFAULT_EPSILON_FLOAT = 1e-10F;
        private const float DEFAULT_POW_EPSILON_FLOAT = 1e-5F;

        /// <summary>
        /// a = b か判定
        /// </summary>
        /// <param name="a">比較対象の値1</param>
        /// <param name="b">比較対象の値2</param>
        /// <param name="epsilon">許容誤差（既定値: DEFAULT_POW_EPSILON_DOUBLE）</param>
        /// <returns>
        /// 等しいと判定される場合は <c>true</c>、それ以外は <c>false</c>。
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEqual(double a, double b, double epsilon = DEFAULT_POW_EPSILON_DOUBLE)
        {
            return a * a + b * b < epsilon * epsilon;
        }

        /// <summary>
        /// a ≧ b か判定
        /// </summary>
        /// <param name="a">比較対象の値1</param>
        /// <param name="b">比較対象の値2</param>
        /// <param name="epsilon">許容誤差（既定値: DEFAULT_EPSILON_DOUBLE）</param>
        /// <returns>
        /// a ≧ b と判定される場合は <c>true</c>、それ以外は <c>false</c>。
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAtLeast(double a, double b, double epsilon = DEFAULT_EPSILON_DOUBLE)
        {
            return a > b - epsilon;
        }

        /// <summary>
        /// a ≦ b か判定
        /// </summary>
        /// <param name="a">比較対象の値1</param>
        /// <param name="b">比較対象の値2</param>
        /// <param name="epsilon">許容誤差（既定値: DEFAULT_EPSILON_DOUBLE）</param>
        /// <returns>
        /// a ≦ b と判定される場合は <c>true</c>、それ以外は <c>false</c>。
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAtMost(double a, double b, double epsilon = DEFAULT_EPSILON_DOUBLE)
        {
            return a < b + epsilon;
        }

        /// <summary>
        /// a = b か判定
        /// </summary>
        /// <param name="a">比較対象の値1</param>
        /// <param name="b">比較対象の値2</param>
        /// <param name="epsilon">許容誤差（既定値: DEFAULT_POW_EPSILON_FLOAT）</param>
        /// <returns>
        /// 等しいと判定される場合は <c>true</c>、それ以外は <c>false</c>。
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEqual(float a, float b, float epsilon = DEFAULT_POW_EPSILON_FLOAT)
        {
            return a * a + b * b < epsilon * epsilon;
        }

        /// <summary>
        /// a ≧ b か判定
        /// </summary>
        /// <param name="a">比較対象の値1</param>
        /// <param name="b">比較対象の値2</param>
        /// <param name="epsilon">許容誤差（既定値: DEFAULT_EPSILON_FLOAT）</param>
        /// <returns>
        /// a ≧ b と判定される場合は <c>true</c>、それ以外は <c>false</c>。
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAtLeast(float a, float b, float epsilon = DEFAULT_EPSILON_FLOAT)
        {
            return a > b - epsilon;
        }

        /// <summary>
        /// a ≦ b か判定
        /// </summary>
        /// <param name="a">比較対象の値1</param>
        /// <param name="b">比較対象の値2</param>
        /// <param name="epsilon">許容誤差（既定値: DEFAULT_EPSILON_FLOAT）</param>
        /// <returns>
        /// a ≦ b と判定される場合は <c>true</c>、それ以外は <c>false</c>。
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAtMost(float a, float b, float epsilon = DEFAULT_EPSILON_FLOAT)
        {
            return a < b + epsilon;
        }

        /// <summary>
        /// 2D ベクトルの等しい判定
        /// </summary>
        /// <param name="v1">比較対象のベクトル1</param>
        /// <param name="v2">比較対象のベクトル2</param>
        /// <param name="epsilon">許容誤差（既定値: DEFAULT_POW_EPSILON_FLOAT）</param>
        /// <returns>
        /// 等しいと判定される場合は <c>true</c>、それ以外は <c>false</c>。
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AlmostEqual(in Vector2 v1, in Vector2 v2, float epsilon = DEFAULT_POW_EPSILON_FLOAT)
        {
            float dx = v1.X - v2.X;
            float dy = v1.Y - v2.Y;
            float sqDistance = dx * dx + dy * dy;
            return sqDistance < epsilon * epsilon;
        }
    }
}
