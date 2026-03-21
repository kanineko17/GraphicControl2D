using CSharpCaluc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //PointF[] tttt =  test222();
            var t = SusikiCalculater.Caluculate("√2x + 2√(2(x+2)^(2+2x^x))*2(x+2)", 0, 1, 0.1);

            string test = SusikiCalculater.ConertSusikiToCSharpSusiki("||sin(x)| - |cos(x)||+2^2√(1+|sin(x)|) + |tan(x/2) + |-2||");
            test = SusikiCalculater.ConertSusikiToCSharpSusiki("√2x + 2*√(2(x+2)^(2+2x^x))*2(x+2)");
        }

        double Func(double x)
        {
            return Math.Sqrt(x - Math.Pow(x, 2) - Math.Sqrt(x)) * Math.Pow(2, 25 * x);
        }

        private PointF[] test222()
        {

            double Range = 1 - 0;

            int Steps = (int)(Range / 0.1) + 1;

            PointF[] Nums = new PointF[Steps];

            for (int i = 0; i < Steps; i++)
            {
                double x = 0 + i * 0.1;
                double y = Func(x);

                // PointFは構造体なので new で代入する必要あり
                Nums[i] = new PointF((float)x, (float)y);
            }

            return Nums;
        }


        ///// <summary>
        ///// 数式文字列をC#言語で記述された言語として評価して、計算を行う
        ///// </summary>
        ///// <param name="Susiki">
        ///// C#で作成した数式 
        ///// 例: "(Math.Cos(x) + Math.Sqrt(4))+2")</param>
        ///// <returns>計算結果 (double)</returns>
        //public static PointF[] Caluculate(string Susiki, string HensuName, double Start, double End, double CalculateInterval)
        //{

        //    string SCRIPT_TEMPLATE = $@"
        //    double Func(double x)
        //    {{
        //        return {Susiki};
        //    }}

        //    double Range = {End} - {Start};

        //    int Steps = (int)(Range / {CalculateInterval}) + 1;

        //    PointF[] Nums = new PointF[Steps];

        //    for (int i = 0; i < Steps; i++)
        //    {{
        //        double x = {Start} + i * {CalculateInterval};

        //        double y = Func(x);

        //        Nums[i].X = (float)x;
        //        Nums[i].Y = (float)y;
        //    }}

        //    return Nums;
        //     ";

        //    string test = Evaluate("√(1-5)*2^(25*1)");
        //}

        //private static string ConvertSusiki(string expression)
        //{


        //}


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

            for (int i = startIndex;  0 <= i; i--)
            {
                if (char.IsDigit(expression[i]) == false && expression[i] != '.')
                {
                    return i - 1; // 数字の終了位置を見つけた
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

        private static string GetStringRange(string expr, int start, int end)
        {
            return expr.Substring(start, end - start + 1);
        }

        private static string StringRangeReplace(string expr, int start, int end, string Replace)
        {
            return expr.Substring(0, start) + Replace + expr.Substring(end + 1);
        }

        /// <summary>
        /// 数式文字列を評価する（√ と ^ に対応）
        /// </summary>
        private static string EvaluateSquareAndPow(string expr)
        {

            // ^ を処理（右結合）
            while (expr.Contains("^"))
            {
                int RightInsideStart = expr.IndexOf("^") + 1;
                int LeftInsideStart = expr.IndexOf("^") - 1;

                int RightEnd = 0;
                string RightInside = "";
                int LeftEnd = 0;
                string LeftInside = "";

                // ^の直後が括弧なら中身を抜き出す
                if (expr[RightInsideStart] == '(')
                {
                    RightEnd = RightFindMatchingParenthesis(expr, RightInsideStart);

                    RightInside = GetStringRange(expr, RightInsideStart + 1, RightEnd - 1);

                    RightInside = EvaluateSquareAndPow(RightInside);
                }
                else if (char.IsDigit(expr[RightInsideStart]))
                {
                    RightEnd = RightFindMatchingDigits(expr, RightInsideStart);

                    RightInside = GetStringRange(expr, RightInsideStart, RightEnd);
                }
                else if (expr[RightInsideStart] == 'x')
                {
                    RightEnd = RightInsideStart;
                    RightInside = "x";
                }
                else
                {
                    throw new Exception("√の後は括弧が必要です");
                }

                // ^の直全が括弧なら中身を抜き出す
                if (expr[LeftInsideStart] == ')')
                {
                    LeftEnd = LeftFindMatchingParenthesis(expr, LeftInsideStart);

                    LeftInside = GetStringRange(expr, LeftInsideStart + 1, LeftEnd - 1);

                    LeftInside = EvaluateSquareAndPow(LeftInside);
                }
                else if (char.IsDigit(expr[LeftInsideStart]))
                {
                    LeftEnd = LeftFindMatchingDigits(expr, LeftInsideStart);

                    LeftInside = GetStringRange(expr, LeftEnd, LeftInsideStart);
                }
                else if (expr[LeftInsideStart] == 'x')
                {
                    LeftEnd = LeftInsideStart;
                    LeftInside = "x";
                }
                else
                {
                    throw new Exception("√の後は括弧が必要です");
                }

                expr = StringRangeReplace(expr, LeftEnd, RightEnd, string.Format("Math.Pow({0}, {1})", LeftInside, RightInside));
            }

            // √( … ) を処理
            while (expr.Contains("√"))
            {
                int InsideStart = expr.IndexOf("√") + 1;

                int end = 0;
                string inside = "";

                // √の直後が括弧なら中身を抜き出す
                if (expr[InsideStart] == '(')
                {
                    end = RightFindMatchingParenthesis(expr, InsideStart);

                    inside = GetStringRange(expr, InsideStart + 1, end - 1);

                    inside = EvaluateSquareAndPow(inside);
                }
                else if (char.IsDigit(expr[InsideStart]))
                {
                    end = RightFindMatchingDigits(expr, InsideStart);

                    inside = GetStringRange(expr, InsideStart, end);
                }
                else if (expr[InsideStart] == 'x')
                {
                    end = InsideStart;
                    inside = "x";
                }
                else
                {
                    throw new Exception("√の後は括弧が必要です");
                }

                expr = StringRangeReplace(expr, InsideStart - 1, end, string.Format("Math.Sqrt({0})", inside));
            }

            return expr;
        }
    }
}
