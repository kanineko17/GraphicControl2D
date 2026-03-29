using graphicbox2d.グラフィック計算;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 数式グラフ図形を表すクラス。
    /// </summary>
    public class MathGraph2D : Graph2D
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.MathGraph;

        /// <summary>
        /// グラフ化する数式を設定
        /// ※数式を設定した後、必ず CalculateGraphPoints() メソッドを実行して点リストを計算してください。
        /// 
        /// ＊概要＊
        /// グラフ化する数式を文字列で指定します。
        /// 例：x^(2√2) + 3*x + 2 + |sin(x)÷2|
        /// 
        /// ＊変数名＊
        /// 数式の変数名は「x」のみ使用可能です。
        /// 〇例：sin(x)、x^2 + 3×x +2
        /// ×例：sin(a)、a^2 + 3×a +2
        /// 
        /// ＊使用可能演算子＊
        /// 以下の演算子が使用可能です。
        /// ・掛け算：×、＊
        /// ・割り算：÷、/
        /// ・足し算：＋、+
        /// ・引き算：－、-
        /// ・乗算　：＾、^
        /// 
        /// ＊使用可能記号＊
        /// ・根号　　　：√  （例：√x、√(x+1)）
        /// ・絶対値記号：｜　（例：|x+1|）
        /// 
        /// ＊使用可能定数記号＊
        /// ・円周率　　：π
        /// ・ネイピア数：e
        /// 
        /// ＊使用可能な関数＊
        /// ・アークサイン              ：Asin
        /// ・アークサイン              ：ArcSin
        /// ・アークコサイン            ：Acos
        /// ・アークコサイン            ：ArcCos
        /// ・アークタンジェント        ：Atan
        /// ・アークタンジェント        ：ArcTan
        /// ・サイン                    ：Sin
        /// ・コサイン                  ：Cos
        /// ・タンジェント              ：Tan
        /// ・ハイパボリックサイン      ：Sinh
        /// ・ハイパボリックコサイン    ：Cosh
        /// ・ハイパボリックタンジェント：Tanh
        /// ・対数（自然対数）          ：Log
        /// ・指数関数                  ：Exp        
        /// 
        /// </summary>
        public string Susiki { get; set; } = "sin(x)";

        /// <summary>
        /// グラフ化するXの開始値
        /// </summary>
        public float StartX { get; set; } = -10.0f;

        /// <summary>
        /// グラフ化するXの終了値
        /// </summary>
        public float EndX { get; set; } = 10.0f;

        /// <summary>
        /// 計算ステップ間隔
        /// この値の感覚でグラフの滑らかさが変わります。
        /// 小さくするほど滑らかになりますが、計算に時間がかかります。
        /// </summary>
        public float CalculateInterval { get; set; } = 0.05f;


        // ===============================================================================
        // 非公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 原点とXの値が最も近い点のインデックス値
        /// </summary>
        private int ZeroXPointIndex = -1;

        // ===============================================================================
        // 公開メソッド
        // ===============================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MathGraph2D()
        {
        }

        /// <summary>
        /// グラフの点リストを計算する
        /// ※呼び出し元で待機は不要です。非同期で実行されます。
        /// </summary>
        public async Task CalculateGraphPoints()
        {
            // 計算開始メッセージを送信
            WinAPI.PostMessage(Graphic2DControl.hWnd, WinMsg.WM_SUSIKI_CALC_START, (IntPtr)_ID, IntPtr.Zero);


            // 重い計算はバックグラウンドで実行
            PointF[] points = await Task.Run(() =>
                GraphicCaluculate.SusikiCaluculate(Susiki, StartX, EndX, CalculateInterval, _ID)
            );

            Points = points.ToList();
            ZeroXPointIndex = GetZeroXPointIndex(Points);

            // 計算終了メッセージを送信
            WinAPI.PostMessage(Graphic2DControl.hWnd, WinMsg.WM_SUSIKI_CALC_END, (IntPtr)_ID, IntPtr.Zero);
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns>コピーしたオブジェクト</returns>
        public override Object2D Clone()
        {
            MathGraph2D clone = new MathGraph2D();

            // 基底クラスのデータをコピー
            this.BaseCopyDataTo(clone);

            // 自クラスのデータをコピー
            clone.Susiki = this.Susiki;
            clone.StartX = this.StartX;
            clone.EndX = this.EndX;
            clone.CalculateInterval = this.CalculateInterval;

            return clone;
        }

        // ===============================================================================
        // 非公開メソッド
        // ===============================================================================

        /// <summary>
        /// 引数の点リストからXの値が0に最も近い点のインデックス値を取得する
        /// </summary>
        /// <param name="Points">点リスト</param>
        /// <returns>インデックス値。リストが空なら -1 を返す</returns>
        private int GetZeroXPointIndex(List<PointF> Points)
        {
            if (Points == null || Points.Count == 0)
            {
                return -1; // 空リストなら -1
            }

            int closestIndex = 0;
            float minDistance = Math.Abs(Points[0].X);

            for (int i = 1; i < Points.Count; i++)
            {
                float distance = Math.Abs(Points[i].X);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }
    }
}