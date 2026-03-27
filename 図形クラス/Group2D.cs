using graphicbox2d.グラフィック計算;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace graphicbox2d
{
    /// <summary>
    /// グループ図形クラス。複数の図形をまとめてグループ化するためのクラス。
    /// </summary>
    public class Group2D : Object2D
    {
        // =======================================================================
        // 公開プロパティ
        // =======================================================================
        public override eObject2DType m_Type => eObject2DType.Group;

        /// <summary>
        /// グループ化する図形のリスト
        /// </summary>
        public List<Group2DItem> ObjectList { get; set; } = new List<Group2DItem>();


        // ===============================================================================
        // 非公開プロパティ
        // ===============================================================================

        /// <summary>
        /// グループ内のすべての図形のバウンディングボックスの頂点を取得するプロパティ
        /// </summary>
        internal List<PointF> AllBoundingBoxPoints { get { return GetAllBoundingBoxPoints(); } }

        /// <summary>
        /// 図形の中心点
        /// </summary>
        internal override Vector2 CenterPoint { get { return GetCenterPoint(); } }

        /// <summary>
        /// ポリゴン図形を完全内包する円（外接円）の半径
        /// </summary>
        internal float CircumCircleR { get { return GetCircumCircleR(); } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Group2D() : base()
        {
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~Group2D()
        {
            ObjectList.Clear();
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns>コピーしたオブジェクト</returns>
        public override Object2D Clone()
        {
            Group2D clone = new Group2D();
            // 基底クラスのデータをコピー
            this.BaseCopyDataTo(clone);
            // 派生クラスのデータをコピー
            foreach (var item in ObjectList)
            {
                Group2DItem CloneGroupItem = new Group2DItem { Object = item.Object.Clone(), ZOrder = item.ZOrder };
                clone.ObjectList.Add(CloneGroupItem);
            }

            return clone;
        }

        /// <summary>
        /// ObjectList を空にしたコピーを作成する（グループの構造だけをコピーする）
        /// </summary>
        /// <returns></returns>
        internal Object2D CloneWithoutObjectList()
        {
            Group2D clone = new Group2D();
            // 基底クラスのデータをコピー
            this.BaseCopyDataTo(clone);

            return clone;
        }

        /// <summary>
        /// マウスヒット中の図形（拡大した図形）を返す。
        /// </summary>
        /// <returns>拡大された図形</returns>
        internal override Object2D GetHitObject()
        {
            Group2D HitGroup = CloneWithoutObjectList() as Group2D;

            foreach (var item in ObjectList)
            {
                Object2D HitObject = item.Object.GetHitObject();

                Group2DItem HitGroupItem = new Group2DItem { Object = HitObject, ZOrder = item.ZOrder };

                HitGroup.ObjectList.Add(HitGroupItem);
            }

            return HitGroup;
        }

        /// <summary>
        /// マウスポイントがこの図形にヒットしているかどうかを判定する。
        /// </summary>
        /// <param name="X">マウス座標の X 値</param>
        /// <param name="Y">マウス座標の Y 値</param>
        /// <param name="MusePointRange">ヒット判定の許容範囲（半径などに利用）</param>
        /// <returns>ヒットしていれば true、そうでなければ false</returns>
        internal override eMouseHitType IsHitMousePoint(PointF MousePoint, float MusePointRange)
        {
            eMouseHitType HitType = eMouseHitType.None;

            foreach (var item in ObjectList)
            {
                eMouseHitType ItemHitType = item.Object.IsHitMousePoint(MousePoint, MusePointRange);
                if (ItemHitType == eMouseHitType.MousePointOnObject)
                {
                    HitType = eMouseHitType.MousePointOnObject;
                    break;
                }
                else if (ItemHitType == eMouseHitType.CrossMouseRange)
                {
                    HitType = eMouseHitType.CrossMouseRange;
                }
            }

            return HitType;
        }

        /// <summary>
        /// マウスポイントとこの図形との距離を返す。
        /// </summary>
        /// <param name="X">マウス座標の X 値</param>
        /// <param name="Y">マウス座標の Y 値</param>
        internal override float GetDistanceHitMousePoint(float X, float Y)
        {
            Vector2 MousePoint = new Vector2(X, Y);

            return Vector2.Distance(MousePoint, CenterPoint);
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
        /// <param name="Movement">移動量</param>
        internal override void Move(PointF Movement)
        {
            foreach (var item in ObjectList)
            {
                item.Object.Move(Movement);
            }
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
        /// <param name="X">移動量X</param>
        /// <param name="Y">移動量Y</param>
        internal override void Move(float X, float Y)
        {
            foreach (var item in ObjectList)
            {
                item.Object.Move(X, Y);
            }
        }

        /// <summary>
        /// バウンディングボックスを取得する
        /// </summary>
        internal override PointF[] GetBoundingBox()
        {
            return CalBoundBox.GetBoundingBoxPolygon(AllBoundingBoxPoints.ToArray());
        }

        /// <summary>
        /// ポリゴンの中心点と外接円の半径を同時に取得する
        /// </summary>
        /// <param name="centerPoint">ポリゴンの中心点</param>
        /// <param name="circumCircleR">外接円の半径</param>
        internal void GetCenterPointAndCircumCircleR(out Vector2 centerPoint, out float circumCircleR)
        {
            centerPoint = GraphicCaluculate.CaluculateCenterPoint(AllBoundingBoxPoints);
            circumCircleR = GraphicCaluculate.CaluculateCircumCircleR(centerPoint, AllBoundingBoxPoints);
        }

        /// <summary>
        /// 中心点を取得する
        /// </summary>
        /// <returns></returns>
        internal Vector2 GetCenterPoint()
        {
            return GraphicCaluculate.CaluculateCenterPoint(AllBoundingBoxPoints);
        }

        /// <summary>
        /// 外接円を取得する
        /// </summary>
        /// <returns></returns>
        internal float GetCircumCircleR()
        {
            Vector2 centerPoint = GraphicCaluculate.CaluculateCenterPoint(AllBoundingBoxPoints);

            return GraphicCaluculate.CaluculateCircumCircleR(centerPoint, AllBoundingBoxPoints);
        }

        /// <summary>
        /// グループ内のすべての図形のバウンディングボックスの頂点を取得する
        /// </summary>
        /// <returns></returns>
        private List<PointF> GetAllBoundingBoxPoints()
        {
            List<PointF> AllPoints = new List<PointF>();
            foreach (var item in ObjectList)
            {
                PointF[] BoundingBox = item.Object.GetBoundingBox();
                if (BoundingBox != null)
                {
                    AllPoints.AddRange(BoundingBox);
                }
            }
            return AllPoints;
        }


        /// <summary>
        /// ドキュメントデータを取り込む
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        public override void ImportDocument(in Object2D_Document doc)
        {
            _ID = doc._ID;
            IsSelect = doc.IsSelect;
            ZOrder = doc.ZOrder;

            Group2D_Document GroupDoc = doc as Group2D_Document;

            foreach (var itemDoc in GroupDoc.ObjectList)
            {
                Group2DItem GroupItem = new Group2DItem();
                GroupItem.ImportDoc(itemDoc);
                ObjectList.Add(GroupItem);
            }
        }

        /// <summary>
        /// ドキュメントデータを書き出す
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        public override void OutDocument(out Object2D_Document doc)
        {
            doc = new Group2D_Document();

            // 基底クラスのデータをコピー
            doc._ID = _ID;
            doc.IsSelect = IsSelect;
            doc.ZOrder = ZOrder;

            // 派生クラスのデータをコピー
            foreach (var item in ObjectList)
            {
                Group2DItem_Document GroupItemDoc = item.GetDoc();

                (doc as Group2D_Document).ObjectList.Add(GroupItemDoc);
            }
        }
    }

    /// <summary>
    /// グループ図形のアイテムを表すクラス。
    /// </summary>
    public class Group2DItem : IComparable<Group2DItem>
    {
        /// <summary>
        /// オブジェクト
        /// </summary>
        public Object2D Object { get; set; }

        /// <summary>
        /// Zオーダー（グループ内の描画順序を指定するための値。大きいほど前面に描画される）
        /// </summary>
        public int ZOrder { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Group2DItem()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="groupOrder"></param>
        public Group2DItem(Object2D obj, int groupOrder)
        {
            Object = obj;
            ZOrder = groupOrder;
        }

        /// <summary>
        /// 比較関数。グループ内の描画順序でソートするために使用される。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Group2DItem other)
        {
            if (other == null) return 1; // nullは常に小さいとみなす
            // GroupOrderを比較して昇順にソート
            return this.ZOrder.CompareTo(other.ZOrder);
        }

        /// <summary>
        /// ドキュメントデータを書き出す
        /// </summary>
        /// <returns>ドキュメントデータ</returns>
        public Group2DItem_Document GetDoc()
        {
            Group2DItem_Document doc = new Group2DItem_Document();
            doc.ZOrder = ZOrder;

            Object2D_Document ObjectDoc = null;
            Object.OutDocument(out ObjectDoc);
            doc.Object = ObjectDoc;

            return doc;
        }

        /// <summary>
        /// ドキュメントデータを取り込む
        /// </summary>
        /// <param name="doc"></param>
        public void ImportDoc(Group2DItem_Document doc)
        {
            ZOrder = doc.ZOrder;

            switch (doc.Object.m_Type)
            {
                case eObject2DType.Point:
                    Object = new Point2D();
                    break;
                case eObject2DType.Line:
                    Object = new Line2D();
                    break;
                case eObject2DType.Circle:
                    Object = new Circle2D();
                    break;
                case eObject2DType.Polygon:
                    Object = new Polygon2D();
                    break;
                case eObject2DType.Arrow:
                    Object = new Arrow2D();
                    break;
                case eObject2DType.Text:
                    Object = new Text2D();
                    break;
                case eObject2DType.Arc:
                    Object = new Arc2D();
                    break;
                case eObject2DType.Graph:
                    Object = new Graph2D();
                    break;
                case eObject2DType.MathGraph:
                    Object = new MathGraph2D();
                    break;
                case eObject2DType.Group:
                    Object = new Group2D();
                    break;
            }

            Object.ImportDocument(doc.Object);
        }
    }
}
