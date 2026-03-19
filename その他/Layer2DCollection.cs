using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// レイヤーコレクションクラス
    /// </summary>
    public class Layer2DCollection : List<Layer2D>
    {
        /// <summary>
        /// レイヤー名でレイヤーを取得するインデクサー
        /// 取得に失敗した場合はnullを返す
        /// </summary>
        /// <param name="name">レイヤー名</param>
        /// <returns>レイヤー</returns>
        public Layer2D this[string name]
        {
            get => this.FirstOrDefault(l => l.LayerName == name);
        }
    }
}
