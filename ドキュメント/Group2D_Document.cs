using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// グループ図形クラス。複数の図形をまとめてグループ化するためのクラス。
    /// </summary>
    public class Group2D_Document : Object2D_Document
    {
        // =======================================================================
        // 公開プロパティ
        // =======================================================================
        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Group;

        /// <summary>
        /// グループ化する図形のリスト
        /// </summary>
        public List<Group2DItem_Document> ObjectList { get; set; } = new List<Group2DItem_Document>();

        /// <summary>
        /// IDisposableインターフェイスの実装。グループ内の各オブジェクトを解放するメソッド。
        /// </summary>
        public override void Dispose()
        {
            foreach (var item in ObjectList)
            {
                item.Object?.Dispose();
            }
            ObjectList.Clear();
        }
    }

    /// <summary>
    /// グループ図形のアイテムを表すクラス。
    /// </summary>
    public class Group2DItem_Document
    {
        /// <summary>
        /// オブジェクト
        /// </summary>
        public Object2D_Document Object { get; set; }

        /// <summary>
        /// Zオーダー（グループ内の描画順序を指定するための値。大きいほど前面に描画される）
        /// </summary>
        public int ZOrder { get; set; }
    }
}
