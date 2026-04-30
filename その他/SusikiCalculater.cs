using graphicbox2d.グローバル変数;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graphicbox2d
{
    /// <summary>
    /// 数式計算の結果の種類を表す列挙型
    /// </summary>
    public enum eSusikiCalRet
    {
        /// <summary>
        /// 計算成功
        /// </summary>
        Success,

        /// <summary>
        /// 計算エラー（例：数式の形式が不正、評価中に例外が発生など）
        /// </summary>
        Error,
    }

    /// <summary>
    /// 
    /// </summary>
    public struct SusikiCalResult
    {
        /// <summary>
        /// 計算結果
        /// </summary>
        public eSusikiCalRet ResultType;

        /// <summary>
        /// 計算結果メッセージ
        /// </summary>
        public string ResultMessage;

        /// <summary>
        /// 計算結果の座標を格納した PointF 配列。
        /// </summary>
        public PointF[] Points;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="_ResultType">計算結果の種類</param>
        /// <param name="_ResultMessage">計算結果のメッセージ</param>
        /// <param name="_Points">計算結果の座標配列</param>
        public SusikiCalResult(eSusikiCalRet _ResultType, string _ResultMessage, PointF[] _Points)
        {
            ResultType = _ResultType;
            ResultMessage = _ResultMessage;
            Points = _Points;
        }
    }

    /// <summary>
    /// 数式計算クラス
    /// </summary>
    public static class SusikiCalculater
    {
        /// <summary>
        /// 絶対値
        /// </summary>
        private const string R_ABS = @"\|";

        /// <summary>
        /// ルート記号
        /// </summary>
        private const string R_SQUARE = @"√";

        /// <summary>
        /// π
        /// </summary>
        private const string R_PI = @"π";

        /// <summary>
        /// 自然対数の底 e
        /// </summary>
        private const string R_E = @"(e(?!xp)|ｅ)";

        /// <summary>
        /// 変数 x
        /// </summary>
        private const string R_X = @"x";

        /// <summary>
        /// 文字
        /// </summary>
        private const string R_CHAR = @"[a-z]";

        /// <summary>
        /// 文字（大文字）
        /// </summary>
        private const string R_CHAR_UPPER = @"[A-Z]";

        /// <summary>
        /// 数字
        /// </summary>
        private const string R_DIGIT = @"\d";

        /// <summary>
        /// 小数点記号
        /// </summary>
        private const string R_DOT = @"\.";

        /// <summary>
        /// 開始括弧
        /// </summary>
        private const string R_START_PAREN = @"\(";
        /// <summary>
        /// 終了括弧
        /// </summary>
        private const string R_END_PAREN = @"\)";

        /// <summary>
        /// 乗記号
        /// </summary>
        private const string R_POW = @"\^";

        /// <summary>
        /// 演算子
        /// </summary>
        private const string R_OPER = @"\+\-\*\/";

        /// <summary>
        /// プラス記号
        /// </summary>
        private const string R_PLUS = @"\+";

        /// <summary>
        /// マイナス記号
        /// </summary>
        private const string R_MINUS = @"\-";

        /// <summary>
        /// 積記号
        /// </summary>
        private const string R_MULT = @"\*";

        /// <summary>
        /// 除記号
        /// </summary>
        private const string R_DIV = @"\/";

        /// <summary>
        /// 全角プラス記号
        /// </summary>
        private const string R_PLUS_BIG = @"＋";

        /// <summary>
        /// 全角マイナス記号
        /// </summary>
        private const string R_MINUS_BIG = @"－";

        /// <summary>
        /// 全角積記号
        /// </summary>
        private const string R_MULT_BIG = @"×";

        /// <summary>
        /// 全角除記号
        /// </summary>
        private const string R_DIV_BIG = @"÷";

        /// <summary>
        /// 小数点つきの数字
        /// 例： 123, 3.14, 0.5 など
        /// </summary>
        private static readonly string R_DIGIT_SET = $"{R_DIGIT}+[{R_DIGIT}{R_DOT}]+{R_DIGIT}+";

        /// <summary>
        /// 小数点つきの数字または変数xまたは定数eやπ
        /// </summary>
        private static readonly string R_DIGIT_SET_OR_X_OR_E_OR_PI = $"({R_DIGIT_SET}|x|Math.E|Math.PI|{R_DIGIT})";
            
        /// <summary>
        /// 絶対値の内側の最も右、または左にある文字や数字、括弧
        /// </summary>
        private static readonly string R_INNER_ABS_SIDE = $@"({R_DIGIT}|{R_CHAR}|{R_CHAR_UPPER}|{R_START_PAREN}|{R_END_PAREN})";

        /// <summary>
        /// 絶対値の内側にあるパターン
        /// </summary>
        private static readonly string R_INNER_ABS = $@"{R_INNER_ABS_SIDE}[^|]*{R_INNER_ABS_SIDE}";

        /// <summary>
        /// 絶対値の内側にあるパターン（1文字のみ）
        /// </summary>
        private static readonly string R_INNER_ABS_ONE_CHAR = $@"({R_X}|{R_DIGIT})";

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
            //RepItems.Add(new ReplaceItem("e", "Math.E"));
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

        public static async Task<SusikiCalResult> Caluculate(string Susiki, double Start, double End, double CalculateInterval)
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

                PointF[] points = await CSharpScript.EvaluateAsync<PointF[]>(script, options);

                SusikiCalResult result = new SusikiCalResult(eSusikiCalRet.Success, "Calculation successful.", points);

                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = "An error occurred while evaluating the expression.\n\nPlease check the format of the expression.\n\nDetails: " + ex.Message;

                PointF[] points = Array.Empty<PointF>();

                SusikiCalResult result = new SusikiCalResult(eSusikiCalRet.Error, errorMessage, points);

                // 呼び元に返す場合は null や空配列を返す
                return result;
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

            // 定数の置換
            CSharpSusiki = ReplaceDifines(CSharpSusiki);

            // 2x →　(2*x)
            CSharpSusiki = ReplaceByRegex(CSharpSusiki, $"({R_DIGIT_SET})x", $"{R_START_PAREN}$1*x{R_END_PAREN}", RegexOptions.IgnoreCase);

            // 2π→　（2*Math.PI）
            CSharpSusiki = ReplaceByRegex(CSharpSusiki, $"({R_DIGIT_SET})Math.PI", $"{R_START_PAREN}$1*Math.PI{R_END_PAREN}");

            // 2e→　（2*Math.E）
            CSharpSusiki = ReplaceByRegex(CSharpSusiki, $"({R_DIGIT_SET})Math.E", $"{R_START_PAREN}$1*Math.E{R_END_PAREN}");

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

        /// <summary>
        /// 数式文字列に含まれる指定位置の終了括弧 ')' に対応する開始括弧 '(' の位置を返す。
        /// ネストされた括弧にも対応。
        /// </summary>
        /// <param name="expression">数式文字列</param>
        /// <param name="startIndex">終了括弧 ')' の位置</param>
        /// <returns>対応する開始括弧 '(' の位置。見つからなければ -1。</returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// 数式文字列に含まれる指定位置の数字の終了位置を返す。
        /// </summary>
        /// <param name="expression">数式文字列</param>
        /// <param name="startIndex">数字の開始位置</param>
        /// <returns>数字の終了位置。見つからなければ -1。</returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// 数式文字列の指定位置に文字列を挿入する。
        /// </summary>
        /// <param name="input">対象の文字列</param>
        /// <param name="index">挿入位置</param>
        /// <param name="insertText">挿入する文字列</param>
        /// <returns>挿入後の文字列</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
        /// <param name="regexOptions">正規表現のオプション</param>
        /// <returns>置換後の文字列</returns>
        private static string ReplaceByRegex(string input, string pattern, string replacement, RegexOptions regexOptions = RegexOptions.None)
        {
            return Regex.Replace(input, pattern, replacement, regexOptions);
        }

        /// <summary>
        /// 数式文字列の指定範囲を抜き出す
        /// </summary>
        /// <param name="expr">数式文字列</param>
        /// <param name="start">開始位置</param>
        /// <param name="end">終了位置</param>
        /// <returns>指定範囲の文字列</returns>
        private static string GetStringRange(string expr, int start, int end)
        {
            return expr.Substring(start, end - start + 1);
        }

        /// <summary>
        /// 数式文字列の指定範囲を置換する
        /// </summary>
        /// <param name="expr">数式文字列</param>
        /// <param name="start">開始位置</param>
        /// <param name="end">終了位置</param>
        /// <param name="Replace">置換文字列</param>
        /// <returns>置換後の文字列</returns>
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

            // 2√(.....)  →　(2*√(.......))
            string MatchPattern = $"({R_DIGIT_SET_OR_X_OR_E_OR_PI})√";

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
            string MatchPattern = $"{R_DIGIT_SET_OR_X_OR_E_OR_PI}{R_START_PAREN}";

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

                CSharpSusiki = StringRangeReplace(CSharpSusiki, InsideStart - 1, end, string.Format("Math.Sqrt({0})", inside));
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

                CSharpSusiki = StringRangeReplace(CSharpSusiki, LeftEnd, RightEnd, string.Format("Math.Pow({0}, {1})", LeftInside, RightInside));
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

            // =======================================================================
            // 絶対値記号内に文字列が１文字しかない場合
            // ========================================================================
            // 正規表現とマッチしている文字列があるか判定
            while (Regex.Match(CSharpSusiki, $"{R_ABS}{R_INNER_ABS_ONE_CHAR}{R_ABS}").Success == true)
            {
                Match m = Regex.Match(CSharpSusiki, $"{R_ABS}{R_INNER_ABS_ONE_CHAR}{R_ABS}");
                int AbsStart = m.Index;
                int AbsEnd = m.Index + m.Length - 1;
                string inside = GetStringRange(CSharpSusiki, AbsStart + 1, AbsEnd - 1);

                CSharpSusiki = StringRangeReplace(CSharpSusiki, AbsStart, AbsEnd, string.Format("Math.Abs({0})", inside));
            }

            // =======================================================================
            // 絶対値記号内に文字列が複数ある場合
            // ========================================================================
            // 正規表現とマッチしている文字列があるか判定
            while (Regex.Match(CSharpSusiki, $"{R_ABS}{R_INNER_ABS}{R_ABS}").Success == true)
            {
                Match m = Regex.Match(CSharpSusiki, $"{R_ABS}{R_INNER_ABS}{R_ABS}");
                int AbsStart = m.Index;
                int AbsEnd = m.Index + m.Length - 1;
                string inside = GetStringRange(CSharpSusiki, AbsStart + 1, AbsEnd - 1);

                CSharpSusiki = StringRangeReplace(CSharpSusiki, AbsStart, AbsEnd, string.Format("Math.Abs({0})", inside));
            }

            return CSharpSusiki;
        }
    }
}
