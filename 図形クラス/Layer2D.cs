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
    /// <summary>
    /// 2Dグラフィックコントロール上のレイヤーを表すクラス。
    /// </summary>
    public class Layer2D : IComparable<Layer2D>
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
        /// 数式グラフオブジェクトのリスト
        /// </summary>
        public List<MathGraph2D> MathGraphs { get; set; }

        /// <summary>
        /// グループオブジェクトのリスト
        /// </summary>
        public List<Group2D> Groups { get; set; }

        /// <summary>
        /// イメージオブジェクトリスト
        /// </summary>
        public List<Image2D> Images { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Layer2D()
        {
            InitObjects();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="layerName">レイヤー名</param>
        public Layer2D(string layerName)
        {
            ZOrder = 0;
            LayerName = layerName;
            IsVisible = true;

            InitObjects();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="zOrder">Zオーダー</param>
        /// <param name="layerName">レイヤー名</param>
        public Layer2D(int zOrder, string layerName)
        {
            ZOrder = zOrder;
            LayerName = layerName;
            IsVisible = true;

            InitObjects();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="zOrder">Zオーダー</param>
        /// <param name="layerName">レイヤー名</param>
        /// <param name="isVisible">表示状態</param>
        public Layer2D(int zOrder, string layerName, bool isVisible)
        {
            ZOrder = zOrder;
            LayerName = layerName;
            IsVisible = isVisible;

            InitObjects();
        }

        /// <summary>
        /// オブジェクト初期化
        /// </summary>
        public void InitObjects()
        {
            Lines = new List<Line2D>();
            Points = new List<Point2D>();
            Circles = new List<Circle2D>();
            Polygons = new List<Polygon2D>();
            Arrows = new List<Arrow2D>();
            Texts = new List<Text2D>();
            Arcs = new List<Arc2D>();
            Graphs = new List<Graph2D>();
            MathGraphs = new List<MathGraph2D>();
            Groups = new List<Group2D>();
            Images = new List<Image2D>();
        }

        /// <summary>
        /// 全オブジェクトを取得する
        /// </summary>
        /// <returns>全オブジェクト</returns>
        public List<Object2D> GetAllObjects()
        {
            List<Object2D> allObjects = new List<Object2D>();

            allObjects.AddRange(Points);
            allObjects.AddRange(Lines);
            allObjects.AddRange(Circles);
            allObjects.AddRange(Polygons);
            allObjects.AddRange(Arrows);
            allObjects.AddRange(Texts);
            allObjects.AddRange(Arcs);
            allObjects.AddRange(Graphs);
            allObjects.AddRange(MathGraphs);
            allObjects.AddRange(Groups);
            allObjects.AddRange(Images);

            return allObjects;
        }

        /// <summary>
        /// 各図形リストから IsSelect が true の要素をすべて削除する。
        /// </summary>
        public void RemoveSelectedObjects()
        {
            if (Lines != null)
                Lines.RemoveAll(line => line.IsSelect);

            if (Points != null)
                Points.RemoveAll(point => point.IsSelect);

            if (Circles != null)
                Circles.RemoveAll(circle => circle.IsSelect);

            if (Polygons != null)
                Polygons.RemoveAll(polygon => polygon.IsSelect);

            if (Arrows != null)
                Arrows.RemoveAll(arrow => arrow.IsSelect);

            if (Texts != null)
                Texts.RemoveAll(text => text.IsSelect);

            if (Arcs != null)
                Arcs.RemoveAll(arc => arc.IsSelect);

            if (Graphs != null)
                Graphs.RemoveAll(graph => graph.IsSelect);
            
            if (MathGraphs != null)
                MathGraphs.RemoveAll(mathGraph => mathGraph.IsSelect);

            if (Groups != null)
                Groups.RemoveAll(grop => grop.IsSelect);

            if (Images != null)
                Images.RemoveAll(image => image.IsSelect);
        }

        /// <summary>
        /// 比較関数。ZOrderを基準にしてレイヤーを昇順にソートするために使用される。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Layer2D other)
        {
            if (other == null) return 1; // nullは常に小さいとみなす
            // ZOrderを比較して昇順にソート
            return this.ZOrder.CompareTo(other.ZOrder);
        }

        /// <summary>
        /// レイヤーのデータをインポートするメソッド。
        /// Layer2Dオブジェクトからデータを読み取り、Layer2D_Documentのプロパティに設定する。
        /// </summary>
        /// <param name="layer2D_Document">出力先の Layer2D_Document オブジェクト</param>
        internal void OutLayerDocument(out Layer2D_Document layer2D_Document)
        {
            layer2D_Document = new Layer2D_Document();

            layer2D_Document.ZOrder = ZOrder;
            layer2D_Document.LayerName = LayerName;
            layer2D_Document.IsVisible = IsVisible;

            List<Object2D> objects = GetAllObjects();

            ImportObjectsToLayerDoc(ref layer2D_Document, in objects);
        }

        /// <summary>
        /// オブジェクトのデータをインポートするメソッド。
        /// </summary>
        /// <param name="layer2D_Document">インポート先の Layer2D_Document オブジェクト</param>
        /// <param name="objects">オブジェクトデータ</param>
        private void ImportObjectsToLayerDoc(ref Layer2D_Document layer2D_Document, in List<Object2D> objects)
        {
            foreach (var object2D in objects)
            {
                Object2D_Document doc = null;
                object2D.OutDocument(out doc);

                switch (doc.m_Type)
                {
                    case eObject2DType.Line:
                        layer2D_Document.Lines.Add((Line2D_Document)doc);
                        break;
                    case eObject2DType.Point:
                        layer2D_Document.Points.Add((Point2D_Document)doc);
                        break;
                    case eObject2DType.Circle:
                        layer2D_Document.Circles.Add((Circle2D_Document)doc);
                        break;
                    case eObject2DType.Polygon:
                        layer2D_Document.Polygons.Add((Polygon2D_Document)doc);
                        break;
                    case eObject2DType.Arrow:
                        layer2D_Document.Arrows.Add((Arrow2D_Document)doc);
                        break;
                    case eObject2DType.Text:
                        layer2D_Document.Texts.Add((Text2D_Document)doc);
                        break;
                    case eObject2DType.Arc:
                        layer2D_Document.Arcs.Add((Arc2D_Document)doc);
                        break;
                    case eObject2DType.Graph:
                        layer2D_Document.Graphs.Add((Graph2D_Document)doc);
                        break;
                    case eObject2DType.MathGraph:
                        layer2D_Document.Graphs.Add((MathGraph2D_Document)doc);
                        break;
                    case eObject2DType.Group:
                        layer2D_Document.Groups.Add((Group2D_Document)doc);
                        break;
                    case eObject2DType.Image:
                        layer2D_Document.Images.Add((Image2D_Document)doc);
                        break;
                }
            }
        }

        internal void ImportLayerDocument(Layer2D_Document layerDoc, bool IsOnlyObjects = false)
        {
            if (IsOnlyObjects == false)
            {
                ZOrder    = layerDoc.ZOrder;
                LayerName = layerDoc.LayerName;
                IsVisible = layerDoc.IsVisible;
            }

            // オブジェクトのインポート
            ImportObjectDocument(layerDoc);
        }

        internal void ImportObjectDocument(Layer2D_Document layerDoc)
        {
            ImportDocumets<Line2D, Line2D_Document>(Lines, layerDoc.Lines);
            ImportDocumets<Point2D, Point2D_Document>(Points, layerDoc.Points);
            ImportDocumets<Circle2D, Circle2D_Document>(Circles, layerDoc.Circles);
            ImportDocumets<Polygon2D, Polygon2D_Document>(Polygons, layerDoc.Polygons);
            ImportDocumets<Arrow2D, Arrow2D_Document>(Arrows, layerDoc.Arrows);
            ImportDocumets<Text2D, Text2D_Document>(Texts, layerDoc.Texts);
            ImportDocumets<Arc2D, Arc2D_Document>(Arcs, layerDoc.Arcs);
            ImportDocumets<Graph2D, Graph2D_Document>(Graphs, layerDoc.Graphs);
            ImportDocumets<MathGraph2D, MathGraph2D_Document>(MathGraphs, layerDoc.MathGraphs);
            ImportDocumets<Group2D, Group2D_Document>(Groups, layerDoc.Groups);
            ImportDocumets<Image2D, Image2D_Document>(Images, layerDoc.Images);
        }

        internal void ImportDocumets<ObjectList, DocumetList>(List<ObjectList> objectList, List<DocumetList> documentList)
            where ObjectList : Object2D
            where DocumetList : Object2D_Document
        {
            foreach (var doc in documentList)
            {
                ObjectList obj = (ObjectList)Activator.CreateInstance(typeof(ObjectList));
                obj.ImportDocument((DocumetList)doc);
                objectList.Add(obj);
            }
        }
    }
}
