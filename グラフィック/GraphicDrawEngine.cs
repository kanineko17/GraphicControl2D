using graphicbox2d.オブジェクトマネージャー;
using graphicbox2d.グラフィック計算;
using graphicbox2d.グローバル変数;
using graphicbox2d.その他;
using graphicbox2d.描画図形クラス;
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
using static System.Net.Mime.MediaTypeNames;

namespace graphicbox2d
{
    /// <summary>
    /// グラフィック描画クラス
    /// </summary>
    internal class GraphicDrawEngine : IDisposable
    {
        // ===============================================================================
        // メンバプロパティ
        // ===============================================================================

        /// <summary>
        /// 親の Graphic2DControl オブジェクト
        /// </summary>
        internal readonly Graphic2DControl m_Parent;

        /// <summary>
        /// 破棄済フラグ
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 全ての破棄対象のオブジェクトリストを取得する
        /// </summary>
        private List<IDisposable> m_AllDisposeObjects => new List<IDisposable>()
        {
            _SelectBoxPaint,          // オブジェクト描画用ペン
            m_GridManager,           // 背景のグリッドマネージャー  
        };

        /// <summary>
        /// オブジェクト描画用のペン
        /// </summary>
        private SKPaint m_SelectBoxPaint => GetSelectBoxSKPaint();
        private SKPaint _SelectBoxPaint = DrawManager.GetLineSkPaint(Color.White, 1, false, LineStyle.SelectBoxDash);

        /// <summary>
        /// 背景のグリッドマネージャー
        /// </summary>
        internal readonly GridManager m_GridManager;

        // ===============================================================================
        // 定数
        // ===============================================================================

