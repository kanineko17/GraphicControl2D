using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 塗りつぶし図形の基底クラス
    /// </summary>
    public class FillObject2D_Document : Object2D_Document, IFillProperty, IDisposable
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
        /// Disposeメソッド。リストをクリアしてリソースを解放する。
        /// </summary>
        public new void Dispose()
        {
            LineCustomLineStyle = null;
            base.Dispose();
        }
    }
}
