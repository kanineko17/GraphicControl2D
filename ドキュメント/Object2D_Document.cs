using graphicbox2d.その他;
using SkiaSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace graphicbox2d
{
    /// <summary>
    /// 全図形の基底クラス
    /// </summary>
    public class Object2D_Document : IDisposable
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public virtual eObject2DType m_Type => eObject2DType.None;

        /// <summary>
        /// 選択フラグ
        /// true:選択中　false:未選択
        /// </summary>
        public bool IsSelect { get; set; } = false;

        /// <summary>
        /// 描画の順番
        /// 値が大きいほど手前に描画される
        /// </summary>
        public float ZOrder { get; set; } = 0f;

        /// <summary>
        /// 格オブジェクトのインデックス番号
        /// 比較の時に使用する
        /// </summary>
        public int _ID;

        /// <summary>
        /// Disposeメソッド。リソースを解放するための処理を実装する。
        /// </summary>
        public void Dispose()
        {
        }
    }
}
