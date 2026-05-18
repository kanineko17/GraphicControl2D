using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.拡張メソッド
{
    internal static class SKControlExtension
    {
        private static Rectangle _InvalidateRect = new Rectangle(0, 0, 1, 1);

        /// <summary>
        /// 再描画領域を表す矩形を取得する拡張メソッド
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Rectangle GetInvalidateRect(this SKControl control)
        {
            return _InvalidateRect;
        }

        public static void Invalidate2(this SKControl control, Rectangle rect)
        {
            // 再描画領域を更新
            _InvalidateRect = rect;
            // コントロールを再描画
            control.Invalidate(_InvalidateRect);
        }

        public static void Invalidate2(this SKControl control)
        {
            // 再描画領域を更新（画面全体を再描画）
            _InvalidateRect = new Rectangle(0, 0, control.Width, control.Height);
            // コントロールを再描画
            control.Invalidate();
        }
    }
}
