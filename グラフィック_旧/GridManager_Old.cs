using graphicbox2d.オブジェクトマネージャー_旧;
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
    internal class GridManager_Old : IDisposable
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
        internal readonly GraphicDrawEngine_Old m_GraphicDrawEngine;

        /// <summary>
        /// フォントマネージャー
        /// </summary>
        internal readonly FontManager m_FontManager = new FontManager();

        /// <summary>
        /// グリッド中心線描画用ペン（X軸)
        /// </summary>
        public Pen GridXAxisCenterPen { get { return GetXAxisCenterPen(); } }
        private readonly Pen _GridXAxisCenterPen = new Pen(Color.White, 1);

        /// <summary>
        /// グリッド中心線描画用ペン（Y軸)
        /// </summary>
        public Pen GridYAxisCenterPen { get { return GetYAxisCenterPen(); } }
        private readonly Pen _GridYAxisCenterPen = new Pen(Color.White, 1);

        /// <summary>
        /// グリッド線描画用ペン
        /// <summary>
        public  Pen GridPen { get { return GetGridPen(); } }
        private readonly Pen _GridPen = new Pen(Color.DimGray, 1);

        /// <summary>
        /// グリッド用フォントブラシ
        /// </summary>
        public Brush GridFontBrush => DrawManager_Old.GetSolidBrush(m_Graphic2DControl.ForeColor);

        /// <summary>
        /// グリッド用フォント
        /// </summary>
        public Font GridFont => GetGridFont();

        /// <summary>
        /// グリッドパス
        /// </summary>
        public GraphicsPath GridPath { get { return GetGridPath(); } }
        private GraphicsPath _GridPath = new GraphicsPath();

        /// <summary>
        /// グリッド座標テキスト
        /// </summary>
        public List<TextData_Old> GridTexts { get { return GetGridTexts(); } }
        private List<TextData_Old> _GridTexts = new List<TextData_Old>();

        /// <summary>
        /// グリッドの原点テキスト
        /// </summary>
        public TextData_Old CenterTextData { get { return GetCenterText(); } }
        private TextData_Old _CenterTextData;

        /// <summary>
        /// 全ての破棄対象オブジェクトリストを取得する
        /// </summary>
        private List<IDisposable> m_AllDisposeObjects => new List<IDisposable>()
        {
            _GridXAxisCenterPen,    // グリッド中心線描画用ペン(X)
            _GridYAxisCenterPen,    // グリッド中心線描画用ペン(Y)
            GridPen,                // グリッド線描画用ペン
            _GridPath,              // グリッドパス
        };

        /// <summary>
        /// X軸の色
        /// </summary>
        public Color XAxisColor => m_Graphic2DControl.XAxisColor;

        /// <summary>
        /// Y軸の色
        /// </summary>
        public Color YAxisColor => m_Graphic2DControl.YAxisColor;

        /// <summary>
        /// グリッドデータのキャッシュ
        /// グリッドに関する Graphic2DControl のプロパティが変更されたかどうかを判定するために使用する
        /// </summary>
        private GridData _cashGridData;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GridManager_Old(Graphic2DControl graphic2DControl, GraphicDrawEngine_Old graphicDrawEngine)
        {
            m_GraphicDrawEngine = graphicDrawEngine;
            m_Graphic2DControl  = graphic2DControl;
        }

        /// <summary>
        /// 描画領域の中央に位置する X 軸（横方向の中心線）の始点と終点を取得する。
        /// </summary>
        /// <param name="Start">X軸中心線の始点座標（左端）。</param>
        /// <param name="End">X軸中心線の終点座標（右端）。</param>
        public void GetXAxisCenterLine(out Point Start, out Point End)
        {
            Start = new Point();
            End = new Point();

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
        public void GetYAxisCenterLine(out Point Start, out Point End)
        {
            Start = new Point();
            End = new Point();

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
        private GraphicsPath GetGridPath()
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
        /// <returns>グリッド目盛りに対応する TextData_Old のリスト。</returns>
        private List<TextData_Old> GetGridTexts()
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
        /// <returns>中心点に配置される TextData_Old。</returns>
        private TextData_Old GetCenterText()
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
        private Pen GetGridPen()
        {
            _GridPen.Color = m_Graphic2DControl.GridColor;
            return _GridPen;
        }


        /// <summary>
        /// X軸の中心線を描画するための Pen を取得する。
        /// ペンの色は Graphic2DControl の XAxisColor 設定に基づいて更新される。
        /// </summary>
        /// <returns>X軸中心線描画用の Pen。</returns>
        private Pen GetXAxisCenterPen()
        {
            _GridXAxisCenterPen.Color = m_Graphic2DControl.XAxisColor;
            return _GridXAxisCenterPen;
        }

        /// <summary>
        /// Y軸の中心線を描画するための Pen を取得する。
        /// ペンの色は Graphic2DControl の YAxisColor 設定に基づいて更新される。
        /// </summary>
        /// <returns>Y軸中心線描画用の Pen。</returns>
        private Pen GetYAxisCenterPen()
        {
            _GridYAxisCenterPen.Color = m_Graphic2DControl.YAxisColor;
            return _GridYAxisCenterPen;
        }

        /// <summary>
        /// グリッドデータを更新する
        /// </summary>
        private void UpdateGridData()
        {
            Graphics g = m_Graphic2DControl.CreateGraphics();

            try
            {
                // 初期化
                _GridPath.Reset();
                _GridTexts.Clear();

                Rectangle Bounds = m_Graphic2DControl.Bounds;

                Point Start = new Point();
                Point End = new Point();

                Point TextPoint = new Point();

                // 縦線
                Start.Y = 0;
                End.Y   = Bounds.Height;

                int YAxisLineNum = Bounds.Width / m_Graphic2DControl.DisplayGridWidth;

                int StartIndex = - m_Graphic2DControl.UserMoveCenterX / m_Graphic2DControl.DisplayGridWidth;

                // 中心線より右側のグリッド線を作成
                for (int i = StartIndex; i <= StartIndex + (YAxisLineNum / 2) + 1; i++)
                {
                    if (i == 0)
                    {
                        continue;
                    }

                    // グリッド線を作成
                    int X = i * m_Graphic2DControl.DisplayGridWidth + m_Graphic2DControl.DisplayCenterPoint.X;

                    Start.X = X;
                    End.X = X;

                    _GridPath.StartFigure();
                    _GridPath.AddLine(Start, End);

                    // グリッド座標テキスト情報の作成
                    Point TextOffsetPoint = GetGridTextOffset(g, (i).ToString());

                    TextPoint.X = X + TextOffsetPoint.X;
                    TextPoint.Y = m_Graphic2DControl.DisplayCenterPoint.Y + TextOffsetPoint.Y;

                    TextData_Old textData = new TextData_Old(i.ToString(), TextPoint);
                    _GridTexts.Add(textData);
                }

                StartIndex = (-m_Graphic2DControl.DisplayGridWidth - m_Graphic2DControl.UserMoveCenterX) / m_Graphic2DControl.DisplayGridWidth;

                // 中心線より右側のグリッド線を作成
                for (int i = StartIndex; Math.Abs(i) <= Math.Abs(StartIndex) + (YAxisLineNum / 2) + 1; i--)
                {
                    // グリッド線を作成
                    int X = i * m_Graphic2DControl.DisplayGridWidth + m_Graphic2DControl.DisplayCenterPoint.X;

                    Start.X = X;
                    End.X = X;

                    _GridPath.StartFigure();
                    _GridPath.AddLine(Start, End);

                    // グリッド座標テキスト情報の作成
                    Point TextOffsetPoint = GetGridTextOffset(g, (i).ToString());

                    TextPoint.X = X + TextOffsetPoint.X;
                    TextPoint.Y = m_Graphic2DControl.DisplayCenterPoint.Y + TextOffsetPoint.Y;

                    TextData_Old textData = new TextData_Old(i.ToString(), TextPoint);
                    _GridTexts.Add(textData);
                }

                // 横線
                Start.X = 0;
                End.X   = Bounds.Width;

                int XAxisLineNum = Bounds.Height / m_Graphic2DControl.DisplayGridWidth;

                StartIndex = -m_Graphic2DControl.UserMoveCenterY / m_Graphic2DControl.DisplayGridWidth;

                // 中心線より下側のグリッド線を作成
                for (int i = StartIndex; i <= StartIndex + (XAxisLineNum / 2) + 1; i++)
                {
                    if (i == 0)
                    {
                        continue;
                    }

                    // グリッド線を作成
                    int Y = i * m_Graphic2DControl.DisplayGridWidth + m_Graphic2DControl.DisplayCenterPoint.Y;

                    Start.Y = Y;
                    End.Y = Y;

                    _GridPath.StartFigure();
                    _GridPath.AddLine(Start, End);

                    // グリッド座標テキスト情報の作成
                    Point TextOffsetPoint = GetGridTextOffset(g, (-i).ToString());

                    TextPoint.X = m_Graphic2DControl.DisplayCenterPoint.X + TextOffsetPoint.X;
                    TextPoint.Y = Y + TextOffsetPoint.Y;

                    TextData_Old textData = new TextData_Old((-i).ToString(), TextPoint);
                    _GridTexts.Add(textData);
                }

                StartIndex = (-m_Graphic2DControl.DisplayGridWidth - m_Graphic2DControl.UserMoveCenterY) / m_Graphic2DControl.DisplayGridWidth;

                // 中心線より上側のグリッド線を作成
                for (int i = StartIndex; Math.Abs(i) <= Math.Abs(StartIndex) + (XAxisLineNum / 2) + 1; i--)
                {
                    // グリッド線を作成
                    int Y = i * m_Graphic2DControl.DisplayGridWidth + m_Graphic2DControl.DisplayCenterPoint.Y;

                    Start.Y = Y;
                    End.Y = Y;

                    _GridPath.StartFigure();
                    _GridPath.AddLine(Start, End);

                    // グリッド座標テキスト情報の作成
                    Point TextOffsetPoint = GetGridTextOffset(g, (-i).ToString());

                    TextPoint.X = m_Graphic2DControl.DisplayCenterPoint.X + TextOffsetPoint.X;
                    TextPoint.Y = Y + TextOffsetPoint.Y;

                    TextData_Old textData = new TextData_Old((-i).ToString(), TextPoint);
                    _GridTexts.Add(textData);
                }

                // 原点の0テキストの描画
                Point CenterTextPoint = new Point();

                Point CenterTextOffsetPoint = GetGridTextOffset(g, "0");

                CenterTextPoint.X = m_Graphic2DControl.DisplayCenterPoint.X + CenterTextOffsetPoint.X;
                CenterTextPoint.Y = m_Graphic2DControl.DisplayCenterPoint.Y + CenterTextOffsetPoint.Y;

                _CenterTextData = new TextData_Old("0", CenterTextPoint);

                // キャッシュデータの更新
                _cashGridData.DisplayCenterPoint = m_Graphic2DControl.DisplayCenterPoint;
                _cashGridData.Width = m_Graphic2DControl.Width;
                _cashGridData.Height = m_Graphic2DControl.Height;
                _cashGridData.GridWidth = m_Graphic2DControl.DisplayGridWidth;
                _cashGridData.Font?.Dispose();
                _cashGridData.Font = (Font)m_Graphic2DControl.Font.Clone();
                _cashGridData.TextPosition = m_Graphic2DControl.TextPosition;
            }
            finally
            {
                g.Dispose();
            }
        }

        /// <summary>
        /// グリッドの格子点からの文字列の相対出力位置を取得
        /// </summary>
        /// <param name="g"></param>
        /// <param name="text">文字列</param>
        /// <returns></returns>
        private Point GetGridTextOffset(Graphics g, string text)
        {
            RectangleF TextRect = RectangleF.Empty;

            Point GridTextOffsetPoint = new Point(0, 0);

            if (m_Graphic2DControl.TextPosition != eGridTextPosition.BottomRight)
            {
                // テキストの出力する大きさを取得する
                TextRect = GraphicCaluculate_Old.GetTextRectangle(text, m_Graphic2DControl.Font);
            }

            switch (m_Graphic2DControl.TextPosition)
            {
                case eGridTextPosition.TopLeft:
                    GridTextOffsetPoint.X = -m_Graphic2DControl.TextOffsetX - (int)TextRect.Width;
                    GridTextOffsetPoint.Y = -m_Graphic2DControl.TextOffsetY - (int)TextRect.Height;
                    break;
                case eGridTextPosition.TopRight:
                    GridTextOffsetPoint.X = m_Graphic2DControl.TextOffsetX;
                    GridTextOffsetPoint.Y = -m_Graphic2DControl.TextOffsetY - (int)TextRect.Height;
                    break;
                case eGridTextPosition.BottomLeft:
                    GridTextOffsetPoint.X = -m_Graphic2DControl.TextOffsetX - (int)TextRect.Width;
                    GridTextOffsetPoint.Y = m_Graphic2DControl.TextOffsetY;
                    break;
                case eGridTextPosition.BottomRight:
                    GridTextOffsetPoint.X = m_Graphic2DControl.TextOffsetX;
                    GridTextOffsetPoint.Y = m_Graphic2DControl.TextOffsetY;
                    break;
            }

            return GridTextOffsetPoint;
        }

        /// <summary>
        /// グリッド座標テキスト用フォントを取得する
        /// </summary>
        /// <returns></returns>
        private Font GetGridFont()
        {
            Font OriginalFont = m_Graphic2DControl.Font;
            Font font = m_FontManager.GetFont(OriginalFont.SystemFontName, OriginalFont.Size * Graphic2DControl.UserZoom, OriginalFont.Style);
            
            return font;
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
}
