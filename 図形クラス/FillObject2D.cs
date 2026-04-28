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
                fillObject2D.LineCustomLineStyle = this.LineCustomLineStyle.Clone() as float[];
                fillObject2D.LineCustomDashPhase = this.LineCustomDashPhase;
                fillObject2D.LineColor           = this.LineColor;
                fillObject2D.IsFilled            = this.IsFilled;
                fillObject2D.FillColor           = this.FillColor;
            }
        }

        /// <summary>
        /// ドキュメントデータを書き出す
        /// </summary>
        /// <param name="doc">出力先ドキュメント</param>
        public override void OutDocument(ref Object2D_Document doc)
        {
            if (doc == null)
            {
                doc = new FillObject2D_Document();
            }

            base.OutDocument(ref doc);

            FillObject2D_Document fillDoc = (FillObject2D_Document)doc;
            fillDoc.IsDrawLine = this.IsDrawLine;
            fillDoc.LineWidth = this.LineWidth;
            fillDoc.LineStyle = this.LineStyle;
            fillDoc.LineCustomLineStyle = this.LineCustomLineStyle?.Clone() as float[];
            fillDoc.LineCustomDashPhase = this.LineCustomDashPhase;
            fillDoc.LineColor = this.LineColor;
            fillDoc.IsFilled = this.IsFilled;
            fillDoc.FillColor = this.FillColor;
        }

        /// <summary>
        /// ドキュメントデータを取り込む
        /// </summary>
        /// <param name="doc">取り込むドキュメント</param>
        public override void ImportDocument(in Object2D_Document doc)
        {
            base.ImportDocument(doc);

            FillObject2D_Document fillDoc = (FillObject2D_Document)doc;

            this.IsDrawLine = fillDoc.IsDrawLine;
            this.LineWidth = fillDoc.LineWidth;
            this.LineStyle = fillDoc.LineStyle;
            this.LineCustomLineStyle = fillDoc.LineCustomLineStyle?.Clone() as float[];
            this.LineCustomDashPhase = fillDoc.LineCustomDashPhase;
            this.LineColor = fillDoc.LineColor;
            this.IsFilled = fillDoc.IsFilled;
            this.FillColor = fillDoc.FillColor;
        }
    }
}
