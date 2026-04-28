using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 点図形クラス
    /// </summary>
    public class Point2D : Circle2D
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Point;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Point2D() 
        {
            this.R = 0.05f;
            this.IsFilled = true;
        }

        /// <summary>
        /// ドキュメントデータを書き出す
        /// </summary>
        /// <param name="doc">出力先ドキュメント</param>
        public override void OutDocument(ref Object2D_Document doc)
        {
            if (doc == null)
            {
                doc = new Point2D_Document();
            }

            base.OutDocument(ref doc);
        }

        /// <summary>
        /// ドキュメントデータを取り込む
        /// </summary>
        /// <param name="doc">取り込むドキュメント</param>
        public override void ImportDocument(in Object2D_Document doc)
        {
            base.ImportDocument(doc);
        }

    }
}
