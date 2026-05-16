using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.拡張メソッド
{
    internal static class SKCanvasExtensions
    {
        /// <summary>
        /// SKCanvasのクリップ領域を表す矩形を取得する拡張メソッド
        /// </summary>
        /// <param name="canvas">拡張メソッドを呼び出すSKCanvasオブジェクト</param>
        /// <returns>クリップ領域を表すRectangleオブジェクト</returns>
        public static Rectangle GetRectangleBounds(this SKCanvas canvas)
        {
            if (canvas.GetDeviceClipBounds(out SKRectI clipI))
            {
                return new Rectangle(clipI.Left, clipI.Top, clipI.Width, clipI.Height);
            }
            return Rectangle.Empty;
        }
    }
}
