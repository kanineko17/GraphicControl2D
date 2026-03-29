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

        public static SKBitmap MakeResizeBitMap(SKBitmap src, int width, int height, float scale)
        {
            int swidth = (int)Math.Round(width * scale);
            int sheight = (int)Math.Round(height * scale);

            SKBitmap resized = MakeResizeBitMap(src, swidth, sheight);

            return resized;
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

        /// <summary>
        /// ビットマップをBase64エンコードされたPNG文字列に変換する。
        /// </summary>
        /// <param name="bitmap">変換対象のSKBitmap</param>
        /// <returns>Base64エンコードされたPNG文字列。bitmapがnullの場合は空文字列。</returns>
        public static string SKBitmapToBase64(SKBitmap bitmap)
        {
            if (bitmap == null)
            {
                return string.Empty;
            }

            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                return Convert.ToBase64String(data.ToArray());
            }
        }

        /// <summary>
        /// Base64エンコードされたPNG文字列をSKBitmapに変換する。
        /// </summary>
        /// <param name="base64">Base64エンコードされたPNG文字列</param>
        /// <returns>デコードされたSKBitmap。base64が空または無効な場合はnull。</returns>
        public static SKBitmap Base64ToSKBitmap(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return null;
            }

            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                return SKBitmap.Decode(bytes);
            }
            catch
            {
                return null;
            }
        }
    }
}
