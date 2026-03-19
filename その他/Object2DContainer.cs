using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace graphicbox2d
{
    /// <summary>
    /// 2Dオブジェクトをまとめて保持するコンテナクラス。
    /// XMLシリアライズ／デシリアライズ時に、各派生型を正しく処理できるよう
    /// XmlInclude属性で対象型を明示している。
    /// </summary>
    [XmlInclude(typeof(Object2D))]
    [XmlInclude(typeof(Line2D))]
    [XmlInclude(typeof(Point2D))]
    [XmlInclude(typeof(Circle2D))]
    [XmlInclude(typeof(Polygon2D))]
    [XmlInclude(typeof(Arrow2D))]
    [XmlInclude(typeof(Text2D))]
    [XmlInclude(typeof(Arc2D))]
    [XmlInclude(typeof(Graph2D))]
    public class Object2DContainer
    {
        /// <summary>
        /// 線分オブジェクトのリスト
        /// </summary>
        public List<Line2D> Lines { get; set; }

        /// <summary>
        /// 点オブジェクトのリスト
        /// </summary>
        public List<Point2D> Points { get; set; }

        /// <summary>
        /// 円オブジェクトのリスト
        /// </summary>
        public List<Circle2D> Circles { get; set; }

        /// <summary>
        /// 多角形オブジェクトのリスト
        /// </summary>
        public List<Polygon2D> Polygons { get; set; }

        /// <summary>
        /// 矢印オブジェクトのリスト
        /// </summary>
        public List<Arrow2D> Arrows { get; set; }

        /// <summary>
        /// テキストオブジェクトのリスト
        /// </summary>
        public List<Text2D> Texts { get; set; }

        /// <summary>
        /// 弧オブジェクトのリスト
        /// </summary>
        public List<Arc2D> Arcs { get; set; }

        /// <summary>
        /// グラフオブジェクトのリスト
        /// </summary>
        public List<Graph2D> Graphs { get; set; }

        /// <summary>
        /// グループオブジェクトのリスト
        /// </summary>
        public List<Group2D> Groups { get; set; }
    }
}
