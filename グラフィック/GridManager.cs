using graphicbox2d.オブジェクトマネージャー;
using graphicbox2d.グラフィック計算;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 背景グリッド線の描画マネージャークラス
    /// </summary>
    internal class GridManager : IDisposable
    {
        /// <summary>
        /// 破棄済フラグ
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Graphic2DControl オブジェクト
        /// </summary>
        internal readonly Graphic2DControl m_Graphic2DControl;

        /// <summary>
        /// 親の GraphicDrawEngine オブジェクト
        /// </summary>
        internal readonly GraphicDrawEngine m_GraphicDrawEngine;

        /// <summary>
        /// グリッド中心線描画用ペン（X軸)
        /// </summary>
        public SKPaint GridXAxisCenterPaint { get { return GetXAxisCenterPaint(); } }
        private readonly SKPaint _GridXAxisCenterPaint = MakeLineSKPaint(SKColors.White, 1, false);

        /// <summary>
        /// グリッド中心線描画用ペン（Y軸)
        /// </summary>
        public SKPaint GridYAxisCenterPaint { get { return GetYAxisCenterPaint(); } }
        private readonly SKPaint _GridYAxisCenterPaint = MakeLineSKPaint(SKColors.White, 1, false);

        /// <summary>
        /// グリッド線描画用ペン
        /// <summary>
        public SKPaint GridPaint { get { return GetGridPaint(); } }
        private readonly SKPaint _GridPaint = MakeLineSKPaint(SKColors.White, 1, false);

        /// <summary>
        /// グリッド用フォントブラシ
        /// </summary>
        public SKPaint GridFontPaint => DrawManager.GetTextSkPaint(m_Graphic2DControl.ForeColor);

        /// <summary>
        /// グリッド用フォント
        /// </summary>
        public SKFont GridSKFont => GetGridSKFont();

        /// <summary>
        /// グリッドパス
        /// </summary>
        public SKPath GridPath { get { return GetGridPath(); } }
        private SKPath _GridPath = new SKPath();

        /// <summary>
        /// グリッド座標テキスト
        /// </summary>
        public List<TextData> GridTexts { get { return GetGridTexts(); } }
        private List<TextData> _GridTexts = new List<TextData>();

        /// <summary>
        /// グリッドの原点テキスト
        /// </summary>
        public TextData CenterTextData { get { return GetCenterText(); } }
        private TextData _CenterTextData;

        /// <summary>
        /// 全ての破棄対象オブジェクトリストを取得する
        /// </summary>
        private List<IDisposable> m_AllDisposeObjects => new List<IDisposable>()
        {
            _GridXAxisCenterPaint,    // グリッド中心線描画用ペン(X)
            _GridYAxisCenterPaint,    // グリッド中心線描画用ペン(Y)
            GridPaint,                // グリッド線描画用ペン
            _GridPath,              // グリッドパス
        };

        /// <summary>
        /// グリッドデータのキャッシュ
        /// グリッドに関する Graphic2DControl のプロパティが変更されたかどうかを判定するために使用する
        /// </summary>
        private GridData _cashGridData;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GridManager(Graphic2DControl graphic2DControl, GraphicDrawEngine graphicDrawEngine = null)
        {
            m_GraphicDrawEngine = graphicDrawEngine;
            m_Graphic2DControl = graphic2DControl;

            // グリッド線描画用ペンの初期化
            GridPaint.Style = SKPaintStyle.Stroke;
            GridPaint.Color = SKColors.Gray;
            GridPaint.StrokeWidth = 1f;
            GridPaint.IsAntialias = false;
        }

        /// <summary>
        /// 描画領域の中央に位置する X 軸（横方向の中心線）の始点と終点を取得する。
        /// </summary>
        /// <param name="Start">X軸中心線の始点座標（左端）。</param>
        /// <param name="End">X軸中心線の終点座標（右端）。</param>
        public void GetXAxisCenterLine(out SKPoint Start, out SKPoint End)
        {
            Start = new SKPoint();
            End = new SKPoint();

            // 描画領域の幅
            int width = m_Graphic2DControl.Bounds.Width;
            // 描画領域の高さ
            int height = m_Graphic2DControl.Bounds.Height;
            // 横線
            int Y = (int)m_Graphic2DControl.DisplayCenterPoint.Y;

            Start.X = 0;
            Start.Y = Y;
            End.X = width;
            End.Y = Y;
        }

        /// <summary>
        /// 描画領域の中央に位置する Y 軸（縦方向の中心線）の始点と終点を取得する。
        /// </summary>
        /// <param name="Start">Y軸中心線の始点座標（上端）。</param>
        /// <param name="End">Y軸中心線の終点座標（下端）。</param>
        public void GetYAxisCenterLine(out SKPoint Start, out SKPoint End)
        {
            Start = new SKPoint();
            End = new SKPoint();

            // 描画領域の幅
            int width = m_Graphic2DControl.Bounds.Width;
            // 描画領域の高さ
            int height = m_Graphic2DControl.Bounds.Height;
            // 縦線
            int X = (int)m_Graphic2DControl.DisplayCenterPoint.X;
            Start.X = X;
            Start.Y = 0;
            End.X = X;
            End.Y = height;
        }

        /// <summary>
        /// グリッド線を描画するための GraphicsPath を取得する。
        /// グリッドデータが変更されている場合は更新処理を行った上で返す。
        /// </summary>
        /// <returns>背景グリッドを表す GraphicsPath。</returns>
        private SKPath GetGridPath()
        {
            if (IsChangedGraphic2DControlGridData() == true)
            {
                UpdateGridData();
            }

            return _GridPath;
        }

        /// <summary>
        /// グリッド上に表示するテキスト群を取得する。
        /// グリッドデータが変更されている場合は更新処理を行った上で返す。
        /// </summary>
        /// <returns>グリッド目盛りに対応する TextData のリスト。</returns>
        private List<TextData> GetGridTexts()
        {
            if (IsChangedGraphic2DControlGridData() == true)
            {
                UpdateGridData();
            }

            return _GridTexts;
        }

        /// <summary>
        /// グリッドの中心点に表示するテキストデータを取得する。
        /// グリッドデータが変更されている場合は更新処理を行った上で返す。
        /// </summary>
        /// <returns>中心点に配置される TextData。</returns>
        private TextData GetCenterText()
        {
            if (IsChangedGraphic2DControlGridData() == true)
            {
                UpdateGridData();
            }

            return _CenterTextData;
        }

        /// <summary>
        /// グリッド描画に使用するGraphic2DControlのデータが変更されたかどうかを判定する。
        /// </summary>
        /// <returns>true : 変更された　false:変更されてない</returns>
        private bool IsChangedGraphic2DControlGridData()
        {
            if (_cashGridData.DisplayCenterPoint != m_Graphic2DControl.DisplayCenterPoint)
            {
                return true;
            }

            if (_cashGridData.Width != m_Graphic2DControl.Width)
            {
                return true;
            }

            if (_cashGridData.Height != m_Graphic2DControl.Height)
            {
                return true;
            }

            if (_cashGridData.GridWidth != m_Graphic2DControl.DisplayGridWidth)
            {
                return true;
            }

            if (_cashGridData.Font == null)
            {
                return true;
            }

            if (_cashGridData.Font.Equals(m_Graphic2DControl.Font) == false)
            {
                return true;
            }

            if (_cashGridData.TextPosition != m_Graphic2DControl.TextPosition)
            {
                return true;
            }


            return false;
        }

        /// <summary>
        /// グリッド線の描画用ペンを取得する。
        /// </summary>
        /// <returns>Y軸中心線描画用の Pen。</returns>
        private SKPaint GetGridPaint()
        {
            _GridPaint.Color = m_Graphic2DControl.GridColor.ToSKColor();
            return _GridPaint;
        }


        /// <summary>
        /// X軸の中心線を描画するための Pen を取得する。
        /// ペンの色は Graphic2DControl の XAxisColor 設定に基づいて更新される。
        /// </summary>
        /// <returns>X軸中心線描画用の Pen。</returns>
        private SKPaint GetXAxisCenterPaint()
        {
            _GridXAxisCenterPaint.Color = m_Graphic2DControl.XAxisColor.ToSKColor();
            return _GridXAxisCenterPaint;
        }

        /// <summary>
        /// Y軸の中心線を描画するための Pen を取得する。
        /// ペンの色は Graphic2DControl の YAxisColor 設定に基づいて更新される。
        /// </summary>
        /// <returns>Y軸中心線描画用の Pen。</returns>
        private SKPaint GetYAxisCenterPaint()
        {
            _GridYAxisCenterPaint.Color = m_Graphic2DControl.YAxisColor.ToSKColor();
            return _GridYAxisCenterPaint;
        }

        /// <summary>
        /// グリッドデータを更新
        /// </summary>
        private void UpdateGridData()
        {
            // 初期化
            _GridPath.Reset();
            _GridTexts.Clear();

            var bounds = new SKRect(0, 0, m_Graphic2DControl.Width, m_Graphic2DControl.Height);

            SKPoint start = new SKPoint();
            SKPoint end = new SKPoint();
            SKPoint textPoint = new SKPoint();

            ///////////////////////////////////////////////////////////////////////
            // 縦線
            ///////////////////////////////////////////////////////////////////////
            start.Y = 0;
            end.Y = bounds.Height;

            // 描画領域に表示されるグリッド線の本数を計算
            int yAxisLineNum = (int)(bounds.Width / m_Graphic2DControl.DisplayGridWidth);

            // 現在、最も中央に近いグリッド線のインデックスを計算
            int startIndex = -m_Graphic2DControl.UserMoveCenterX / m_Graphic2DControl.DisplayGridWidth;

            // 中心線より右側
            for (int i = startIndex; i <= startIndex + (yAxisLineNum / 2) + 1; i++)
            {
                if (i == 0)
                    continue;

                int x = i * m_Graphic2DControl.DisplayGridWidth + m_Graphic2DControl.DisplayCenterPoint.X;

                start.X = x;
                end.X = x;

                _GridPath.MoveTo(start);
                _GridPath.LineTo(end);

                // テキスト位置
                var offset = GetGridTextOffset(i.ToString());
                textPoint.X = x + offset.X;
                textPoint.Y = m_Graphic2DControl.DisplayCenterPoint.Y + offset.Y;

                _GridTexts.Add(new TextData(i.ToString(), textPoint));
            }

            // 中心線より左側
            for (int i = startIndex - 1; startIndex - (yAxisLineNum / 2) - 1 <= i; i--)
            {
                int x = i * m_Graphic2DControl.DisplayGridWidth + m_Graphic2DControl.DisplayCenterPoint.X;

                start.X = x;
                end.X = x;

                _GridPath.MoveTo(start);
                _GridPath.LineTo(end);

                var offset = GetGridTextOffset(i.ToString());
                textPoint.X = x + offset.X;
                textPoint.Y = m_Graphic2DControl.DisplayCenterPoint.Y + offset.Y;

                _GridTexts.Add(new TextData(i.ToString(), textPoint));
            }

            ///////////////////////////////////////////////////////////////////////
            // 横線
            ///////////////////////////////////////////////////////////////////////
            start.X = 0;
            end.X = bounds.Width;

            // 描画領域に表示されるグリッド線の本数を計算
            int xAxisLineNum = (int)(bounds.Height / m_Graphic2DControl.DisplayGridWidth);

            // 現在、最も中央に近いグリッド線のインデックスを計算
            startIndex = -m_Graphic2DControl.UserMoveCenterY / m_Graphic2DControl.DisplayGridWidth;

            // 下側
            for (int i = startIndex; i <= startIndex + (xAxisLineNum / 2) + 1; i++)
            {
                if (i == 0)
                    continue;

                int y = i * m_Graphic2DControl.DisplayGridWidth + m_Graphic2DControl.DisplayCenterPoint.Y;

                start.Y = y;
                end.Y = y;

                _GridPath.MoveTo(start);
                _GridPath.LineTo(end);

                var offset = GetGridTextOffset((-i).ToString());
                textPoint.X = m_Graphic2DControl.DisplayCenterPoint.X + offset.X;
                textPoint.Y = y + offset.Y;

                _GridTexts.Add(new TextData((-i).ToString(), textPoint));
            }

            // 上側
            for (int i = startIndex - 1; startIndex - (xAxisLineNum / 2) - 1 <= i; i--)
            {
                int y = i * m_Graphic2DControl.DisplayGridWidth + m_Graphic2DControl.DisplayCenterPoint.Y;

                start.Y = y;
                end.Y = y;

                _GridPath.MoveTo(start);
                _GridPath.LineTo(end);

                var offset = GetGridTextOffset((-i).ToString());
                textPoint.X = m_Graphic2DControl.DisplayCenterPoint.X + offset.X;
                textPoint.Y = y + offset.Y;

                _GridTexts.Add(new TextData((-i).ToString(), textPoint));
            }

            // 原点の0
            var centerOffset = GetGridTextOffset("0");
            SKPoint centerTextPoint = new SKPoint(
                m_Graphic2DControl.DisplayCenterPoint.X + centerOffset.X,
                m_Graphic2DControl.DisplayCenterPoint.Y + centerOffset.Y
            );

            _CenterTextData = new TextData("0", centerTextPoint);

            // キャッシュ更新
            _cashGridData.DisplayCenterPoint = m_Graphic2DControl.DisplayCenterPoint;
            _cashGridData.Width = m_Graphic2DControl.Width;
            _cashGridData.Height = m_Graphic2DControl.Height;
            _cashGridData.GridWidth = m_Graphic2DControl.DisplayGridWidth;
            _cashGridData.TextPosition = m_Graphic2DControl.TextPosition;
            _cashGridData.Font = m_Graphic2DControl.Font;
        }

        /// <summary>
        /// グリッドの格子点からの文字列の相対出力位置を取得（SkiaSharp版）
        /// </summary>
        private SKPoint GetGridTextOffset(string text)
        {
            SKPoint offset = new SKPoint(0, 0);

            float DisplayTextWidth = 0;
            float DisplayTextHeight = 0;

            // BottomRight 以外は文字サイズを測る
            if (m_Graphic2DControl.TextPosition != eGridTextPosition.BottomRight)
            {
                SizeF size = CalText.GetTextSize(text, this.GridSKFont, eCalculateType.Client);
                // 横幅
                DisplayTextWidth = size.Width;

                DisplayTextHeight = size.Height;
            }

            float DisplayTextOffsetX = m_Graphic2DControl.TextOffsetX * Graphic2DControl.UserZoom;
            float DisplayTextOffsetY = m_Graphic2DControl.TextOffsetY * Graphic2DControl.UserZoom;

            switch (m_Graphic2DControl.TextPosition)
            {
                case eGridTextPosition.TopLeft:
                    offset.X = -DisplayTextOffsetX - DisplayTextWidth;
                    offset.Y = -DisplayTextOffsetY - DisplayTextHeight;
                    break;

                case eGridTextPosition.TopRight:
                    offset.X = DisplayTextOffsetX;
                    offset.Y = -DisplayTextOffsetY - DisplayTextHeight;
                    break;

                case eGridTextPosition.BottomLeft:
                    offset.X = -DisplayTextOffsetX - DisplayTextWidth;
                    offset.Y = DisplayTextOffsetY;
                    break;

                case eGridTextPosition.BottomRight:
                    offset.X = DisplayTextOffsetX;
                    offset.Y = DisplayTextOffsetY;
                    break;
            }

            return offset;
        }

        /// <summary>
        /// グリッド座標テキスト用フォントを取得する
        /// </summary>
        /// <returns></returns>
        private SKFont GetGridSKFont()
        {
            Font OriginalFont = m_Graphic2DControl.Font;

            SKFont sKFont = DrawManager.GetSKFont(OriginalFont.SystemFontName, OriginalFont.Size * 1.33f * Graphic2DControl.UserZoom, OriginalFont.Style.ToSKFontStyle());

            return sKFont;
        }

        /// <summary>
        /// 線用のSKPaint 作成
        /// </summary>
        /// <param name="sKColor">カラー</param>
        /// <param name="width">線幅</param>
        /// <returns></returns>
        private static SKPaint MakeLineSKPaint(SKColor sKColor, int width, bool isAntialias = true)
        {
            SKPaint skPaint = new SKPaint()
            {
                Color = sKColor,
                StrokeWidth = width,
                IsAntialias = isAntialias,
                Style = SKPaintStyle.Stroke,
            };

            return skPaint;
        }

        /// <summary>
        /// 破棄実装
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
    }

    /// <summary>
    /// グリッドデータ
    /// グリッド作成に必要なデータをまとめた構造体
    /// </summary>
    internal struct GridData
    {
        public Point DisplayCenterPoint;
        public int Width;
        public int Height;
        public int GridWidth;
        public Font Font;
        public eGridTextPosition TextPosition;
    }
}
