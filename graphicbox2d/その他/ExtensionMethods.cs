using graphicbox2d.オブジェクトマネージャー;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{

    /// <summary>
    /// Vector2 型の拡張メソッドを提供します。
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Vector2 を PointF に変換します。
        /// </summary>
        /// <param name="v">変換対象の Vector2。</param>
        /// <returns>変換後の PointF。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ToPointF(this Vector2 v)
        {
            return new PointF(v.X, v.Y);
        }

        /// <summary>
        /// Vector2 を Point に変換します。
        /// </summary>
        /// <param name="v">変換対象の Vector2。</param>
        /// <returns>変換後の Point。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point ToPoint(this Vector2 v)
        {
            return new Point((int)v.X, (int)v.Y);
        }

        /// <summary>
        /// Vector2 を SKPoint に変換する
        /// </summary>
        /// <param name="v">変換元のベクトル</param>
        /// <returns>SKPoint</returns>
        internal static SKPoint ToSKPoint(this Vector2 v)
        {
            return new SKPoint(v.X, v.Y);
        }

    }

    /// <summary>
    /// PointF 型の拡張メソッドを提供します。
    /// </summary>
    public static class PointFExtensions
    {
        /// <summary>
        /// PointF を Vector2 に変換します。
        /// </summary>
        /// <param name="p">変換対象の PointF。</param>
        /// <returns>変換後の Vector2。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this PointF p)
        {
            return new Vector2(p.X, p.Y);
        }
    }

    /// <summary>
    /// Point 型の拡張メソッドを提供します。
    /// </summary>
    public static class PointExtensions
    {
        /// <summary>
        /// Point を Vector2 に変換します。
        /// </summary>
        /// <param name="p">変換対象の Point。</param>
        /// <returns>変換後の Vector2。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Point p)
        {
            return new Vector2(p.X, p.Y);
        }
    }

    /// <summary>
    /// Graphics クラス用の拡張メソッド
    /// </summary>
    internal static class GraphicsExtensions
    {
        /// <summary>
        /// 輪郭線描画用ペン
        /// </summary>
        private static readonly Pen m_FillOutLinePen = new Pen(Color.White);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static GraphicsExtensions()
        {
            m_FillOutLinePen.Width = 1;
            m_FillOutLinePen.DashStyle = DashStyle.Solid;
        }

        /// <summary>
        /// 中心座標と半径を指定して円の輪郭を描画する。
        /// </summary>
        /// <param name="g">描画先の Graphics オブジェクト。</param>
        /// <param name="pen">円の輪郭に使用する Pen。</param>
        /// <param name="cx">円の中心の X 座標。</param>
        /// <param name="cy">円の中心の Y 座標。</param>
        /// <param name="radius">円の半径。</param>
        public static void DrawCircle(this Graphics g, Pen pen, float cx, float cy, float radius)
        {
            g.DrawEllipse(pen, cx - radius, cy - radius, radius * 2, radius * 2);
        }

        /// <summary>
        /// 中心座標と半径を指定して円を塗りつぶし描画する。
        /// </summary>
        /// <param name="g">描画先の Graphics オブジェクト。</param>
        /// <param name="brush">塗りつぶしに使用する Brush。</param>
        /// <param name="cx">円の中心の X 座標。</param>
        /// <param name="cy">円の中心の Y 座標。</param>
        /// <param name="radius">円の半径。</param>
        public static void DrawFillCircle(this Graphics g, Brush brush, float cx, float cy, float radius)
        {
            g.FillEllipse(brush, cx - radius, cy - radius, radius * 2, radius * 2);
        }

        /// <summary>
        /// 塗りつぶし円を描画し、Brush が HatchBrush の場合は前景色で輪郭線を描画する。
        /// </summary>
        /// <param name="g">描画先の Graphics オブジェクト。</param>
        /// <param name="brush">塗りつぶしに使用する Brush。HatchBrush の場合に輪郭が描画される。</param>
        /// <param name="cx">円の中心の X 座標。</param>
        /// <param name="cy">円の中心の Y 座標。</param>
        /// <param name="radius">円の半径。</param>
        public static void DrawFillCircleWithOutLine(this Graphics g, Brush brush, float cx, float cy, float radius)
        {
            g.FillEllipse(brush, cx - radius, cy - radius, radius * 2, radius * 2);

            if (brush is HatchBrush)
            {
                m_FillOutLinePen.Color = (brush as HatchBrush).ForegroundColor;

                DrawCircle(g, m_FillOutLinePen, cx, cy, radius);
            }
        }

        /// <summary>
        /// 多角形を塗りつぶし、Brush が HatchBrush の場合は前景色で輪郭線を描画する。
        /// </summary>
        /// <param name="g">描画先の Graphics オブジェクト。</param>
        /// <param name="brush">塗りつぶしに使用する Brush。HatchBrush の場合に輪郭が描画される。</param>
        /// <param name="points">多角形を構成する頂点座標の配列。</param>
        public static void FillPolygonWithOutLine(this Graphics g, Brush brush, PointF[] points)
        {
            g.FillPolygon(brush, points);

            if (brush is HatchBrush)
            {
                m_FillOutLinePen.Color = (brush as HatchBrush).ForegroundColor;

                g.DrawPolygon(m_FillOutLinePen, points);
            }
        }

		/// <summary>
		/// 指定されたテキストデータを描画します。
		/// </summary>
		/// <param name="g">描画対象の Graphics オブジェクト。</param>
		/// <param name="textData">描画するテキストデータ。</param>
		/// <param name="font">テキスト描画に使用するフォント。</param>
		/// <param name="brush">テキスト描画に使用するブラシ。</param>
		/// <remarks>
		/// <see cref="TextData.Text"/> を <see cref="TextData_Old.TextPoint"/> の位置に描画します。
		/// </remarks>
		public static void DrawText(this Graphics g, TextData_Old textData, Font font, Brush brush)
        {
            g.DrawString(textData.Text, font, brush, textData.TextPoint);
        }

		/// <summary>
		/// 複数のテキストデータを順に描画します。
		/// </summary>
		/// <param name="g">描画対象の Graphics オブジェクト。</param>
		/// <param name="textData">描画するテキストデータのリスト。</param>
		/// <param name="font">テキスト描画に使用するフォント。</param>
		/// <param name="brush">テキスト描画に使用するブラシ。</param>
		/// <remarks>
		/// <see cref="DrawText(Graphics, TextData_Old, Font, Brush)"/> を内部で呼び出し、
		/// リスト内の各テキストを順に描画します。
		/// </remarks>
		public static void DrawTexts(this Graphics g, List<TextData_Old> textData, Font font, Brush brush)
        {
            for (int i = 0; i < textData.Count; i++)
            {
                DrawText(g, textData[i], font, brush);
            }
        }

        /// <summary>
        /// 指定されたパラメータで円弧を描画する関数。
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト。</param>
        /// <param name="pen">円弧を描画する Pen。</param>
        /// <param name="center">円の中心座標。</param>
        /// <param name="radius">円の半径。</param>
        /// <param name="startAngle">円弧の開始角度（度数法）。</param>
        /// <param name="endAngle">円弧の終了角度（度数法）。</param>
        /// <param name="IsDrawSideLine">円弧両サイドの線の描画有無</param>
        public static void DrawArc(this Graphics g, Pen pen, PointF center, float radius, float startAngle, float endAngle, bool IsDrawSideLine)
        {
            // 円弧を描画するための外接矩形を作成
            RectangleF rect = new RectangleF(
                center.X - radius,
                center.Y - radius,
                radius * 2,
                radius * 2);

            float sweepAngle = endAngle - startAngle;

            // 円弧を描画
            g.DrawArc(pen, rect, startAngle, sweepAngle);

            if (IsDrawSideLine == true)
            {
                PointF StartAnglePoint = GetArcSideLinePoint(center, radius, startAngle);

                PointF EndAnglePoint = GetArcSideLinePoint(center, radius, endAngle);

                g.DrawLine(pen, center, StartAnglePoint);

                g.DrawLine(pen, center, EndAnglePoint);
            }
        }

        /// <summary>
        /// 円弧を塗りつぶして描画し、必要に応じてアウトラインも描画する拡張メソッド。
        /// Brush が HatchBrush の場合は、その前景色を用いて円弧の輪郭線を描画する。
        /// </summary>
        /// <param name="g">描画対象の Graphics オブジェクト。</param>
        /// <param name="brush">塗りつぶしに使用する Brush。HatchBrush の場合はアウトラインも描画される。</param>
        /// <param name="center">円の中心座標。</param>
        /// <param name="radius">円の半径。</param>
        /// <param name="startAngle">円弧の開始角度（度数法）。</param>
        /// <param name="endAngle">円弧の終了角度（度数法）。</param>
        /// <param name="IsDrawSideLine">円弧両サイドの線の描画有無</param>
        public static void DrawFillArc(this Graphics g, Brush brush, PointF center, float radius, float startAngle, float endAngle, bool IsDrawSideLine)
        {
            // 円弧を描画するための外接矩形を作成
            Rectangle rect = new Rectangle(
                (int)(center.X - radius),
                (int)(center.Y - radius),
                (int)(radius * 2),
                (int)(radius * 2));

            // 円弧（扇形）を塗りつぶし
            float sweepAngle = endAngle - startAngle;

            g.FillPie(brush, rect, startAngle, sweepAngle);

            if (brush is HatchBrush)
            {
                m_FillOutLinePen.Color = (brush as HatchBrush).ForegroundColor;

                // 円弧の外周を描画
                DrawArc(g, m_FillOutLinePen, center, radius, startAngle, endAngle, IsDrawSideLine);
            }
        }

        /// <summary>
        /// 円弧の角度点（円弧の両端の点）を取得する
        /// </summary>
        /// <param name="center">円弧中点</param>
        /// <param name="radius">円弧半径</param>
        /// <param name="angle">角度</param>
        /// <returns>円弧の両端の点</returns>
        private static PointF GetArcSideLinePoint(PointF center, float radius, float angle)
        {
            float rad = GraphicCaluculate.DegreeToRadian(angle);
            PointF sidePoint = new PointF(
                center.X + radius * (float)Math.Cos(rad),
                center.Y + radius * (float)Math.Sin(rad)
                );

            return sidePoint;
        }
    }

    public static class ColorExtensions
    {
        // 色の反転
        public static Color Invert(this Color color)
        {
            return Color.FromArgb(
                color.A,
                255 - color.R,
                255 - color.G,
                255 - color.B
            );
        }

        // 明るさを調整（係数を掛ける）
        public static Color AdjustBrightness(this Color color, float factor)
        {
            int r = (int)(color.R * factor);
            int g = (int)(color.G * factor);
            int b = (int)(color.B * factor);

            // 0〜255にクリップ
            r = Math.Min(255, Math.Max(0, r));
            g = Math.Min(255, Math.Max(0, g));
            b = Math.Min(255, Math.Max(0, b));

            return Color.FromArgb(color.A, r, g, b);
        }

        /// <summary>
        /// System.Drawing.Color を SkiaSharp.SKColor に変換する拡張メソッド
        /// </summary>
        /// <param name="color">変換元の System.Drawing.Color</param>
        /// <returns>変換後の SKColor</returns>
        public static SKColor ToSKColor(this Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }
    }

    internal static class SKPointExtensions
    {
        /// <summary>
        /// SKPoint を Vector2 に変換する
        /// </summary>
        /// <param name="p">変換元の SKPoint</param>
        /// <returns>Vector2</returns>
        internal static Vector2 ToVector2(this SKPoint p)
        {
            return new Vector2(p.X, p.Y);
        }
    }

    internal static class SKCanvasExtensions
    {
        /// <summary>
        /// ポリゴンを描画
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="paint"></param>
        /// <param name="points"></param>
        internal static void DrawPolygon(this SKCanvas canvas, SKPoint[] points, SKPaint paint)
        {
            if (points == null || points.Length < 2)
            {
                return;
            }

            var path = new SKPath();

            try
            {
                path.MoveTo(points[0]);

                for (int i = 1; i < points.Length; i++)
                {
                    path.LineTo(points[i]);
                }

                path.Close();

                canvas.DrawPath(path, paint);
            }
            finally
            {
                path.Dispose();
            }

        }

        /// <summary>
        /// 指定されたパラメータで円弧を描画する関数（SkiaSharp版）。
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas。</param>
        /// <param name="paint">円弧を描画する SKPaint。</param>
        /// <param name="center">円の中心座標。</param>
        /// <param name="radius">円の半径。</param>
        /// <param name="startAngle">円弧の開始角度（度数法）。</param>
        /// <param name="endAngle">円弧の終了角度（度数法）。</param>
        /// <param name="isDrawSideLine">円弧両サイドの線の描画有無。</param>
        public static void DrawArc(this SKCanvas canvas, SKPaint paint, SKPoint center, float radius, float startAngle, float endAngle, bool isDrawSideLine = false)
        {
            // 円弧を描画するための外接矩形を作成
            SKRect rect = new SKRect(
                center.X - radius,
                center.Y - radius,
                center.X + radius,
                center.Y + radius);

            float sweepAngle = endAngle - startAngle;

            // 円弧を描画
            canvas.DrawArc(rect, startAngle, sweepAngle, false, paint);

            if (isDrawSideLine == true && paint.IsStroke == true)
            {
                SKPoint startAnglePoint = GetArcSideLinePoint(center, radius, startAngle);
                SKPoint endAnglePoint   = GetArcSideLinePoint(center, radius, endAngle);

                canvas.DrawLine(center, startAnglePoint, paint);
                canvas.DrawLine(center, endAnglePoint, paint);
            }
        }

        /// <summary>
        /// 与えられた座標点列を滑らかな曲線として描画する拡張メソッド。
        /// 各点の中間点を制御点に用いて二次ベジェ曲線を生成し、
        /// 始点から終点まで連続的に結んで描画する。
        /// </summary>
        /// <param name="canvas">描画対象の SKCanvas オブジェクト。</param>
        /// <param name="paint">描画に使用する SKPaint（色や線幅などを指定）。</param>
        /// <param name="points">曲線を構成する座標点列。2点以上が必要。</param>
        /// <remarks>
        /// - points が null または要素数が 2 未満の場合は描画を行わない。
        /// - 内部で SKPath を生成し、二次ベジェ曲線で各点を結んでいる。
        /// - 最後の点は LineTo により直線で結ばれる。
        /// - 使用後は SKPath を Dispose してリソースを解放する。
        /// </remarks>
        public static void DrawSmoothCurve(this SKCanvas canvas, SKPaint paint, SKPoint[] points)
        {
            if (canvas == null || paint == null || points == null || points.Length < 2)
                return;

            SKPath path = new SKPath();

            try
            {
                path.MoveTo(points[0]);

                for (int i = 1; i < points.Length - 1; i++)
                {
                    // 中間点を制御点にして二次ベジェで結ぶ
                    float midX = (points[i].X + points[i + 1].X) / 2;
                    float midY = (points[i].Y + points[i + 1].Y) / 2;
                    path.QuadTo(points[i].X, points[i].Y, midX, midY);
                }

                int lastIndex = points.Length - 1;

                // 最後の点まで結ぶ
                path.LineTo(points[lastIndex].X, points[lastIndex].Y);

                canvas.DrawPath(path, paint);
            }
            finally
            {
                path.Dispose();
            }

        }

        public static void DrawText2(this SKCanvas canvas, string text, float X, float Y, SKFont font, SKPaint paint, float angle = 0)
        {
            SKTextBlob textBlob = SKTextBlob.Create(text, font);

            var metrics = font.Metrics;

            float OffsetY = Y - metrics.Ascent;

            try
            {
                if (Comp.IsEqual(angle, 0))
                {
                    // 回転が無い場合はそのまま描画
                    canvas.DrawText(textBlob, X, OffsetY, paint);
                }
                else
                {
                    // 回転の中心を決める
                    canvas.Save();
                    canvas.Translate(X, OffsetY);

                    // SkiaSharpは時計回りが正なので、必要なら符号調整
                    canvas.RotateDegrees(-angle);

                    // 描画（座標は回転後の原点からの位置）
                    canvas.DrawText(textBlob, 0, 0, paint);

                    // 変換をリセット
                    canvas.Restore();
                }
            }
            finally
            {
                textBlob.Dispose();
            }
        }

        public static void DrawText2(this SKCanvas canvas, string text, SKPoint point, SKFont font, SKPaint paint, float angle = 0)
        {
            DrawText2(canvas, text, point.X, point.Y, font, paint, angle);
        }

        public static void DrawText2(this SKCanvas canvas, TextData textData, SKFont font, SKPaint paint, float angle = 0)
        {
            DrawText2(canvas, textData.Text, textData.TextPoint, font, paint, angle);
        }

        public static void DrawTexts(this SKCanvas canvas, List<TextData> textData, SKFont font, SKPaint paint, float angle = 0)
        {
            for (int i = 0; i < textData.Count; i++)
            {
                DrawText2(canvas, textData[i], font, paint, angle);
            }
        }

        /// <summary>
        /// 円弧の端点座標を計算する補助関数。
        /// </summary>
        private static SKPoint GetArcSideLinePoint(SKPoint center, float radius, float angle)
        {
            // 度数法をラジアンに変換
            float rad = angle * (float)Math.PI / 180f;
            return new SKPoint(
                center.X + radius * (float)Math.Cos(rad),
                center.Y + radius * (float)Math.Sin(rad));
        }
    }

    public static class FontStyleExtensions
    {
        /// <summary>
        /// System.Drawing.FontStyle を SKFontStyle に変換する拡張メソッド
        /// </summary>
        public static SKFontStyle ToSKFontStyle(this FontStyle style)
        {
            switch (style)
            {
                case FontStyle.Bold:
                    return SKFontStyle.Bold;
                case FontStyle.Italic:
                    return SKFontStyle.Italic;
                case FontStyle.Bold | FontStyle.Italic:
                    return SKFontStyle.BoldItalic;
                default:
                    return SKFontStyle.Normal;
            }
        }
    }
}
