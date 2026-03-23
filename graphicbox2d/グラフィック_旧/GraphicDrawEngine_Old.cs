using graphicbox2d.その他;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graphicbox2d
{
    /// <summary>
    /// グラフィック描画クラス
    /// </summary>
    internal class GraphicDrawEngine_Old : IDisposable
    {
        // ===============================================================================
        // メンバプロパティ
        // ===============================================================================

        /// <summary>
        /// 親の Graphic2DControl オブジェクト
        /// </summary>
        internal readonly Graphic2DControl m_Parent;

        /// <summary>
        /// 選択中の図形の点線スタイル
        /// </summary>
        private  float [] SELECT_BOX_DASH_STYLE = new float[] { 3, 5 };

        /// <summary>
        /// 破棄済フラグ
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 全ての破棄対象のオブジェクトリストを取得する
        /// </summary>
        private List<IDisposable> m_AllDisposeObjects => new List<IDisposable>()
        {
            _SelectBoxPen,          // オブジェクト描画用ペン
            m_GridManager,           // 背景のグリッドマネージャー  
        };

        /// <summary>
        /// オブジェクト描画用のペン
        /// </summary>
        private Pen m_SelectBoxPen => GetSelectBoxPen();
        private Pen _SelectBoxPen = new Pen(Color.White, 1);

        /// <summary>
        /// 背景のグリッドマネージャー
        /// </summary>
        internal readonly GridManager_Old m_GridManager;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GraphicDrawEngine_Old(Graphic2DControl Parent)
        {
            m_Parent                  = Parent;
            m_GridManager             = new GridManager_Old(Parent, this);
            _SelectBoxPen.DashStyle   = DashStyle.Custom;
            _SelectBoxPen.DashPattern = SELECT_BOX_DASH_STYLE;
        }

        /// <summary>
        /// IDisposable 実装
        /// </summary>
        public void Dispose()
        {
            if (_disposed == true)
            {
                return;
            }

            // 描画用オブジェクトを全て解放する
            foreach (var obj in m_AllDisposeObjects)
            {
                obj.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// 2Dオブジェクトを描画する関数。
        /// Object2D の種類 (Line, Point, Circle, Polygon, Arrow) に応じて、
        /// 適切な描画メソッドを呼び出し Graphics オブジェクトに描画する。
        /// null が渡された場合は ArgumentNullException をスローする。
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト。</param>
        /// <param name="object2D">描画する Object2D インスタンス。種類に応じて内部で振り分けられる。</param>
        /// <exception cref="ArgumentNullException">
        /// object2D が null の場合、または未対応の種類が指定された場合にスローされる。
        /// </exception>
        public void DrawObject2D(Graphics g, Object2D object2D)
        {
            if (object2D == null)
            {
                throw new ArgumentNullException(nameof(object2D));
            }

            switch (object2D.m_Type)
            {
                case eObject2DType.Line:
                    DrawLine2D(g, object2D as Line2D);
                    break;
                case eObject2DType.Point:
                    DrawPoint2D(g, object2D as Point2D);
                    break;
                case eObject2DType.Circle:
                    DrawCircle2D(g, object2D as Circle2D);
                    break;
                case eObject2DType.Polygon:
                    DrawPolygon2D(g, object2D as Polygon2D);
                    break;
                case eObject2DType.Arrow:
                    DrawArrow2D(g, object2D as Arrow2D);
                    break;
                case eObject2DType.Text:
                    DrawText2D(g, object2D as Text2D);
                    break;
                case eObject2DType.Arc:
                    DrawArc2D(g, object2D as Arc2D);
                    break;
                case eObject2DType.Graph:
                    DrawGraph2D(g, object2D as Graph2D);
                    break;
                default:
                    throw new ArgumentNullException(nameof(object2D));
            }
        }

        /// <summary>
        /// 2Dラインを描画する関数。
        /// グリッド座標系で指定された始点・終点をクライアント座標に変換し、
        /// 指定されたペンを用いて Graphics オブジェクトに直線を描画する。
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト。</param>
        /// <param name="line">グリッド座標系で定義された Line2D オブジェクト。</param>
        public void DrawLine2D(Graphics g, Line2D line)
        {
            // グリッド座標をクライアント座標に変換
            PointF ClientStart = GraphicCaluculate_Old.ConvertGridPointToClientPoint(line.Start);
            PointF ClientEnd   = GraphicCaluculate_Old.ConvertGridPointToClientPoint(line.End);

            Pen pen = GetPen(line);
            g.DrawLine(pen, ClientStart, ClientEnd);

            if (line.IsSelect == true)
            {
                PointF[] points = GraphicCaluculate_Old.GetBoundingBoxLine(ClientStart, ClientEnd, line.Width);
                g.DrawPolygon(m_SelectBoxPen, points);
            }
        }

        /// <summary>
        /// 2Dポイントを描画する関数。
        /// グリッド座標系で指定された点をクライアント座標に変換し、
        /// 半径をスケーリングして塗りつぶし円として描画する。
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト。</param>
        /// <param name="point">グリッド座標系で定義された Point2D オブジェクト。</param>
        public void DrawPoint2D(Graphics g, Point2D point)
        {
            DrawCircle2D(g, point);
        }

        /// <summary>
        /// 2D円を描画する関数。
        /// グリッド座標系で指定された中心点と半径をクライアント座標に変換し、
        /// 塗りつぶし／非塗りつぶしの状態に応じて円を描画する。
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト。</param>
        /// <param name="circle">グリッド座標系で定義された Circle2D オブジェクト。</param>
        public void DrawCircle2D(Graphics g, Circle2D circle)
        {
            //=============
            // 値を保持
            SmoothingMode prevSmoothingMode = g.SmoothingMode;

            // アンチエイリアスを有効化
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // グリッド座標をクライアント座標に変換
            PointF pointF       = new PointF(circle.X, circle.Y);
            PointF ClientPointF = GraphicCaluculate_Old.ConvertGridPointToClientPoint(pointF);
            float ClientR       = circle.R * m_Parent.DisplayGridWidth;

            if (circle.IsFilled == false)
            {
                Pen pen;

                pen = GetPen(circle);

                g.DrawCircle(pen, ClientPointF.X, ClientPointF.Y, ClientR);
            }
            else
            {
                Brush Brush = GetBrush(circle);

                g.DrawFillCircleWithOutLine(Brush, ClientPointF.X, ClientPointF.Y, ClientR);
            }

            if (circle.IsSelect == true)
            {
                PointF[] points = GraphicCaluculate_Old.GetBoundingBoxCircle(ClientPointF.X, ClientPointF.Y, ClientR);
                g.DrawPolygon(m_SelectBoxPen, points);
            }

            //=============
            // 値を復元
            g.SmoothingMode = prevSmoothingMode;
        }

        /// <summary>
        /// 2Dポリゴンを描画する関数。
        /// グリッド座標系で指定された頂点群をクライアント座標に変換し、
        /// 塗りつぶし／非塗りつぶしの状態に応じて多角形を描画する。
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト。</param>
        /// <param name="polygon">グリッド座標系で定義された Polygon2D オブジェクト。</param>
        public void DrawPolygon2D(Graphics g, Polygon2D polygon)
        {
            // グリッド座標をクライアント座標に変換
            PointF[] ClientPoints = polygon.Points.Select(pt => GraphicCaluculate_Old.ConvertGridPointToClientPoint(pt)).ToArray();

            if (polygon.IsFilled == false)
            {
                Pen pen = GetPen(polygon);

                g.DrawPolygon(pen, ClientPoints);
            }
            else
            {
                Brush Brush = GetBrush(polygon);

                g.FillPolygonWithOutLine(Brush, ClientPoints);
            }

            if (polygon.IsSelect == true)
            {
                PointF[] points = GraphicCaluculate_Old.GetBoundingBoxPolygon(ClientPoints);
                g.DrawPolygon(m_SelectBoxPen, points);
            }
        }

        /// <summary>
        /// 2D矢印を描画する関数。
        /// グリッド座標系で指定された始点・終点をクライアント座標に変換し、
        /// 矢印用のペンを用いて直線を描画する。
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト。</param>
        /// <param name="arrow">グリッド座標系で定義された Arrow2D オブジェクト。</param>
        public void DrawArrow2D(Graphics g, Arrow2D arrow)
        {
            // グリッド座標をクライアント座標に変換
            PointF ClientStart = GraphicCaluculate_Old.ConvertGridPointToClientPoint(arrow.Start);
            PointF ClientEnd   = GraphicCaluculate_Old.ConvertGridPointToClientPoint(arrow.End);

            Pen pen = GetArrowPen(arrow);
            g.DrawLine(pen, ClientStart, ClientEnd);

            // 選択中の場合はハイライト描画
            if (arrow.IsSelect == true)
            {
                PointF[] points = GraphicCaluculate_Old.GetBoundingBoxLine(ClientStart, ClientEnd, arrow.Width);
                g.DrawPolygon(m_SelectBoxPen, points);
            }
        }


        /// <summary>
        /// 指定された Text2D オブジェクトを 2D 描画するメソッド。
        /// グリッド座標をクライアント座標に変換し、指定角度で回転させて文字列を描画する。
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト。</param>
        /// <param name="text">描画する文字列情報を保持する Text2D オブジェクト。</param>
        public void DrawText2D(Graphics g, Text2D text)
        {
            // グリッド座標をクライアント座標に変換
            PointF pointF = new PointF(text.X, text.Y);
            PointF ClientPoint = GraphicCaluculate_Old.ConvertGridPointToClientPoint(pointF);

            Brush Brush = GetBrush(text);

            if (Comp.IsEqual(text.Angle, 0) == true)
            {
                // 回転が無い場合はそのまま描画
                Font font = DrawManager_Old.GetFont(text.FontName, text.DrawFontSize);
                g.DrawString(text.Text, font, Brush, ClientPoint.X, ClientPoint.Y);
            }
            else
            {
                // 回転の中心を決める
                g.TranslateTransform(ClientPoint.X, ClientPoint.Y);

                // 角度を指定
                g.RotateTransform(-1 * text.Angle);

                Font font = DrawManager_Old.GetFont(text.FontName, text.DrawFontSize);

                // 描画（座標は回転後の原点からの位置）
                g.DrawString(text.Text, font, Brush, 0, 0);

                // 変換をリセット
                g.ResetTransform();
            }

            if (text.IsSelect == true)
            {
                PointF[] points = GraphicCaluculate_Old.GetBoundingBoxText(ClientPoint.X, ClientPoint.Y, text.Text, text.DrawFontSize, text.FontName, text.Angle);
                g.DrawPolygon(m_SelectBoxPen, points);
            }
        }

        /// <summary>
        /// Arc2D オブジェクトを描画するメソッド。
        /// グリッド座標をクライアント座標に変換し、
        /// 塗りつぶし指定がある場合は円弧を塗りつぶし＋アウトライン描画、
        /// 指定がない場合は輪郭線のみを描画する。
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト。</param>
        /// <param name="arc">
        /// 描画対象の Arc2D オブジェクト。  
        /// - X, Y : グリッド座標  
        /// - R : 半径（グリッド単位）  
        /// - StartAngle, EndAngle : 円弧の角度範囲  
        /// - IsFilled : 塗りつぶし有無  
        /// - その他、ペンやブラシ情報を保持  
        /// </param>

        public void DrawArc2D(Graphics g, Arc2D arc)
        {
            //=============
            // 値を保持
            SmoothingMode prevSmoothingMode = g.SmoothingMode;

            // アンチエイリアスを有効化
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // グリッド座標をクライアント座標に変換
            PointF pointF       = new PointF(arc.X, arc.Y);
            PointF ClientPointF = GraphicCaluculate_Old.ConvertGridPointToClientPoint(pointF);
            float ClientR       = arc.R * m_Parent.DisplayGridWidth;

            if (arc.IsFilled == false)
            {
                Pen pen = GetPen(arc);

                g.DrawArc(pen, ClientPointF, ClientR, -arc.StartAngle, -arc.EndAngle, arc.IsDrawSideLines);
            }
            else
            {
                Brush Brush = GetBrush(arc);

                g.DrawFillArc(Brush, ClientPointF, ClientR, -arc.StartAngle, -arc.EndAngle, arc.IsDrawSideLines);
            }

            if (arc.IsSelect == true)
            {
                PointF[] points = GraphicCaluculate_Old.GetBoundingBoxCircle(ClientPointF.X, ClientPointF.Y, ClientR);
                g.DrawPolygon(m_SelectBoxPen, points);
            }

            //=============
            // 値を復元
            g.SmoothingMode = prevSmoothingMode;
        }

        /// <summary>
        /// グラフを描画する関数。
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト。</param>
        /// <param name="graph">描画対象グラフ図形オブジェクト</param>
        public void DrawGraph2D(Graphics g, Graph2D graph)
        {
            // 点が無い場合は描画しない
            if (graph.Points.Count == 0)
            {
                return;
            }

            //=============
            // 値を保持
            SmoothingMode prevSmoothingMode = g.SmoothingMode;

            // アンチエイリアスを有効化
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // グリッド座標をクライアント座標に変換
            PointF[] ClientPoints = graph.Points.Select(pt => GraphicCaluculate_Old.ConvertGridPointToClientPoint(pt)).ToArray();

            Pen pen = GetPen(graph);

            g.DrawCurve(pen, ClientPoints);

            if (graph.IsSelect == true)
            {
                PointF[] points = GraphicCaluculate_Old.GetBoundingBoxPolygon(ClientPoints);
                g.DrawPolygon(m_SelectBoxPen, points);
            }

            //=============
            // 値を復元
            g.SmoothingMode = prevSmoothingMode;
        }

        /// <summary>
        /// オブジェクトに合わせたペンを自動取得
        /// </summary>
        /// <param name="object2D">オブジェクト</param>
        /// <returns>ペン</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Pen GetPen(Object2D object2D)
        {
            if (object2D == null)
            {
                throw new ArgumentNullException(nameof(object2D));
            }

            int lineWidth       = 1;
            LineStyle lineStyle = LineStyle.Solid;
            Color color         = Color.Black;

            if (object2D is ILineProperty)
            {
                lineWidth = (object2D as ILineProperty).Width;
                lineStyle = (object2D as ILineProperty).Style;
                color     = (object2D as ILineProperty).Color;
            }

            Pen pen = DrawManager_Old.GetPen(color, lineWidth, (DashStyle)lineStyle);

            return pen;
        }

        /// <summary>
        /// 矢印図形に合わせたペンを自動取得
        /// </summary>
        /// <param name="arrow2D">矢印図形</param>
        /// <returns>ペン</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Pen GetArrowPen(Arrow2D arrow2D)
        {
            if (arrow2D == null)
            {
                throw new ArgumentNullException(nameof(arrow2D));
            }

            Pen pen = DrawManager_Old.GetArrowPen(arrow2D.Color, arrow2D.Width, (DashStyle)arrow2D.Style, arrow2D.StartCap, arrow2D.EndCap);

            return pen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Pen GetSelectBoxPen()
        {
            _SelectBoxPen.Color = m_Parent.BackColor.Invert();

            return _SelectBoxPen;
        }

        /// <summary>
        /// 図形オブジェクトに合わせたブラシを自動取得
        /// </summary>
        /// <param name="object2D">図形オブジェクト</param>
        /// <returns>ブラシ</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Brush GetBrush(Object2D object2D)
        {
            if (object2D == null)
            {
                throw new ArgumentNullException(nameof(object2D));
            }

            Color color = Color.Black;

            if (object2D is IFillProperty)
            {
                color = (object2D as IFillProperty).FillColor;
            }

            Brush brush = DrawManager_Old.GetSolidBrush(color);

            return brush;
        }

        /// <summary>
        /// 背景のグリッド線を描画
        /// </summary>
        /// <param name="g"></param>
        public void DrawBackGroundGrid(Graphics g)
        {
            // グリッド線を描画
            DrawGrid(g);

            // 中央線を描画
            DrawCenterLine(g);
        }

        /// <summary>
        /// 背景のグリッド座標テキストを描画
        /// </summary>
        /// <param name="g"></param>
        public void DrawBackGroundGridText(Graphics g)
        {
            // グリッド座標テキストを描画
            DrawGridText(g);
            // 原点の0テキストを描画
            DrawCenterText(g);
        }

        /// <summary>
        /// マウス位置・スケーリング情報テキストを描画
        /// </summary>
        /// <param name="g"></param>
        public void DrawInfoText(Graphics g)
        {
            Font font = m_Parent.InfoTextFont;
            Brush brush = DrawManager_Old.GetSolidBrush(m_Parent.ForeColor);

            PointF gridMousePoint = GetGridMousePoint();

            string mousePosText = string.Format("Mouse Position : X={0:0.0000}, Y={1:0.0000}", gridMousePoint.X, gridMousePoint.Y);
            string scaleText    = string.Format("Zoom : {0:0.00}%", Graphic2DControl.UserZoom * 100);

            string otherText = "";

            if (m_Parent.IsCaluculatingSusiki == true)
            {
                otherText = "Info : Calculating formula・・・・";
            }

            PointF textPoint;

            // マウス位置テキストの描画
            textPoint = GetDrawInfoTextPosition(g, mousePosText, 1);
            g.DrawString(mousePosText, font, brush, textPoint.X, textPoint.Y);

            // スケーリングテキストの描画
            textPoint = GetDrawInfoTextPosition(g, scaleText, 2);
            g.DrawString(scaleText, font, brush, textPoint.X, textPoint.Y);

            // その他テキストの描画
            textPoint = GetDrawInfoTextPosition(g, otherText, 3);
            g.DrawString(otherText, font, brush, textPoint.X, textPoint.Y);

        }

        /// <summary>
        /// 情報テキストの表示位置データを更新
        /// </summary>
        /// <param name="g"></param>
        public PointF GetDrawInfoTextPosition(Graphics g, string Text, int OutRecordNo)
        {
            if (string.IsNullOrEmpty(Text) == true)
            {
                return default;
            }

            // コントロールの右下に表示
            SizeF textSize = g.MeasureString(Text, m_Parent.InfoTextFont);

            // マウス位置テキストの表示位置の更新
            PointF TextPt = new PointF();
            TextPt.X = m_Parent.ClientSize.Width - textSize.Width - 5;
            TextPt.Y = m_Parent.ClientSize.Height - OutRecordNo * textSize.Height - 5;

            return TextPt;
        }

        /// <summary>
        /// グリッドのマウス位置を取得する
        /// </summary>
        /// <returns></returns>
        private PointF GetGridMousePoint()
        {
            // クライアント座標のマウス位置を取得
            Point ClientMousePoint = m_Parent.PointToClient(Cursor.Position);
            // グリッド座標に変換
            PointF GridMousePoint = GraphicCaluculate_Old.ConvertClientPointToGridPoint(ClientMousePoint);

            return GridMousePoint;
        }

        /// <summary>
        /// 指定された Graphics オブジェクトに中央線（縦・横両方）を描画する
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト</param>
        private void DrawCenterLine(Graphics g)
        {
            Point Start;
            Point End;

            // X軸中心線の描画
            m_GridManager.GetXAxisCenterLine(out Start, out End);

            g.DrawLine(m_GridManager.GridXAxisCenterPen, Start, End);

            // Y軸中心線の描画
            m_GridManager.GetYAxisCenterLine(out Start, out End);

            g.DrawLine(m_GridManager.GridYAxisCenterPen, Start, End);
        }

        /// <summary>
        /// 指定された Graphics オブジェクトに原点のテキストを描画する
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト</param>
        private void DrawCenterText(Graphics g)
        {
            // 原点の0テキストの描画
            g.DrawText(m_GridManager.CenterTextData, m_GridManager.GridFont, m_GridManager.GridFontBrush);
        }

        /// <summary>
        /// 指定された Graphics オブジェクトにグリッドを描画する
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト</param>
        private void DrawGrid(Graphics g)
        {
            g.DrawPath(m_GridManager.GridPen, m_GridManager.GridPath);
        }

        /// <summary>
        /// 指定された Graphics オブジェクトにグリッド座標テキストを描画する
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト</param>
        private void DrawGridText(Graphics g)
        {
            g.DrawTexts(m_GridManager.GridTexts, m_GridManager.GridFont, m_GridManager.GridFontBrush);
        }
    }
}
