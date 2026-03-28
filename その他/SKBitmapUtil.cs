using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace graphicbox2d.その他
{
    internal class SKBitmapUtil
    {
        public static SKBitmap MakeScaleBitMap(SKBitmap src, float Scale)
        {
            if (src == null)
            {
                return null;
            }

            int width = (int)Math.Round(src.Width * Scale);
            int height = (int)Math.Round(src.Height * Scale);

            return MakeResizeBitMap(src, width, height);
        }

        public static SKBitmap MakeResizeBitMap(SKBitmap src, int width, int height)
        {
            if (src == null)
            {
                return null;
            }

            var resized = new SKBitmap(width, height, src.ColorType, src.AlphaType);

            using (var surface = new SKCanvas(resized))
            {
                surface.Clear(SKColors.Transparent);

                var destRect = new SKRect(0, 0, width, height);
                var srcRect = new SKRect(0, 0, src.Width, src.Height);

                surface.DrawBitmap(src, srcRect, destRect, new SKPaint
                {
                    IsAntialias = true
                });
            }

            return resized;
        }
    }
}
