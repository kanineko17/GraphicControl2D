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
        /// <returns>下位16ビット</returns>
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
        /// <returns>下位8ビット</returns>
        public static int GetLower8Bit(int Target)
        {
            int Result = Target & 0x78;

            return Result;
        }
    }
}
