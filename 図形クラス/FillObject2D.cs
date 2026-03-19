using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 塗りつぶし図形の基底クラス
    /// </summary>
    public class FillObject2D : Object2D, IFillProperty
    {

        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.None;

        /// <summary>
        /// 輪郭線描画フラグ
        /// </summary>
        public bool IsDrawLine { get; set; } = true;

        /// <summary>
        /// 線の太さ
        /// </summary>
        public int LineWidth { get; set; } = 1;

        /// <summary>
        /// 線の種類
        /// </summary>
        public LineStyle LineStyle { get; set; } = LineStyle.Solid;

        /// <summary>
        /// カスタムの線のパターン
        /// ※StyleプロパティがLineStyle.Customの場合に有効
        /// </summary>
        public float[] LineCustomLineStyle { get; set; }

        /// <summary>
        /// カスタムの線の位相
        /// ※StyleプロパティがLineStyle.Customの場合に有効
        /// </summary>
        public float LineCustomDashPhase { get; set; }

        /// <summary>
        /// 輪郭線の色
        /// </summary>
        public Color LineColor { get; set; } = Color.White;

        /// <summary>
        /// 塗りつぶしフラグ
        /// </summary>
        public bool IsFilled { get; set; } = true;

        /// <summary>
        /// 塗りつぶしの色
        /// </summary>
        public Color FillColor { get; set; } = Color.White;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FillObject2D()
        {
        }

        // ===============================================================================
        // 非公開メソッド
        // ===============================================================================

        /// <summary>
        /// コピー元のObject2Dデータをコピー先にコピーする。
        /// </summary>
        /// <param name="target">コピー先</param>
        protected new void BaseCopyDataTo(Object2D target)
        {
            base.BaseCopyDataTo(target);

            if (target is FillObject2D fillObject2D)
            {
                fillObject2D.IsDrawLine       = this.IsDrawLine;
                fillObject2D.LineWidth           = this.LineWidth;
                fillObject2D.LineStyle           = this.LineStyle;
                fillObject2D.LineCustomLineStyle = this.LineCustomLineStyle;
                fillObject2D.LineColor           = this.LineColor;
                fillObject2D.IsFilled            = this.IsFilled;
                fillObject2D.FillColor           = this.FillColor;
            }
        }
    }
}