        /// <summary>
        /// 数式計算中文字列
        /// </summary>
        private const string CALUCULATING_TEXT = "Info : Calculating formula....";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GraphicDrawEngine(Graphic2DControl Parent)
        {
            m_Parent                  = Parent;
            m_GridManager             = new GridManager(Parent, this);
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
        /// <param name="sKCanvas">描画対象の SKCanvas オブジェクト。</param>
        /// <param name="object2D">描画する Object2D インスタンス。種類に応じて内部で振り分けられる。</param>
        /// <param name="drawType">描画タイプ(通常: Normal, マウスヒット時: Hit)</param>
        /// <exception cref="ArgumentNullException">
        /// object2D が null の場合、または未対応の種類が指定された場合にスローされる。
        /// </exception>
        public void DrawObject2D(SKCanvas sKCanvas, Object2D object2D, eDrawFigureType drawType)
        {
            if (object2D == null)
            {
                throw new ArgumentNullException(nameof(object2D));
            }

            switch (object2D.m_Type)
            {
                case eObject2DType.Line:
                    DrawLine2D(sKCanvas, object2D as Line2D, drawType);
                    break;
                case eObject2DType.Point:
                    DrawPoint2D(sKCanvas, object2D as Point2D, drawType);
                    break;
                case eObject2DType.Circle:
                    DrawCircle2D(sKCanvas, object2D as Circle2D, drawType);
                    break;
                case eObject2DType.Polygon:
                    DrawPolygon2D(sKCanvas, object2D as Polygon2D, drawType);
                    break;
                case eObject2DType.Arrow:
                    DrawArrow2D(sKCanvas, object2D as Arrow2D, drawType);
                    break;
                case eObject2DType.Text:
                    DrawText2D(sKCanvas, object2D as Text2D, drawType);
                    break;
                case eObject2DType.Arc:
                    DrawArc2D(sKCanvas, object2D as Arc2D, drawType);
                    break;
                case eObject2DType.Graph:
                case eObject2DType.MathGraph:
                    DrawGraph2D(sKCanvas, object2D as Graph2D, drawType);
                    break;
                case eObject2DType.Group:
                    DrawGroup2D(sKCanvas, object2D as Group2D, drawType);
                    break;
                case eObject2DType.Image:
                    DrawImage2D(sKCanvas, object2D as Image2D, drawType);
                    break;
                default:
                    throw new ArgumentNullException(nameof(object2D));
            }
        }

        /// <summary>
        /// 2Dラインを描画する関数。
        /// グリッド座標系で指定された始点・終点をクライアント座標に変換し、
        /// 指定された SKPaint を用いて SKCanvas に直線を描画する。
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas。</param>
        /// <param name="line">グリッド座標系で定義された Line2D オブジェクト。</param>
        /// <param name="drawType">描画タイプ(通常: Normal, マウスヒット時: MouseHit)</param>
        public void DrawLine2D(SKCanvas canvas, Line2D line, eDrawFigureType drawType)
        {
            // グリッド座標をクライアント座標に変換
            Line2D_DrawFigure figure = line.GetDrawFigure(drawType) as Line2D_DrawFigure;

            SKPaint paint = GetLineSKPaint(line, drawType);

            // 直線描画
            canvas.DrawLine(figure.Start, figure.End, paint);

            // 選択状態なら選択ボックスを描画
            if (line.IsSelect == true)
            {
                SKPoint[] points = CalBoundBox.GetBoundingBoxLineSK(figure.Start, figure.End, line.Width);

                canvas.DrawPolygon(points, m_SelectBoxPaint);
            }
        }

        /// <summary>
        /// 2Dポイントを描画する関数。
        /// グリッド座標系で指定された点をクライアント座標に変換し、
        /// 半径をスケーリングして塗りつぶし円として描画する。
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト。</param>
        /// <param name="point">グリッド座標系で定義された Point2D オブジェクト。</param>
        /// <param name="drawType">描画タイプ(通常: Normal, マウスヒット時: Hit)</param>
        public void DrawPoint2D(SKCanvas canvas, Point2D point, eDrawFigureType drawType)
        {
            DrawCircle2D(canvas, point, drawType);
        }

        /// <summary>
        /// 2D円を描画する関数。
        /// グリッド座標系で指定された中心点と半径をクライアント座標に変換し、
        /// 塗りつぶし／非塗りつぶしの状態に応じて円を描画する。
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト。</param>
        /// <param name="circle">グリッド座標系で定義された Circle2D オブジェクト。</param>
        /// <param name="drawType">描画タイプ(通常: Normal, マウスヒット時: Hit)</param>
        public void DrawCircle2D(SKCanvas canvas, Circle2D circle, eDrawFigureType drawType)
        {
            Circle2D_DrawFigure figure = circle.GetDrawFigure(drawType) as Circle2D_DrawFigure;

            if (circle.IsFilled == true)
            {
                SKPaint paint = GetFillSKPaint(circle);

                canvas.DrawCircle(figure.X, figure.Y, figure.R, paint);
            }

            if (circle.IsDrawLine == true)
            {
                SKPaint paint = GetLineSKPaint(circle, drawType);

                canvas.DrawCircle(figure.X, figure.Y, figure.R, paint);
            }

            if (circle.IsSelect == true)
            {
                SKPoint[] points = CalBoundBox.GetBoundingBoxCircleSK(figure.X, figure.Y, figure.R);
                canvas.DrawPolygon(points, m_SelectBoxPaint);
            }
        }

        /// <summary>
        /// 2Dポリゴンを描画する関数。
        /// グリッド座標系で指定された頂点群をクライアント座標に変換し、
        /// 塗りつぶし／非塗りつぶしの状態に応じて多角形を描画する。
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト。</param>
        /// <param name="polygon">グリッド座標系で定義された Polygon2D オブジェクト。</param>
        /// <param name="drawType">描画タイプ(通常: Normal, マウスヒット時: Hit)</param>
        public void DrawPolygon2D(SKCanvas canvas, Polygon2D polygon, eDrawFigureType drawType)
        {
            Polygon2D_DrawFigure figure = polygon.GetDrawFigure(drawType) as Polygon2D_DrawFigure;

            if (polygon.IsFilled == true)
            {
                SKPaint paint = GetFillSKPaint(polygon);

                canvas.DrawPolygon(figure.Points, paint);
            }

            if (polygon.IsDrawLine == true)
            {
                SKPaint paint = GetLineSKPaint(polygon, drawType);

                canvas.DrawPolygon(figure.Points, paint);
            }

            if (polygon.IsSelect == true)
            {
                SKPoint[] points = CalBoundBox.GetBoundingBoxPolygonSK(figure.Points);
                canvas.DrawPolygon(points, m_SelectBoxPaint);
            }
        }

        /// <summary>
        /// 2D矢印を描画する関数。
        /// グリッド座標系で指定された始点・終点をクライアント座標に変換し、
        /// 矢印用のペンを用いて直線を描画する。
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト。</param>
        /// <param name="arrow">グリッド座標系で定義された Arrow2D オブジェクト。</param>
        /// <param name="drawType">描画タイプ(通常: Normal, マウスヒット時: Hit)</param>
        public void DrawArrow2D(SKCanvas canvas, Arrow2D arrow, eDrawFigureType drawType)
        {
            Arrow2D_DrawFigure figure = arrow.GetDrawFigure(drawType) as Arrow2D_DrawFigure;

            SKPaint paint = GetLineSKPaint(arrow, drawType);
            canvas.DrawLine(figure.Start, figure.End, paint);

            // 選択中の場合はハイライト描画
            if (arrow.IsSelect == true)
            {
                SKPoint[] points = CalBoundBox.GetBoundingBoxLineSK(figure.Start, figure.End, arrow.Width);
                canvas.DrawPolygon(points, m_SelectBoxPaint);
            }
        }
        /// <summary>
        /// 指定された Text2D オブジェクトを 2D 描画するメソッド。
        /// グリッド座標をクライアント座標に変換し、指定角度で回転させて文字列を描画する。
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト。</param>
        /// <param name="text">描画する文字列情報を保持する Text2D オブジェクト。</param>
        /// <param name="drawType">描画タイプ(通常: Normal, マウスヒット時: Hit)</param>
        public void DrawText2D(SKCanvas canvas, Text2D text, eDrawFigureType drawType)
        {
            Text2D_DrawFigure figure = text.GetDrawFigure(drawType) as Text2D_DrawFigure;

            SKPaint paint = GetTextSKPaint(text);

            SKFont font = DrawManager.GetSKFont(figure.FontName, figure.FontSize);

            try
            {
                if (Comp.IsEqual(figure.Angle, 0))
                {
                    // 回転が無い場合はそのまま描画
                    canvas.DrawText2(figure.Text, figure.ClientPoint, font, paint);
                }
                else
                {
                    // 回転の中心を決める
                    canvas.Save();
                    canvas.Translate(figure.ClientPoint.X, figure.ClientPoint.Y);

                    // SkiaSharpは時計回りが正なので、必要なら符号調整
                    canvas.RotateDegrees(-figure.Angle);

                    // 描画（座標は回転後の原点からの位置）
                    canvas.DrawText2(figure.Text, 0, 0, font, paint);

                    // 変換をリセット
                    canvas.Restore();
                }

                if (text.IsSelect)
                {
                    var points = CalBoundBox.GetBoundingBoxTextSK(
                        figure.ClientPoint.X, figure.ClientPoint.Y,
                        font, figure.Text, figure.Angle, eCalculateType.Client);

                    canvas.DrawPolygon(points, m_SelectBoxPaint);
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// Arc2D オブジェクトを描画するメソッド。
        /// グリッド座標をクライアント座標に変換し、
        /// 塗りつぶし指定がある場合は円弧を塗りつぶし＋アウトライン描画、
        /// 指定がない場合は輪郭線のみを描画する。
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト。</param>
        /// <param name="arc">
        /// 描画対象の Arc2D オブジェクト。  
        /// - X, Y : グリッド座標  
        /// - R : 半径（グリッド単位）  
        /// - StartAngle, EndAngle : 円弧の角度範囲  
        /// - IsFilled : 塗りつぶし有無  
        /// - その他、ペンやブラシ情報を保持  
        /// </param>
        /// <param name="drawType">描画タイプ(通常: Normal, マウスヒット時: Hit)</param>
        public void DrawArc2D(SKCanvas canvas, Arc2D arc, eDrawFigureType drawType)
        {
            Arc2D_DrawFigure figure = arc.GetDrawFigure(drawType) as Arc2D_DrawFigure;

            SKPoint ClientPointF = new SKPoint(figure.X, figure.Y);

            if (arc.IsFilled == true)
            {
                SKPaint paint = GetFillSKPaint(arc);

                canvas.DrawArc(paint, ClientPointF, figure.R, -figure.StartAngle, -figure.EndAngle);
            }

            if (arc.IsDrawLine == true)
            {
                SKPaint paint = GetLineSKPaint(arc, drawType);

                canvas.DrawArc(paint, ClientPointF, figure.R, -figure.StartAngle, -figure.EndAngle, figure.IsDrawSideLines);
            }

            if (arc.IsSelect == true)
            {
                SKPoint[] points = CalBoundBox.GetBoundingBoxCircleSK(figure.X, figure.Y, figure.R);
                canvas.DrawPolygon(points, m_SelectBoxPaint);
            }
        }

        /// <summary>
        /// グラフを描画する関数。
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト。</param>
        /// <param name="graph">描画対象グラフ図形オブジェクト</param>
        /// <param name="drawType">描画タイプ(通常: Normal, マウスヒット時: Hit)</param>
        public void DrawGraph2D(SKCanvas canvas, Graph2D graph, eDrawFigureType drawType)
        {
            Graph2D_DrawFigure figure = graph.GetDrawFigure(drawType) as Graph2D_DrawFigure;

            // 点が無い場合は描画しない
            if (figure == null)
            {
                return;
            }

            SKPaint paint = GetLineSKPaint(graph, drawType);

            canvas.DrawSmoothCurve(paint, figure.Points);

            if (graph.IsSelect == true)
            {
                SKPoint[] points = CalBoundBox.GetBoundingBoxPolygonSK(figure.Points);
                canvas.DrawPolygon(points, m_SelectBoxPaint);
            }
        }

        /// <summary>
        /// グループ図形を描画する関数。
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="group"></param>
        /// <param name="drawType">描画タイプ(通常: Normal, マウスヒット時: Hit)</param>
        public void DrawGroup2D(SKCanvas canvas, Group2D group, eDrawFigureType drawType)
        {
            group.ObjectList.Sort();

            foreach (var objectItem in group.ObjectList)
            {
                DrawObject2D(canvas, objectItem.Object, drawType);
            }

            if (group.IsSelect == true)
            {
                Group2D_DrawFigure figure = group.GetDrawFigure(drawType) as Group2D_DrawFigure;
                canvas.DrawPolygon(figure.BoundingBoxPoints, m_SelectBoxPaint);
            }
        }

        /// <summary>
        /// イメージ図形を描画する関数。
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas。</param>
        /// <param name="image">グリッド座標系で定義された Image2D オブジェクト。</param>
        /// <param name="drawType">描画タイプ(通常: Normal, マウスヒット時: MouseHit)</param>
        public void DrawImage2D(SKCanvas canvas, Image2D image, eDrawFigureType drawType)
        {
            Image2D_DrawFigure figure = image.GetDrawFigure(drawType) as Image2D_DrawFigure;

            canvas.DrawBitmap2(figure.Bitmap, figure.X, figure.Y, figure.Angle);

            if (image.IsSelect == true)
            {
                SKPoint[] ClientBoundingBox = CalBoundBox.GetBoundingBoxSK(figure.X, figure.Y, figure.Bitmap.Width, figure.Bitmap.Height, figure.Angle, eCalculateType.Client, eRotateType.Center);
                canvas.DrawPolygon(ClientBoundingBox, m_SelectBoxPaint);
            }
        }

        /// <summary>
        /// オブジェクトに合わせたSKPaintオブジェクトを自動取得
        /// </summary>
        /// <param name="object2D">オブジェクト</param>
        /// <param name="drawType">描画タイプ(通常: Normal, マウスヒット時: Hit)</param>
        /// <returns>線描画用SKPaintオブジェクト</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SKPaint GetLineSKPaint(Object2D object2D, eDrawFigureType drawType)
        {
            if (object2D == null)
            {
                throw new ArgumentNullException(nameof(object2D));
            }

            int lineWidth = 1;
            bool isAntiAlias = true;
            LineStyle lineStyle = LineStyle.Solid;
            Color lineColor = Color.White;
            float[] customLineStylePattern = default;
            float customDashPhase = 0;

            if (object2D is ILineProperty line)
            {
                lineWidth = line.Width;
                lineStyle = line.Style;
                lineColor = line.Color;
                customLineStylePattern = line.CustomLineStyle;
                customDashPhase = line.CustomDashPhase;
            }

            if (object2D is IFillProperty fill)
            {
                lineWidth = fill.LineWidth;
                lineStyle = fill.LineStyle;
                lineColor = fill.LineColor;
                customLineStylePattern = fill.LineCustomLineStyle;
                customDashPhase = fill.LineCustomDashPhase;
            }

            if (drawType == eDrawFigureType.Hit)
            {
                lineWidth += object2D.MouseHitLineOffset;
            }

            SKPaint sKPaint = DrawManager.GetLineSkPaint(lineColor, lineWidth, isAntiAlias, lineStyle, customLineStylePattern, customDashPhase);

            return sKPaint;
        }

        /// <summary>
        /// オブジェクトに合わせたSKPaintオブジェクトを自動取得
        /// </summary>
        /// <param name="object2D">オブジェクト</param>
        /// <returns>線描画用SKPaintオブジェクト</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SKPaint GetFillSKPaint(Object2D object2D)
        {
            if (object2D == null)
            {
                throw new ArgumentNullException(nameof(object2D));
            }

            Color color = Color.White;

            if (object2D is IFillProperty fill)
            {
                color = fill.FillColor;
            }

            if (object2D is ITextProperty text)
            {
                color = text.Color;
            }

            SKPaint paint = DrawManager.GetFillSkPaint(color);

            return paint;
        }

        /// <summary>
        /// オブジェクトに合わせたSKPaintオブジェクトを自動取得
        /// </summary>
        /// <param name="object2D">オブジェクト</param>
        /// <returns>線描画用SKPaintオブジェクト</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SKPaint GetTextSKPaint(Object2D object2D)
        {
            if (object2D == null)
            {
                throw new ArgumentNullException(nameof(object2D));
            }

            Color color = Color.White;

            if (object2D is ITextProperty text)
            {
                color = text.Color;
            }

            SKPaint paint = DrawManager.GetTextSkPaint(color);

            return paint;
        }

        /// <summary>
        /// 背景に画像を描画する関数。
        /// 背景画像を描画する。描画方法は BackgroundImageLayout プロパティに従う。
        /// GraphicControl2DのBackgroundImageを使用して、背景を描画する
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="width">画像幅</param>
        /// <param name="height">画像高さ</param>
        public void DrawBackgroundImage(SKCanvas canvas, int width, int height)
        {
            if (m_Parent.sKBackGroundBitmap == null)
            {
                return;
            }

            switch (m_Parent.BackgroundImageLayout)
            {
                case ImageLayout.Stretch:
                    canvas.DrawBitmap(m_Parent.sKBackGroundBitmap, new SKRect(0, 0, width, height));
                    break;

                case ImageLayout.Center:
                    {
                        float x = (width - m_Parent.sKBackGroundBitmap.Width) / 2f;
                        float y = (height - m_Parent.sKBackGroundBitmap.Height) / 2f;
                        canvas.DrawBitmap(m_Parent.sKBackGroundBitmap, x, y);
                    }
                    break;

                case ImageLayout.Zoom:
                    {
                        float scale = Math.Min(
                            (float)width / m_Parent.sKBackGroundBitmap.Width,
                            (float)height / m_Parent.sKBackGroundBitmap.Height);

                        float w = m_Parent.sKBackGroundBitmap.Width * scale;
                        float h = m_Parent.sKBackGroundBitmap.Height * scale;

                        float x = (width - w) / 2f;
                        float y = (height - h) / 2f;

                        canvas.DrawBitmap(m_Parent.sKBackGroundBitmap, new SKRect(x, y, x + w, y + h));
                    }
                    break;

                case ImageLayout.Tile:
                    {
                        var shader = SKShader.CreateBitmap(
                            m_Parent.sKBackGroundBitmap,
                            SKShaderTileMode.Repeat,
                            SKShaderTileMode.Repeat);

                        var paint = new SKPaint { Shader = shader };
                        canvas.DrawRect(new SKRect(0, 0, width, height), paint);

                        shader.Dispose();
                        paint.Dispose();
                    }
                    break;

                case ImageLayout.None:
                    // 左上にそのまま描画
                    canvas.DrawBitmap(m_Parent.sKBackGroundBitmap, 0, 0);
                    break;
            }
        }

        /// <summary>
        /// 背景のグリッド線を描画
        /// </summary>
        /// <param name="canvas"></param>
        public void DrawBackGroundGrid(SKCanvas canvas)
        {
            // グリッド線を描画
            DrawGrid(canvas);

            // 中央線を描画
            DrawCenterLine(canvas);
        }

        /// <summary>
        /// 背景のグリッド座標テキストを描画
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト。</param>
        public void DrawBackGroundGridText(SKCanvas canvas)
        {
            // グリッド座標テキストを描画
            DrawGridText(canvas);
            // 原点の0テキストを描画
            DrawCenterText(canvas);
        }

        /// <summary>
        /// マウス位置・スケーリング情報テキストを描画
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト。</param>
        public void DrawInfoText(SKCanvas canvas)
        {
            SKFont font = DrawManager.ConvertFontToSKFont(m_Parent.InfoTextFont);
            SKPaint paint = DrawManager.GetTextSkPaint(m_Parent.ForeColor);

            try
            {
                PointF gridMousePoint = GetGridMousePoint();

                string mousePosText = string.Format("Mouse Position : X={0:0.0000}, Y={1:0.0000}", gridMousePoint.X, gridMousePoint.Y);
                string scaleText = string.Format("Zoom : {0:0.00}%", Graphic2DControl.UserZoom * 100);

                string otherText = "";

                if (m_Parent.IsCaluculatingSusiki == true)
                {
                    otherText = CALUCULATING_TEXT;
                }

                PointF textPoint;

                // マウス位置テキストの描画
                textPoint = GetDrawInfoTextPosition(mousePosText, 1);
                canvas.DrawText(mousePosText, textPoint.X, textPoint.Y, font, paint);

                // スケーリングテキストの描画
                textPoint = GetDrawInfoTextPosition(scaleText, 2);
                canvas.DrawText(scaleText, textPoint.X, textPoint.Y, font, paint);

                // その他テキストの描画
                textPoint = GetDrawInfoTextPosition(otherText, 3);
                canvas.DrawText(otherText, textPoint.X, textPoint.Y, font, paint);
            }
            finally
            {
                font.Dispose();
                paint.Dispose();
            }
        }

        /// <summary>
        /// 情報テキストの表示位置データを更新
        /// </summary>
        /// <param name="Text">表示するテキスト文字列。</param>
        /// <param name="OutRecordNo">表示行番号（下から何行目か）。</param>
        public PointF GetDrawInfoTextPosition(string Text, int OutRecordNo)
        {
            if (string.IsNullOrEmpty(Text) == true)
            {
                return default;
            }

            // コントロールの右下に表示
            SizeF textSize = CalText.GetTextSize(Text, m_Parent.InfoTextFont, eCalculateType.Client);

            // マウス位置テキストの表示位置の更新
            PointF TextPt = new PointF();
            TextPt.X = m_Parent.ClientSize.Width - textSize.Width - 5;
            TextPt.Y = m_Parent.ClientSize.Height - OutRecordNo * m_Parent.InfoTextFont.Height;

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
            PointF GridMousePoint = CalConvert.ConvertClientPointToDisplayGridPoint(ClientMousePoint);

            return GridMousePoint;
        }

        /// <summary>
        /// 指定された Graphics オブジェクトに中央線（縦・横両方）を描画する
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト</param>
        private void DrawCenterLine(SKCanvas canvas)
        {
            SKPoint Start;
            SKPoint End;

            // X軸中心線の描画
            m_GridManager.GetXAxisCenterLine(out Start, out End);

            canvas.DrawLine(Start, End, m_GridManager.GridXAxisCenterPaint);

            // Y軸中心線の描画
            m_GridManager.GetYAxisCenterLine(out Start, out End);

            canvas.DrawLine(Start, End, m_GridManager.GridYAxisCenterPaint);
        }

        /// <summary>
        /// 指定された Graphics オブジェクトに原点のテキストを描画する
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト</param>
        private void DrawCenterText(SKCanvas canvas)
        {
            // 原点の0テキストの描画
            canvas.DrawText2(m_GridManager.CenterTextData, m_GridManager.GridSKFont, m_GridManager.GridFontPaint);
        }

        /// <summary>
        /// 指定された SKCanvas にグリッドを描画する
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas</param>
        private void DrawGrid(SKCanvas canvas)
        {
            canvas.DrawPath(m_GridManager.GridPath, m_GridManager.GridPaint);
        }


        /// <summary>
        /// 指定された Graphics オブジェクトにグリッド座標テキストを描画する
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト</param>
        private void DrawGridText(SKCanvas canvas)
        {
            canvas.DrawTexts(m_GridManager.GridTexts, m_GridManager.GridSKFont, m_GridManager.GridFontPaint);
        }

        /// <summary>
        /// 選択中図形のバウンディングボックスを描画するペンを取得
        /// </summary>
        /// <returns></returns>
        public SKPaint GetSelectBoxSKPaint()
        {
            _SelectBoxPaint.Color = m_Parent.SelectBoxColor.ToSKColor();

            return _SelectBoxPaint;
        }
    }
}
