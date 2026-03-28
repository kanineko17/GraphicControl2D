using graphicbox2d.オブジェクトマネージャー;
using graphicbox2d.グローバル変数;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static graphicbox2d.グラフィック計算.CalConvert;

namespace graphicbox2d.グラフィック計算
{
    internal class CalText
    {

        /// <summary>
        /// 指定したテキストと SKFont を使用して、
        /// 描画時に必要となるテキストの境界矩形（バウンディングボックス）を取得する。
        /// </summary>
        /// <param name="text">計測対象の文字列</param>
        /// <param name="font">使用する SKFont（呼び出し元で生成済み）</param>
        /// <returns>テキストの描画領域を表す SKRect</returns>
        /// <remarks>
        /// ・SKTextBlob を生成して Bounds から矩形を取得  
        /// ・font と SKTextBlob は finally で確実に Dispose  
        /// ・改行を含む場合は SKTextBlob が行単位で処理するため、
        ///   実際の描画位置と一致する矩形が得られる  
        /// </remarks>
        public static SizeF GetTextSize(string text, SKFont font, eCalculateType calculateType)
        {
            SKRect textRect;

            float width = font.MeasureText(text, out textRect);

            float height = textRect.Height;

            if (calculateType == eCalculateType.Grid)
            {
                width = ConvertClientLengthToDisplayGridLength((int)width);
                height = ConvertClientLengthToDisplayGridLength((int)height);
            }

            SizeF size = new SizeF(width, height);

            return size;
        }

        /// <summary>
        /// フォント名とサイズを指定して SKFont を生成し、
        /// テキストの描画領域（バウンディングボックス）を取得する。
        /// </summary>
        /// <param name="text">計測対象の文字列</param>
        /// <param name="fontSize">フォントサイズ（px）</param>
        /// <param name="fontName">フォント名</param>
        /// <returns>テキストの描画領域を表す SKRect</returns>
        /// <remarks>
        /// ・DrawManager.GetSKFont により SKFont を生成  
        /// ・SKTextBlob を使用して Bounds を取得  
        /// ・生成した font と SKTextBlob は finally で Dispose  
        /// ・フォント指定でのテキスト計測に便利なオーバーロード  
        /// </remarks>
        public static SizeF GetTextSize(string text, Font font, eCalculateType calculateType)
        {
            SKFont skFont = DrawManager.ConvertFontToSKFont(font);

            try
            {
                return GetTextSize(text, skFont, calculateType);
            }
            finally
            {
                skFont.Dispose();
            }
        }
    }
}
