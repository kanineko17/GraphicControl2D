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
    /// 数式グラフ図形のドキュメントクラス
    /// </summary>
    public class MathGraph2D_Document : Graph2D_Document
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
    }
}