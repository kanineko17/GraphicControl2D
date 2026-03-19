using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 矢印図形クラス
    /// </summary>
    public class Arrow2D : Line2D, ILineProperty
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Arrow;

        /// <summary>
        /// 線分の始点の矢印スタイル
        /// </summary>
        public Arrow2DLineCapType StartCap { get; set; } = Arrow2DLineCapType.None;

        /// <summary>
        /// 線分の終点の矢印スタイル
        /// </summary>
        public Arrow2DLineCapType EndCap { get; set; } = Arrow2DLineCapType.Arrow;

        /// <summary>
        /// 線分の始点の矢印サイズ
        /// </summary>
        public CapSize StartCapSize { get; set; } = new CapSize() { Width = 4, Height = 4 };

        /// <summary>
        /// 線分の終点の矢印サイズ
        /// </summary>
        public CapSize EndCapSize { get; set; }  = new CapSize() { Width = 4, Height = 4 };

        // ===============================================================================
        // 公開メソッド
        // ===============================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Arrow2D()
        {
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns>コピーしたオブジェクト</returns>
        public override Object2D Clone()
        {
            Arrow2D clone = new Arrow2D();

            // 基底クラスのデータをコピー
            this.BaseCopyDataTo(clone);

            clone.StartCap = StartCap;
            clone.EndCap   = EndCap;

            return clone;
        }

        // ===============================================================================
        // 非公開メソッド
        // ===============================================================================

        /// <summary>
        /// マウスヒット中の図形（拡大した図形）を返す。
        /// </summary>
        /// <returns>拡張された図形</returns>
        internal override Object2D GetHitObject()
        {
            Arrow2D arrow2D = (Arrow2D)this.Clone();

            arrow2D.Width += MouseHitLineOffset;

            return arrow2D;
        }
    }

    /// <summary>
    /// 矢印付き線分の線端タイプ
    /// </summary>
    public enum Arrow2DLineCapType
    {
        /// <summary>矢印なし</summary>
        None,
        /// <summary>通常の矢印</summary>
        Arrow,
    }
}
