using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;

namespace graphicbox2d
{
    internal class Layer2D_Document
    {
        /// <summary>
        /// レイヤーのZオーダーを表すプロパティ。数値が大きいほど手前に表示される。
        /// </summary>
        public int ZOrder { get; set; }

        /// <summary>
        /// レイヤーの名前を表すプロパティ。レイヤーを識別するための文字列。
        /// </summary>
        public string LayerName { get; set; }

        /// <summary>
        /// レイヤーの表示状態を表すプロパティ。trueの場合は表示、falseの場合は非表示。
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 線分オブジェクトのリスト
        /// </summary>
        public List<Line2D_Document> Lines { get; set; }

        /// <summary>
        /// 点オブジェクトのリスト
        /// </summary>
        public List<Point2D_Document> Points { get; set; }

        /// <summary>
        /// 円オブジェクトのリスト
        /// </summary>
        public List<Circle2D_Document> Circles { get; set; }

        /// <summary>
        /// 多角形オブジェクトのリスト
        /// </summary>
        public List<Polygon2D_Document> Polygons { get; set; }

        /// <summary>
        /// 矢印オブジェクトのリスト
        /// </summary>
        public List<Arrow2D_Document> Arrows { get; set; }

        /// <summary>
        /// テキストオブジェクトのリスト
        /// </summary>
        public List<Text2D_Document> Texts { get; set; }

        /// <summary>
        /// 弧オブジェクトのリスト
        /// </summary>
        public List<Arc2D_Document> Arcs { get; set; }

        /// <summary>
        /// グラフオブジェクトのリスト
        /// </summary>
        public List<Graph2D_Document> Graphs { get; set; }

        /// <summary>
        /// 数式グラフオブジェクトのリスト
        /// </summary>
        public List<MathGraph2D_Document> MathGraphs { get; set; }

        /// <summary>
        /// グループオブジェクトのリスト
        /// </summary>
        public List<Group2D_Document> Groups { get; set; }

        /// <summary>
        /// 画像オブジェクトのリスト
        /// </summary>
        public List<Image2D_Document> Images { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Layer2D_Document()
        {
            Lines = new List<Line2D_Document>();
            Points = new List<Point2D_Document>();
            Circles = new List<Circle2D_Document>();
            Polygons = new List<Polygon2D_Document>();
            Arrows = new List<Arrow2D_Document>();
            Texts = new List<Text2D_Document>();
            Arcs = new List<Arc2D_Document>();
            Graphs = new List<Graph2D_Document>();
            MathGraphs = new List<MathGraph2D_Document>();
            Groups = new List<Group2D_Document>();
            Images = new List<Image2D_Document>();
        }
    }
}
