using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 数式計算クラス
    /// </summary>
    public static class SusikiCalculater
    {
        /// <summary>
        /// 正規表現パターン定義
        /// </summary>
        private const string R_ABS = @"\|";
        private const string R_CHAR = @"a-z";
        private const string R_DIGIT = @"\d";
        private const string R_DOT = @"\.";
        private const string R_START_PAREN = @"\(";
        private const string R_END_PAREN = @"\)";
        private const string R_OPER = @"\+\-\*\/";
        private const string R_MINUS = @"\-";
        private const string R_DIGIT_DOT = @"\d\.";

        // 置換用データ構造
        struct ReplaceItem
        {
            public string Before;
            public string After;

            public ReplaceItem(string _Before, string _After)
            {
                Before = _Before;
                After = _After;
            }
        }

        // 置換文字列リスト
        static private List<ReplaceItem> RepItems = new List<ReplaceItem>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static SusikiCalculater()
        {
            // 置換リスト定義
            RepItems.Add(new ReplaceItem("e", "Math.E"));
            RepItems.Add(new ReplaceItem("ｅ", "Math.E"));
            RepItems.Add(new ReplaceItem("π", "Math.PI"));
            RepItems.Add(new ReplaceItem("×", "*"));
            RepItems.Add(new ReplaceItem("÷", "/"));
            RepItems.Add(new ReplaceItem("＋", "+"));
            RepItems.Add(new ReplaceItem("－", "-"));
            RepItems.Add(new ReplaceItem("Acos", "Math.Acos"));
            RepItems.Add(new ReplaceItem("Asin", "Math.Asin"));
            RepItems.Add(new ReplaceItem("Atan", "Math.Atan"));
            RepItems.Add(new ReplaceItem("Arccos", "Math.Acos"));
            RepItems.Add(new ReplaceItem("Arcsin", "Math.Asin"));
            RepItems.Add(new ReplaceItem("Arctan", "Math.Atan"));
            RepItems.Add(new ReplaceItem("Cos", "Math.Cos"));
            RepItems.Add(new ReplaceItem("Cosh", "Math.Cosh"));
            RepItems.Add(new ReplaceItem("Sin", "Math.Sin"));
            RepItems.Add(new ReplaceItem("Tan", "Math.Tan"));
            RepItems.Add(new ReplaceItem("Sinh", "Math.Sinh"));
            RepItems.Add(new ReplaceItem("Tanh", "Math.Tanh"));
            RepItems.Add(new ReplaceItem("Log", "Math.Log"));
            RepItems.Add(new ReplaceItem("Exp", "Math.Exp"));
            RepItems.Add(new ReplaceItem("Abs", "Math.Abs"));
        }

        /// <summary>
        /// 入力された数式文字列を C# の式に変換し、指定範囲内で計算を行い、
        /// 各ステップごとの座標 (x, y) を PointF 配列として返す。
        /// </summary>
        /// <param name="Susiki">ユーザー入力の数式文字列（例: "2x+3", "√(x)" など）</param>
        /// <param name="Start">計算範囲の開始値</param>
        /// <param name="End">計算範囲の終了値</param>
        /// <param name="CalculateInterval">計算ステップ間隔（x の刻み幅）</param>
        /// <returns>
        /// 計算結果の座標を格納した PointF 配列。
        /// 各要素は (x, y) のペアで、x は入力範囲の値、y は数式の評価結果。
        /// エラーが発生した場合は空の配列を返す。
        /// </returns>
        /// <remarks>
        /// - 内部で ConertSusikiToCSharpSusiki により数式を C# 式へ変換する。
        /// - MakeScript によりスクリプトを生成し、Roslyn C# Script で評価する。
        /// - PointF は構造体なので new で代入している点に注意。
        /// </remarks>

        public static async Task<PointF[]> Caluculate(string Susiki, double Start, double End, double CalculateInterval)
        {
            try
            {
                string CSharpSusiki = ConertSusikiToCSharpSusiki(Susiki);

                string script = MakeScript(CSharpSusiki, Start, End, CalculateInterval);

                var options = ScriptOptions.Default
                    .AddImports("System")
                    .AddReferences(typeof(Math).Assembly,
                                   typeof(System.Drawing.PointF).Assembly,
                                   typeof(object).Assembly)
                    .WithImports("System", "System.Drawing");

                return await CSharpScript.EvaluateAsync<PointF[]>(script, options);
            }
            catch (Exception ex)
            {
                // エラー内容を確認
                Console.WriteLine("エラー発生: " + ex.Message);
                Console.WriteLine("詳細: " + ex.ToString());

                // 呼び元に返す場合は null や空配列を返す
                return Array.Empty<PointF>();
            }
        }

        /// <summary>
        /// 数式をC#の数式に変換
        /// </summary>
        /// <param name="Susiki"></param>
        /// <returns></returns>
        private static string ConertSusikiToCSharpSusiki(string Susiki)
        {
            string CSharpSusiki = Susiki;

            CSharpSusiki = CSharpSusiki.Replace(" ", ""); // 空白削除
            CSharpSusiki = CSharpSusiki.Replace("　", ""); // 全角空白削除

            // 2x →　(2*x)
            CSharpSusiki = ReplaceByRegex(CSharpSusiki, $"([{R_DIGIT_DOT}]+)x", "($1*x)", RegexOptions.IgnoreCase);

            // 2π→　（2*Math.PI）
            CSharpSusiki = ReplaceByRegex(CSharpSusiki, $"([{R_DIGIT_DOT}]+)π", "($1*π)");

            // 2e→　（2*Math.E）
            CSharpSusiki = ReplaceByRegex(CSharpSusiki, $"([{R_DIGIT_DOT}]+)e", "($1*e)");

            // 2√(.....)  →　(2*√(.......))
            CSharpSusiki = ConnectSquare(CSharpSusiki);

            // 2(.....)  →　(2*(.......))
            CSharpSusiki = ConnectStartParenthese(CSharpSusiki);

            // 絶対値の処理
            CSharpSusiki = EvaluateAbs(CSharpSusiki);

            // √の処理
            CSharpSusiki = EvaluateSquare(CSharpSusiki);

            // 指数の処理
            CSharpSusiki = EvaluatePow(CSharpSusiki);

            // 定数の置換
            CSharpSusiki = ReplaceDifines(CSharpSusiki);

            return CSharpSusiki;
        }

        /// <summary>
        /// スクリプト作成
        /// CSharpScriptクラスに渡すスクリプトを作成する
        /// </summary>
        /// <param name="Susiki">数式</param>
        /// <param name="Start">計算範囲の開始値</param>
        /// <param name="End">計算範囲の終了値</param>
        /// <param name="CalculateInterval">計算ステップ間隔（x の刻み幅）</param>
        /// <returns></returns>
        private static string MakeScript(string Susiki, double Start, double End, double CalculateInterval)
        {
            string stript = $@"
                double Func(double x)
                {{
                    return {Susiki};
                }}
        
                double Range = {End} - {Start};
        
                int Steps = (int)(Range / {CalculateInterval}) + 1;
        
                PointF[] Nums = new PointF[Steps];
        
                for (int i = 0; i < Steps; i++)
                {{
                    double x = {Start} + i * {CalculateInterval};
                    double y = Func(x);
        
                    // PointFは構造体なので new で代入する必要あり
                    Nums[i] = new PointF((float)x, (float)y);
                }}
        
                return Nums;
                ";

            return stript;
        }

        /// <summary>
        /// 定義した定数の置換
        /// </summary>
        /// <param name="Susiki"></param>
        /// <returns></returns>
        private static string ReplaceDifines(string Susiki)
        {
            foreach (ReplaceItem rep in RepItems)
            {
                Susiki = ReplaceByRegex(Susiki, rep.Before, rep.After, RegexOptions.IgnoreCase);
            }

            return Susiki;
        }

        // =====================================================================
        // 位置計算用関数
        // =====================================================================

        /// <summary>
        /// 数式文字列に含まれる指定位置の開始括弧 '(' に対応する終了括弧 ')' の位置を返す。
        /// ネストされた括弧にも対応。
        /// </summary>
        /// <param name="expression">数式文字列</param>
        /// <param name="startIndex">開始括弧 '(' の位置</param>
        /// <returns>対応する終了括弧 ')' の位置。見つからなければ -1。</returns>
        private static int RightFindMatchingParenthesis(string expression, int startIndex)
        {
            if (expression[startIndex] != '(')
                throw new ArgumentException("startIndex は '(' を指している必要があります。");

            int depth = 0;
            for (int i = startIndex; i < expression.Length; i++)
            {
                if (expression[i] == '(')
                {
                    depth++;
                }
                else if (expression[i] == ')')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return i; // 対応する終了括弧を見つけた
                    }
                }
            }
            return -1; // 見つからなかった
        }

        private static int LeftFindMatchingParenthesis(string expression, int startIndex)
        {
            if (expression[startIndex] != ')')
                throw new ArgumentException("startIndex は ')' を指している必要があります。");

            int depth = 0;
            for (int i = startIndex; 0 <= i; i--)
            {
                if (expression[i] == ')')
                {
                    depth++;
                }
                else if (expression[i] == '(')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return i; // 対応する終了括弧を見つけた
                    }
                }
            }
            return -1; // 見つからなかった
        }

        /// <summary>
        /// 数式文字列に含まれる指定位置の数字の終了位置を返す。
        /// </summary>
        /// <param name="expression">数式文字列</param>
        /// <param name="startIndex">数値開始</param>
        /// <returns>数値の終了位置。見つからなければ -1。</returns>
        private static int LeftFindMatchingDigits(string expression, int startIndex)
        {
            if (char.IsDigit(expression[startIndex]) == false)
                throw new ArgumentException("startIndex は数字を指している必要があります。");

            for (int i = startIndex; 0 <= i; i--)
            {
                if (char.IsDigit(expression[i]) == false && expression[i] != '.')
                {
                    return i + 1; // 数字の終了位置を見つけた
                }

            }

            return 0;
        }

        private static int RightFindMatchingDigits(string expression, int startIndex)
        {
            if (char.IsDigit(expression[startIndex]) == false)
                throw new ArgumentException("startIndex は数字を指している必要があります。");

            for (int i = startIndex; i < expression.Length; i++)
            {
                if (char.IsDigit(expression[i]) == false && expression[i] != '.')
                {
                    return i - 1; // 数字の終了位置を見つけた
                }

            }

            return expression.Length - 1;
        }

        // =====================================================================
        // 文字列操作系関数
        // =====================================================================


        private static string InsertAt(string input, int index, string insertText)
        {
            if (index < 0 || index > input.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "インデックスが範囲外です");

            return input.Substring(0, index) + insertText + input.Substring(index);
        }

        /// <summary>
        /// 引数の文字列に対して、正規表現でマッチした部分を置換文字列で置き換える
        /// </summary>
        /// <param name="input">対象の文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <param name="replacement">置換文字列（正規表現のグループ参照可）</param>
        /// <param name="regexOptions">正規表現オプション</param>
        /// <returns>置換後の文字列</returns>
        private static string ReplaceByRegex(string input, string pattern, string replacement, RegexOptions regexOptions = RegexOptions.None)
        {
            return Regex.Replace(input, pattern, replacement, regexOptions);
        }

        private static string GetStringRange(string expr, int start, int end)
        {
            return expr.Substring(start, end - start + 1);
        }

        private static string StringRangeReplace(string expr, int start, int end, string Replace)
        {
            return expr.Substring(0, start) + Replace + expr.Substring(end + 1);
        }

        /// <summary>
        /// 2√  →　(2*√..)に変換
        /// </summary>
        /// <param name="CSharpSusiki">数式</param>
        /// <returns></returns>
        private static string ConnectSquare(string CSharpSusiki)
        {

            string MatchPattern = $"([{R_DIGIT}{R_DOT}]+|x)√";

            // 2√(.....)  →　(2*√(.......))
            while (Regex.IsMatch(CSharpSusiki, MatchPattern) == true)
            {
                Match m = Regex.Match(CSharpSusiki, MatchPattern);

                int EndIndex = -1;

                if (CSharpSusiki[m.Index + m.Length] == '(')
                {
                    EndIndex = RightFindMatchingParenthesis(CSharpSusiki, m.Index + m.Length);
                }
                else if (char.IsDigit(CSharpSusiki[m.Index + m.Length]))
                {
                    EndIndex = RightFindMatchingDigits(CSharpSusiki, m.Index + m.Length);
                }

                CSharpSusiki = InsertAt(CSharpSusiki, EndIndex + 1, ")");
                CSharpSusiki = InsertAt(CSharpSusiki, m.Index + m.Length - 1, "*");
                CSharpSusiki = InsertAt(CSharpSusiki, m.Index, "(");
            }

            return CSharpSusiki;
        }

        /// <summary>
        /// 2(.....)  →　(2*(.......))に変換
        /// </summary>
        /// <param name="CSharpSusiki">数式</param>
        /// <returns></returns>
        private static string ConnectStartParenthese(string CSharpSusiki)
        {
            string MatchPattern = $"([{R_DIGIT}{R_DOT}]+|x){R_START_PAREN}";

            // 2(.....)  →　(2*(.......))
            while (Regex.IsMatch(CSharpSusiki, MatchPattern) == true)
            {
                Match m = Regex.Match(CSharpSusiki, MatchPattern);

                int EndKakkoIndex = RightFindMatchingParenthesis(CSharpSusiki, m.Index + m.Length - 1);

                CSharpSusiki = InsertAt(CSharpSusiki, EndKakkoIndex + 1, ")");
                CSharpSusiki = InsertAt(CSharpSusiki, m.Index + m.Length - 1, "*");
                CSharpSusiki = InsertAt(CSharpSusiki, m.Index, "(");
            }

            return CSharpSusiki;
        }

        /// <summary>
        /// √記号処理
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string EvaluateSquare(string expr)
        {
            string CSharpSusiki = expr;
            // √( … ) を処理
            while (CSharpSusiki.Contains("√"))
            {
                int InsideStart = CSharpSusiki.IndexOf("√") + 1;

                int end = 0;
                string inside = "";

                // √の直後が括弧なら中身を抜き出す
                if (CSharpSusiki[InsideStart] == '(')
                {
                    end = RightFindMatchingParenthesis(CSharpSusiki, InsideStart);

                    inside = GetStringRange(CSharpSusiki, InsideStart + 1, end - 1);

                    inside = EvaluatePow(inside);
                }
                else if (char.IsDigit(CSharpSusiki[InsideStart]))
                {
                    end = RightFindMatchingDigits(CSharpSusiki, InsideStart);

                    inside = GetStringRange(CSharpSusiki, InsideStart, end);
                }
                else if (CSharpSusiki[InsideStart] == 'x')
                {
                    end = InsideStart;
                    inside = "x";
                }
                else
                {
                    throw new Exception("√の後は括弧が必要です");
                }

                CSharpSusiki = StringRangeReplace(CSharpSusiki, InsideStart - 1, end, string.Format("sqrt({0})", inside));
            }

            return CSharpSusiki;
        }

        /// <summary>
        /// 乗記号処理
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string EvaluatePow(string expr)
        {
            string CSharpSusiki = expr;

            // ^ を処理（右結合）
            while (CSharpSusiki.Contains("^"))
            {
                int RightInsideStart = CSharpSusiki.IndexOf("^") + 1;
                int LeftInsideStart = CSharpSusiki.IndexOf("^") - 1;

                int RightEnd = 0;
                string RightInside = "";
                int LeftEnd = 0;
                string LeftInside = "";

                // ^の直後が括弧なら中身を抜き出す
                if (CSharpSusiki[RightInsideStart] == '(')
                {
                    RightEnd = RightFindMatchingParenthesis(CSharpSusiki, RightInsideStart);

                    RightInside = GetStringRange(CSharpSusiki, RightInsideStart + 1, RightEnd - 1);

                    RightInside = EvaluatePow(RightInside);
                }
                else if (char.IsDigit(CSharpSusiki[RightInsideStart]))
                {
                    RightEnd = RightFindMatchingDigits(CSharpSusiki, RightInsideStart);

                    RightInside = GetStringRange(CSharpSusiki, RightInsideStart, RightEnd);
                }
                else if (CSharpSusiki[RightInsideStart] == 'x')
                {
                    RightEnd = RightInsideStart;
                    RightInside = "x";
                }
                else
                {
                    throw new Exception("√の後は括弧が必要です");
                }

                if (CSharpSusiki[LeftInsideStart] == ')')
                {
                    LeftEnd = LeftFindMatchingParenthesis(CSharpSusiki, LeftInsideStart);

                    LeftInside = GetStringRange(CSharpSusiki, LeftEnd + 1, LeftInsideStart - 1);

                    LeftInside = EvaluatePow(LeftInside);
                }
                else if (char.IsDigit(CSharpSusiki[LeftInsideStart]))
                {
                    LeftEnd = LeftFindMatchingDigits(CSharpSusiki, LeftInsideStart);

                    LeftInside = GetStringRange(CSharpSusiki, LeftEnd, LeftInsideStart);
                }
                else if (CSharpSusiki[LeftInsideStart] == 'x')
                {
                    LeftEnd = LeftInsideStart;
                    LeftInside = "x";
                }
                else
                {
                    throw new Exception("Pow Error");
                }

                CSharpSusiki = StringRangeReplace(CSharpSusiki, LeftEnd, RightEnd, string.Format("pow({0}, {1})", LeftInside, RightInside));
            }

            return CSharpSusiki;
        }

        /// <summary>
        /// 絶対値記号処理
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private static string EvaluateAbs(string expr)
        {
            string CSharpSusiki = expr;

            string MatchPattern = $"{R_ABS}[{R_DIGIT}{R_CHAR}{R_END_PAREN}]+{R_ABS}";

            while (Regex.IsMatch(CSharpSusiki, MatchPattern, RegexOptions.IgnoreCase) == true)
            {
                Match m = Regex.Match(CSharpSusiki, MatchPattern, RegexOptions.IgnoreCase);

                string inside = GetStringRange(CSharpSusiki, m.Index + 1, m.Index + m.Length - 2);
                CSharpSusiki = StringRangeReplace(CSharpSusiki, m.Index, m.Index + m.Length - 1, $"abs({inside})");
            }

            MatchPattern = $"{R_ABS}{R_MINUS}[{R_DIGIT}{R_CHAR}]+{R_ABS}";

            while (Regex.IsMatch(CSharpSusiki, MatchPattern, RegexOptions.IgnoreCase) == true)
            {
                Match m = Regex.Match(CSharpSusiki, MatchPattern, RegexOptions.IgnoreCase);

                string inside = GetStringRange(CSharpSusiki, m.Index + 1, m.Index + m.Length - 2);
                CSharpSusiki = StringRangeReplace(CSharpSusiki, m.Index, m.Index + m.Length - 1, $"abs({inside})");
            }

            string InsideFirttChar = $"{R_DIGIT}{R_CHAR}{R_START_PAREN}";
            string InsideLastChar = $"{R_DIGIT}{R_CHAR}{R_END_PAREN}";
            string InseideMid = $"{R_DIGIT}{R_CHAR}{R_OPER}{R_DOT}{R_START_PAREN}{R_END_PAREN}+";

            MatchPattern = $"{R_ABS}[{InsideFirttChar}][{InseideMid}]+[{InsideLastChar}]{R_ABS}";

            while (Regex.IsMatch(CSharpSusiki, MatchPattern, RegexOptions.IgnoreCase) == true)
            {
                Match m = Regex.Match(CSharpSusiki, MatchPattern, RegexOptions.IgnoreCase);

                string inside = GetStringRange(CSharpSusiki, m.Index + 1, m.Index + m.Length - 2);
                CSharpSusiki = StringRangeReplace(CSharpSusiki, m.Index, m.Index + m.Length - 1, $"abs({inside})");
            }

            MatchPattern = $"{R_ABS}{R_MINUS}[{InsideFirttChar}][{InseideMid}]+[{InsideLastChar}]{R_ABS}";

            while (Regex.IsMatch(CSharpSusiki, MatchPattern, RegexOptions.IgnoreCase) == true)
            {
                Match m = Regex.Match(CSharpSusiki, MatchPattern, RegexOptions.IgnoreCase);

                string inside = GetStringRange(CSharpSusiki, m.Index + 1, m.Index + m.Length - 2);
                CSharpSusiki = StringRangeReplace(CSharpSusiki, m.Index, m.Index + m.Length - 1, $"abs({inside})");
            }

            return CSharpSusiki;
        }
    }
}
