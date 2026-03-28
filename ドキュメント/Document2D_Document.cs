using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// ドキュメントクラス
    /// </summary>
    internal class Document2D
    {
        /// <summary>
        /// バージョン
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// オブジェクトクラスのcounterの値を保存するプロパティ
        /// </summary>
        public int ObjectIDCounter { get; set; } = 0;

        /// <summary>
        /// レイヤーリスト
        /// </summary>
        public List<Layer2D_Document> Layers { get; set; } = new List<Layer2D_Document>();
    }
}
