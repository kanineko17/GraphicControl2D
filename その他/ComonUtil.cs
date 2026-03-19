using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 汎用関数クラス
    /// </summary>
    static class ComonUtil
    {

        /// <summary>
        /// 指定した整数を左にビットシフトする
        /// </summary>
        /// <param name="Target">シフト対象の整数値</param>
        /// <param name="ShiftNum">シフトするビット数</param>
        /// <returns>左シフト後の整数値</returns>
        public static int BitShiftLeft(int Target, short ShiftNum)
        {
            int Result = Target << ShiftNum;

            return Result;
        }

        /// <summary>
        /// 指定した整数を右にビットシフトする
        /// </summary>
        /// <param name="Target">シフト対象の整数値</param>
        /// <param name="ShiftNum">シフトするビット数</param>
        /// <returns>右シフト後の整数値</returns>
        public static int BitShiftRight(int Target, short ShiftNum)
        {
            int Result = Target >> ShiftNum;

            return Result;
        }

        /// <summary>
        /// 32ビット値の上位16ビットを取得する。
        /// </summary>
        /// <param name="Target">32bit値</param>
        /// <returns>上位16ビット</returns>
        public static int GetUpper16Bit(int Target)
        {
            // 16bit右にシフト
            int Result = BitShiftRight(Target, 16);

            // 上位16ビットを削除して返す
            Result = Result & 0xFFFF;

            return Result; 
        }

        /// <summary>
        /// 32ビット値の下位16ビットを取得する
        /// </summary>
        /// <param name="Target">32bit値</param>
        /// <returns>下位16ビット/returns>
        public static int GetLower16Bit(int Target)
        {
            // 上位16ビットを削除して返す
            int Result = Target & 0xFFFF;

            return Result;
        }

        /// <summary>
        /// 32ビット値の下位8ビットを取得する
        /// </summary>
        /// <param name="Target">32bit値</param>
        /// <returns>下位8ビット/returns>
        public static int GetLower8Bit(int Target)
        {
            int Result = Target & 0x78;

            return Result;
        }

    public static SKBitmap ImageToSKBitmap(Image image)
    {
        using (var ms = new MemoryStream())
        {
            // PNG 形式でメモリに保存
            image.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);

            // SkiaSharp で読み込み
            return SKBitmap.Decode(ms);
        }
    }

        public static SKBitmap BitmapToSKBitmap(Bitmap bitmap)
        {
            var skBitmap = new SKBitmap(bitmap.Width, bitmap.Height, SKColorType.Bgra8888, SKAlphaType.Premul);

            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppPArgb);

            skBitmap.InstallPixels(
                new SKImageInfo(bitmap.Width, bitmap.Height, SKColorType.Bgra8888, SKAlphaType.Premul),
                data.Scan0,
                data.Stride);

            bitmap.UnlockBits(data);

            return skBitmap;
        }


    }
}
