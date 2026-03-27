using graphicbox2d.グラフィック計算;
using graphicbox2d.グローバル変数;
using Newtonsoft.Json;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace graphicbox2d
{
    /// <summary>
    /// グラフィック2Dコントロールクラス
    /// </summary>
    [ToolboxBitmap(typeof(Graphic2DControl))]
    [Description("グラフィックを描画するためのコントロール")]
    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    public partial class Graphic2DControl : UserControl
    {
        // ===============================================================================
        //
        //                       公開プロパティ
        // 
        // ===============================================================================

        /// <summary>
        /// 画面に描画するレイヤーオブジェクトリスト
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Layer2DCollection Layers { get; set; }

        /// <summary>
        /// 選択中の図形オブジェクト
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Object2D> SelectedObjects { get { return GetSelectedObjects(); } }

        /// <summary>
        /// ユーザー操作によって変更された画面拡大率
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static float UserZoom { get; set; } = 1.0f;

        /// <summary>
        /// ユーザー操作によって、中心座標を移動した量 X方向
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int UserMoveCenterX { get; set; } = 0;

        /// <summary>
        /// ユーザー操作によって、中心座標を移動した量 Y方向
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int UserMoveCenterY { get; set; } = 0;


        // ===============================================================================
        //
        //                       公開イベント
        // 
        // ===============================================================================

        // ===============================================================================
        // マウスイベント
        // ===============================================================================
        /// <summary>
        /// グラフィックコントロール2D独自マウスダウンイベント
        /// </summary>
        [Browsable(true)]
        [Description("Graphic2Dコントロール上でマウスが押されたときに発生します。")]
        [Category("マウス")]
        public event Graphic2DMouseEventHandler ExMouseDown;

        /// <summary>
        /// グラフィックコントロール2D独自マウスムーブイベント
        /// </summary>
        [Browsable(true)]
        [Description("Graphic2Dコントロール上でマウスが移動したときに発生します。")]
        [Category("マウス")]
        public event Graphic2DMouseEventHandler ExMouseMove;

        /// <summary>
        /// グラフィックコントロール2D独自マウスアップイベント
        /// </summary>
        [Browsable(true)]
        [Description("Graphic2Dコントロール上でマウスが離されたときに発生します。")]
        [Category("マウス")]
        public event Graphic2DMouseEventHandler ExMouseUp;

        /// <summary>
        /// グラフィックコントロール2D独自マウスクリックイベント
        /// </summary>
        [Browsable(true)]
        [Description("Graphic2Dコントロール上でマウスがクリックされたときに発生します。※マウスを押した位置とマウスを離した位置が一致しているときのみ発生します。")]
        [Category("マウス")]
        public event Graphic2DMouseEventHandler ExMouseClick;

        // ===============================================================================
        // オブジェクト操作イベント
        // ===============================================================================
        /// <summary>
        /// オブジェクトドラッグ操作イベント
        /// </summary>
        [Browsable(true)]
        [Description("Graphic2Dコントロール上でオブジェクトがドラッグ操作されている時に発生します。")]
        [Category("オブジェクト操作")]
        public event Graphic2DObjectDraggingEventHandler ObjectDragging;

        /// <summary>
        /// オブジェクト上でマウスダウンされたときに発生します。
        /// </summary>
        [Browsable(true)]
        [Description("オブジェクト上でマウスダウンされたときに発生します。")]
        [Category("オブジェクト操作")]
        public event Graphic2DObjectEventHandler MouseDownOnObject;

        /// <summary>
        /// オブジェクト上でマウスアップされたときに発生します。
        /// </summary>
        [Browsable(true)]
        [Description("オブジェクト上でマウスアップされたときに発生します。")]
        [Category("オブジェクト操作")]
        public event Graphic2DObjectEventHandler MouseUpOnObject;

        /// <summary>
        /// オブジェクト上にマウスエンターされたときに発生します。
        /// </summary>
        [Browsable(true)]
        [Description("オブジェクト上にマウスエンターされたときに発生します。")]
        [Category("オブジェクト操作")]
        public event Graphic2DObjectEventHandler MouseEnterOnObject;

        /// <summary>
        /// オブジェクト上からマウスリーブされたときに発生します。
        /// </summary>
        [Browsable(true)]
        [Description("オブジェクト上からマウスリーブされたときに発生します。")]
        [Category("オブジェクト操作")]
        public event Graphic2DObjectEventHandler MouseLeaveOnObject;

        /// <summary>
        /// Graphic2Dコントロール上でオブジェクトが選択されたときに発生します。  
        /// ユーザーが図形に対して明示的な操作を行ったことを示し、  
        /// 編集・強調・情報表示などの処理に利用されます。
        /// </summary>
        [Browsable(true)]
        [Description("Graphic2Dコントロール上でオブジェクトが選択された時に発生します。")]
        [Category("オブジェクト操作")]
        public event Graphic2DObjectEventHandler ObjectSelected;

        /// <summary>
        /// Graphic2Dコントロール上でオブジェクトの選択が解除されたときに発生します。  
        /// ユーザーが図形との関係を解いたことを示し、選択状態のリセットやUI更新に利用されます。
        /// </summary>
        [Browsable(true)]
        [Description("Graphic2Dコントロール上でオブジェクトが選択解除された時に発生します。")]
        [Category("オブジェクト操作")]
        public event Graphic2DObjectEventHandler ObjectUnSelected;

        /// <summary>
        /// Graphic2Dコントロール上でオブジェクトにマウスが乗ったときに発生します。  
        /// ユーザーの注目対象を検知し、ハイライトやツールチップ表示などの演出に利用されます。
        /// </summary>
        [Browsable(true)]
        [Description("Graphic2Dコントロール上でオブジェクト上にマウスがある時に発生します。")]
        [Category("オブジェクト操作")]
        public event Graphic2DObjectEventHandler ObjectHovered;

        /// <summary>
        /// Graphic2Dコントロール上でオブジェクトが削除された時に発生します
        /// </summary>
        [Browsable(true)]
        [Description("Graphic2Dコントロール上でオブジェクトが削除された時に発生します。")]
        [Category("オブジェクト操作")]
        public event Graphic2DObjectDleteEventHandler ObjectDeleted;

        // ===============================================================================
        // 数式計算イベント
        // ===============================================================================
        /// <summary>
        ///  Graphic2Dコントロールでグラフオブジェクトの数式計算処理が開始されたときに発生します。
        /// </summary>
        [Browsable(true)]
        [Description("Graphic2Dコントロール内で、数式計算が開始された時に発生します。")]
        [Category("グラフ")]
        public event Graphic2DObjectEventHandler SusikiCaluculateStart;

        /// <summary>
        ///  Graphic2Dコントロールでグラフオブジェクトの数式計算処理が終了されたときに発生します。
        /// </summary>
        [Browsable(true)]
        [Description("Graphic2Dコントロール内で、数式計算が終了した時に発生します。")]
        [Category("グラフ")]
        public event Graphic2DObjectEventHandler SusikiCaluculateEnd;

        // ===============================================================================
        //
        //                       公開メソッド
        // 
        // ===============================================================================

        // ==========================================
        //　 ＊コンストラクタ＊
        // ==========================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Graphic2DControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~Graphic2DControl()
        {

        }

        // ==========================================
        //　 ＊再描画＊
        // ==========================================

        /// <summary>
        /// 画面を再描画する
        /// </summary>
        public void Redraw()
        {
            this.skControl.Invalidate();
        }

        /// <summary>
        /// 画面を再描画する
        /// </summary>
        public new void Invalidate()
        {
            if (this.skControl == null)
            {
                return;
            }

            this.skControl.Invalidate();
        }

        // ==========================================
        //　 ＊データの保存・読込み＊
        // ==========================================

        /// <summary>
        /// 現在画面に表示している全ての2Dオブジェクトをファイルに保存する。
        /// </summary>
        /// <param name="filePath">保存先のファイルパス</param>
        public void SaveData(string filePath)
        {
            Document2D document = new Document2D();

            document.Version = GetVersion();

            document.ObjectIDCounter = Object2D.counter;

            foreach (Layer2D layer in this.Layers)
            {
                Layer2D_Document layerDocument;

                layer.OutLayerDocument(out layerDocument);

                document.Layers.Add(layerDocument);
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;

            string json = JsonConvert.SerializeObject(document, Formatting.Indented, settings);

            File.WriteAllText(filePath, json);

        }

        /// <summary>
        /// 指定ファイルから2Dオブジェクトを読み込み、現在のデータに追加する。
        /// 既存データをクリアしてから読み込むことも可能。
        /// </summary>
        /// <param name="filePath">読み込み対象のファイルパス</param>
        /// <param name="IsClearDisplayData">
        /// trueの場合、読み込み前に現在保持している全データをクリアする
        /// </param>
        public void LoadData(string filePath, bool IsClearDisplayData = false)
        {
            if (IsClearDisplayData == true)
            {
                // 画面を初期化
                ClearAllData();
            }

            // 新しいデータを読み込む
            if (IsJson(filePath) == true)
            {
                LoadNewData(filePath);
            }
            else
            {
                LoadOldData(filePath);
            }

            // 再描画
            Invalidate();
        }

        // ==========================================
        //　 ＊コントロールキャプチャの取得＊
        // ==========================================

        /// <summary>
        /// このグラフィックコントロールの描画内容をキャプチャし、
        /// 画像としてクリップボードにコピーする。
        /// </summary>
        public void GetDisplayCapchar()
        {
            // コントロールのサイズに合わせたBitmapを生成
            using (Bitmap bmp = new Bitmap(this.Width, this.Height))
            {
                // コントロールの内容をBitmapに描画
                this.DrawToBitmap(bmp, new Rectangle(0, 0, this.Width, this.Height));

                // クリップボードに画像をコピー
                Clipboard.SetImage(bmp);
            }
        }

        // ===============================================================================
        // 
        //                       非公開プロパティ
        // 
        // ===============================================================================

        // ===============================================================================
        // 定数
        // ===============================================================================
        /// <summary>
        /// マウス判定距離（この距離以内なら「ヒット扱い」）（グリッド座標で指定）
        /// </summary>
        internal const float MOUSE_HIT_RANGE = 0.1f;

        // ===============================================================================
        // プロパティ
        // ===============================================================================
        /// <summary>
        /// グラフィック描画エンジンオブジェクト
        /// </summary>
        private GraphicDrawEngine _DRAW_ENGINE;

        /// <summary>
        /// デザインモード用背景描画エンジンオブジェクト
        /// </summary>
        private GraphicDrawEngine_Old _DRAW_ENGINE_OLD;

        /// <summary>
        /// 画面描画に使用するグリッド幅
        /// </summary>
        internal int DisplayGridWidth
        {
            get
            {
                int width = (int)(GridWidth * UserZoom);

                if (width == 0)
                {
                    width = 1;
                }

                return width;
            }
        }

        /// <summary>
        /// オリジナルの画面中央座標 + ユーザー移動量（現在の画面中央座標）
        /// </summary>
        internal Point DisplayCenterPoint => GetDisplayCenterPoint();

        /// <summary>
        /// オリジナルの画面中央座標（コントロールの幅・高さの中心点）
        /// </summary>
        internal Point OriginalCenterPoint = new Point(0, 0);

        /// <summary>
        /// 全オブジェクトを取得する
        /// </summary>
        internal List<Object2D> AllObjects { get { return GetAllObjects(); } }

        /// <summary>
        /// マウスヒット中のオブジェクト（選択・操作対象）
        /// </summary>
        internal Object2D HitClientObject { get { return _HitClientObject; } set { SetWidthUpdateCuser(ref _HitClientObject, value); } }
        private Object2D _HitClientObject = null;

        /// <summary>
        /// オブジェクトドラッグ操作中フラグ
        /// </summary>
        internal bool IsDraggingObject { get { return _IsDraggingObject; } set { SetWidthUpdateCuser(ref _IsDraggingObject, value); } }
        private bool _IsDraggingObject = false;

        /// <summary>
        /// オブジェクトホバー中フラグ
        /// </summary>
        internal bool IsHoveringObject => HitClientObject != null && MouseDownButton == MouseButtons.None;

        /// <summary>
        /// マウスダウン中のボタン種類
        /// </summary>
        internal MouseButtons MouseDownButton { get { return _MouseDownButton; } set { SetWidthUpdateCuser(ref _MouseDownButton, value); } }
        private MouseButtons _MouseDownButton = MouseButtons.None;

        /// <summary>
        /// 数式計算中フラグ
        /// </summary>
        internal bool IsCaluculatingSusiki { get { return _IsCaluculatingSusiki; } set { SetWidthUpdateCuser(ref _IsCaluculatingSusiki, value); } }
        private bool _IsCaluculatingSusiki = false;

        /// <summary>
        /// グラフィック2Dコントロールのマウスイベント拡張データ
        /// </summary>
        private Graphic2DMouseEventExtensionData _MouseExtensionData = new Graphic2DMouseEventExtensionData();

        /// <summary>
        /// 通常時のマウスカーソル
        /// 
        /// </summary>
        internal Cursor DefaultCuror = MouseCursors.NoneModeCursor;

        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        internal static IntPtr hWnd;

        /// <summary>
        /// SkiaSharpコントロール
        /// </summary>
        internal SKControl skControl;

        /// <summary>
        /// バックグラウンドビットマップ（SkiaSharp描画用）
        /// </summary>
        internal SKBitmap sKBackGroundBitmap;


        // ===============================================================================
        //
        //                       非公開メソッド
        // 
        // ===============================================================================

        // ==========================================
        //　 ＊オーバーライドイベント＊
        // ==========================================

        /// <summary>
        /// ユーザーコントロール読み込みイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // 初期化
            Layers = new Layer2DCollection();

            HitClientObject = null;

            OriginalCenterPoint.X = (int)this.Width / 2;
            OriginalCenterPoint.Y = (int)this.Height / 2;

            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);

            // ウィンドウハンドルを取得
            hWnd = this.Handle;

            Global.Graphic2DControl = this;
            Global.GraphicDrawEngine = _DRAW_ENGINE;

            if (DesignMode == true)
            {
                _DRAW_ENGINE_OLD = new GraphicDrawEngine_Old(this);
            }
            else
            {
                _DRAW_ENGINE = new GraphicDrawEngine(this);

                // SkiaSharpコントロール初期化
                InitializaSKControl();
            }

        }

        /// <summary>
        /// ユーザーコントロール解放イベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (_DRAW_ENGINE != null)
            {
                _DRAW_ENGINE.Dispose();
            }

            if (_DRAW_ENGINE_OLD != null)
            {
                _DRAW_ENGINE_OLD.Dispose();
            }

            ClearAllData();
        }

        /// <summary>
        /// 描画イベント（デザイナーモード用）
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode == false)
            {
                return;
            }

            base.OnPaint(e);

            // マウス座標テキストを描画
            if (IsShowInfoText == true)
            {
                _DRAW_ENGINE_OLD.DrawInfoText(e.Graphics);
            }
        }

        /// <summary>
        /// 背景描画イベント（デザイナーモード用）
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (DesignMode == false)
            {
                return;
            }

            base.OnPaintBackground(e);

            // グリッド表示が有効な場合
            if (GridShow == true)
            {
                // 背景のグリッド線を書き込む
                _DRAW_ENGINE_OLD.DrawBackGroundGrid(e.Graphics);
            }

            // グリッド座標テキスト表示が有効な場合
            if (GridTextShow == true)
            {
                // グリッド座標テキストを書き込む
                _DRAW_ENGINE_OLD.DrawBackGroundGridText(e.Graphics);
            }
        }

        /// <summary>
        /// BackgroundImageチェンジイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            if (DesignMode == true)
            {
                return;
            }

            base.OnBackgroundImageChanged(e);

            this.sKBackGroundBitmap = this.BackgroundImage.ToSKBitmap();

            Invalidate();
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        /// <param name="IsInvalidate"></param>
        private void MoveObjects(List<Object2D> objects, float deltaX, float deltaY, bool IsInvalidate = true)
        {
            PointF MovePoint = new PointF(deltaX, deltaY);

            foreach (Object2D obj in objects)
            {
                obj.Move(deltaX, deltaY);
            }

            // 再描画
            if (IsInvalidate == true)
            {
                this.skControl.Invalidate();
            }
        }

        // ==========================================
        //　 ＊Graphic2D独自イベント＊
        // ==========================================

        /// <summary>
        /// マウス移動イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnExMouseMove(Graphic2DMouseEventArgs e)
        {
            switch (Mode)
            {
                case eGraphic2DControlMode.Default:
                    OnExMouseMove_DefaultMode(e);
                    break;
                case eGraphic2DControlMode.Select:
                    OnExMouseMove_SelectMode(e);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// マウスダウンイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnExMouseDown(Graphic2DMouseEventArgs e)
        {
            MouseDownButton = e.Button;

            // オブジェクトがヒットしている場合、オブジェクトドラッグ操作中フラグを立てる
            if (HitClientObject != null)
            {
                IsDraggingObject = true;
            }
        }

        /// <summary>
        /// マウスアップイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnExMouseUp(Graphic2DMouseEventArgs e)
        {
            MouseDownButton = MouseButtons.None;

            // オブジェクトドラッグ操作中フラグを下ろす
            IsDraggingObject = false;
        }

        /// <summary>
        /// マウスクリックイベント
        /// ※マウスを押した位置とマウスを離した位置が一致しているときのみ発生します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnExMouseClick(Graphic2DMouseEventArgs e)
        {
            switch (Mode)
            {
                case eGraphic2DControlMode.Default:
                    //何もしない
                    break;
                case eGraphic2DControlMode.Select:
                    OnMouseClick_SelectMode(e);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// オブジェクト削除イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnDeleteObject(Graphic2DObjectDeleteEventArgs e)
        {
        }

        /// <summary>
        /// オブジェクト上でマウスダウンイベント
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseDownOnObject(Graphic2DObjectEventArgs e)
        {
        }

        /// <summary>
        /// オブジェクト上でマウスアップイベント
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseUpOnObject(Graphic2DObjectEventArgs e)
        {
        }

        /// <summary>
        /// オブジェクト上にマウスエンターイベント
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseEnterOnObject(Graphic2DObjectEventArgs e)
        {
        }

        /// <summary>
        /// オブジェクト上からマウスリーブイベント
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseLeaveOnObject(Graphic2DObjectEventArgs e)
        {
        }

        /// <summary>
        /// オブジェクト選択イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnObjectSelected(Graphic2DObjectEventArgs e)
        {
        }

        /// <summary>
        /// オブジェクト選択解除イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnObjectUnSelected(Graphic2DObjectEventArgs e)
        {
        }

        /// <summary>
        /// オブジェクトホバーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnObjectHovered(Graphic2DObjectEventArgs e)
        {
        }

        /// <summary>
        /// オブジェクトドラッグ操作イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnObjectDragging(Graphic2DObjectDraggingEventArgs e)
        {
        }

        /// <summary>
        /// 数式開始イベント
        /// 数式開始メッセージがポストされた時に呼び出される。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSusikiCaluculateStart(Graphic2DObjectEventArgs e)
        {
            IsCaluculatingSusiki = true;
        }

        /// <summary>
        /// 数式開始終了イベント
        /// 数式終了メッセージがポストされた時に呼び出される。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSusikiCaluculateEnd(Graphic2DObjectEventArgs e)
        {
            IsCaluculatingSusiki = false;

            // 再描画
            this.skControl.Invalidate();
        }

        // ==========================================
        //　 ＊その他メソッド＊
        // ==========================================

        /// <summary>
        /// 全レイヤーの全オブジェクトを取得する
        /// </summary>
        /// <returns>全レイヤーの全オブジェクト</returns>
        private List<Object2D> GetAllObjects()
        {
            List<Object2D> allObjects = new List<Object2D>();

            foreach (Layer2D layer in Layers)
            {
                List<Object2D> objects = layer.GetAllObjects();

                allObjects.AddRange(objects);
            }

            return allObjects;
        }

        /// <summary>
        /// 選択中の図形オブジェクトをすべて取得する
        /// </summary>
        /// <returns>選択中の図形オブジェクトをまとめたList</returns>
        public List<Object2D> GetSelectedObjects()
        {
            List<Object2D> selected = AllObjects.Where(obje => obje.IsSelect == true).ToList();

            return selected;
        }

        /// <summary>
        /// 画面に表示する中心座標の位置を取得する
        /// </summary>
        /// <returns>画面に表示する中心座標</returns>
        private Point GetDisplayCenterPoint()
        {
            Point point = new Point();
            point.X = OriginalCenterPoint.X + UserMoveCenterX;
            point.Y = OriginalCenterPoint.Y + UserMoveCenterY;

            return point;
        }

        /// <summary>
        /// マウス移動イベント（デフォルトモード）
        /// </summary>
        /// <param name="e"></param>
        /// <param name="IsInvalidate">true:再描画を実行する false:再描画を実行しない</param>
        protected virtual void OnExMouseMove_DefaultMode(Graphic2DMouseEventArgs e, bool IsInvalidate = true)
        {
            Point MouseMovement = GetMouseMovement(e);

            // マウスにヒットしているオブジェクトを更新
            if (IsDraggingObject == false)
            {
                // オブジェクトドラッグ操作中でない場合のみ、マウスヒットオブジェクトを更新する
                UpdateHitClientObject(e.X, e.Y);
            }

            // マウスホイールが押されている場合、画面移動を実行
            if (IsDraggingObject == false && MouseDownButton == MouseButtons.Middle)
            {
                UserMoveCenterX += MouseMovement.X;
                UserMoveCenterY += MouseMovement.Y;
            }

            if (IsInvalidate == true)
            {
                // 再描画を実行
                this.skControl.Invalidate();
            }
        }

        /// <summary>
        /// マウス移動イベント(セレクトモード)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnExMouseMove_SelectMode(Graphic2DMouseEventArgs e)
        {
            Point MouseMovement = GetMouseMovement(e);

            // オブジェクトドラッグ操作中の場合
            if (IsDraggingObject == true)
            {
                // マウスカーソルに合わせてオブジェクトを移動する
                PointF GridMouseMovement = CalConvert.ConvertClientMouseMovementToGridMouseMovement(new Point(MouseMovement.X, MouseMovement.Y));

                if (HitClientObject != null)
                {
                    HitClientObject.Move(GridMouseMovement);
                }
            }

            // デフォルトモードのマウス移動処理を実行
            OnExMouseMove_DefaultMode(e, false);

            // 再描画を実行
            this.skControl.Invalidate();
        }

        /// <summary>
        /// 描画イベント（デフォルトモード）
        /// </summary>
        /// <param name="e"></param>
        private void OnPaint_DefaultMode(SKPaintSurfaceEventArgs e)
        {
            Layers.Sort();

            foreach (Layer2D layer in Layers)
            {
                // レイヤーが非表示の場合はスキップする
                if (layer.IsVisible == false)
                {
                    continue;
                }

                List<Object2D> allObjects = layer.GetAllObjects();

                // 全オブジェクトをZオーダーでソートする
                allObjects.Sort();

                foreach (Object2D object2D in allObjects)
                {
                    _DRAW_ENGINE.DrawObject2D(e.Surface.Canvas, object2D);
                }
            }
        }

        /// <summary>
        /// 描画イベント （セレクトモード）
        /// </summary>
        /// <param name="e"></param>
        private void OnPaint_SelectMode(SKPaintSurfaceEventArgs e)
        {
            Layers.Sort();

            foreach (Layer2D layer in Layers)
            {
                // レイヤーが非表示の場合はスキップする
                if (layer.IsVisible == false)
                {
                    continue;
                }

                List<Object2D> allObjects = layer.GetAllObjects();

                // 全オブジェクトをZオーダーでソートする
                allObjects.Sort();

                foreach (Object2D object2D in allObjects)
                {
                    // マウスにヒットしているオブジェクトは後で強調表示するため、ここでは描画しない
                    if (object2D == HitClientObject)
                    {
                        continue;
                    }

                    _DRAW_ENGINE.DrawObject2D(e.Surface.Canvas, object2D);
                }

                // マウスにヒットしているオブジェクトを強調表示する
                if (HitClientObject != null)
                {
                    // マウスにヒットしている状態の図形を取得
                    Object2D HitObject = HitClientObject.GetHitObject();

                    _DRAW_ENGINE.DrawObject2D(e.Surface.Canvas, HitObject);
                }
            }
        }

        /// <summary>
        /// マウスクリックイベント時の処理（セレクトモード）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseClick_SelectMode(Graphic2DMouseEventArgs e)
        {
            if (HitClientObject != null)
            {
                // 選択状態を逆転する
                HitClientObject.IsSelect = !HitClientObject.IsSelect;

                // 再描画
                Invalidate();
            }
        }

        /// <summary>
        /// マウス移動量を取得する
        /// 直前のマウス位置と現在のマウス位置の差分を取得する
        /// </summary>
        /// <param name="e"></param>
        /// <returns>マウス移動量</returns>
        private Point GetMouseMovement(Graphic2DMouseEventArgs e)
        {
            Point MouseMovement = new Point();
            MouseMovement.X = e.X - e.Last_Move_X;
            MouseMovement.Y = e.Y - e.Last_Move_Y;

            return MouseMovement;
        }

        /// <summary>
        /// 各図形リストから IsSelect が true の要素をすべて削除する。
        /// </summary>
        public void RemoveSelectedObjects()
        {
            foreach (Layer2D layer in Layers)
            {
                layer.RemoveSelectedObjects();
            }
            // 再描画
            this.skControl.Invalidate();
        }

        /// <summary>
        /// 値を変更と共にカーソルを更新する
        /// </summary>
        /// <param name="tartget">更新対象</param>
        /// <param name="value">更新値</param>
        private void SetWidthUpdateCuser<T>(ref T tartget, T value)
        {
            tartget = value;

            // マウスカーソル更新
            UpdateCursor();
        }

        /// <summary>
        /// マウスカーソル更新
        /// 現在の操作状況に応じて、マウスカーソルを変更する
        /// </summary>
        private void UpdateCursor()
        {
            // オブジェクトドラッグ中の場合、マウスカーソルを変更する
            if (IsDraggingObject == true)
            {
                this.Cursor = Cursors.Hand;
            }
            // 中心座標を移動中の場合、マウスカーソルを変更する
            else if (MouseDownButton == MouseButtons.Middle)
            {
                this.Cursor = Cursors.NoMove2D;
            }
            else
            {
                this.Cursor = DefaultCuror;
            }
        }

        /// <summary>
        /// マウスにヒットしているオブジェクトを更新する
        /// </summary>
        /// <param name="X">マウスX座標</param>
        /// <param name="Y">マウスY座標</param>
        private void UpdateHitClientObject(int X, int Y)
        {
            Point MousePosition = new Point(X, Y);

            PointF GridMousePoint = CalConvert.ConvertClientPointToGridPoint(MousePosition);

            Object2D Old_HitClientObject = HitClientObject;

            List<Object2D> HitCrossRangeObjects = new List<Object2D>();

            List<Object2D> HitOnObjects = new List<Object2D>();

            List<Object2D> allObjects = AllObjects;

            for (int i = 0; i < allObjects.Count; i++)
            {
                eMouseHitType hitType = allObjects[i].IsHitMousePoint(GridMousePoint, MOUSE_HIT_RANGE);

                if (hitType == eMouseHitType.CrossMouseRange)
                {
                    HitCrossRangeObjects.Add(allObjects[i]);
                }
                else if (hitType == eMouseHitType.MousePointOnObject)
                {
                    HitOnObjects.Add(allObjects[i]);
                }
            }

            int AllHitCount = HitOnObjects.Count + HitCrossRangeObjects.Count;

            // どのオブジェクトにもヒットしていない場合
            if (AllHitCount == 0)
            {
                HitClientObject = null;
            }
            // 1つだけのオブジェクトにヒットしている場合
            else if (AllHitCount == 1)
            {
                if (HitOnObjects.Count == 1)
                {
                    HitClientObject = (Object2D)HitOnObjects[0];
                }
                else
                {
                    HitClientObject = (Object2D)HitCrossRangeObjects[0];
                }
            }
            // 複数のオブジェクトにヒットしている場合
            else
            {
                if (HitOnObjects.Count == 0)
                {
                    // ======================
                    // マウスポイント(点)に重なっている図形は存在せず、マウスポイント範囲を表す半径と図形が交差している場合
                    // ======================

                    // 複数ヒットしている場合は、最も近いオブジェクトを選択する
                    HitClientObject = GraphicCaluculate.GetNearestMousePointObjest(HitCrossRangeObjects, GridMousePoint.X, GridMousePoint.Y);
                }
                else
                {
                    // ======================
                    // マウスポイント(点)に重なっている図形が存在する場合
                    // ======================

                    // 複数ヒットしている場合は、Zオーダーが一番大きいオブジェクトを選択する
                    HitOnObjects.Sort();

                    HitClientObject = HitOnObjects.Last();
                }
            }

            if (Old_HitClientObject != HitClientObject)
            {
                if (Old_HitClientObject != null)
                {
                    WinAPI.PostMessage(hWnd, WinMsg.WM_MOUSE_LEAVE_ON_OBJECT, (IntPtr)Old_HitClientObject._ID, IntPtr.Zero);
                }

                if (HitClientObject != null)
                {
                    WinAPI.PostMessage(hWnd, WinMsg.WM_MOUSE_ENTER_ON_OBJECT, (IntPtr)HitClientObject._ID, IntPtr.Zero);
                }

                this.skControl.Invalidate();
            }
        }

        /// <summary>
        /// オブジェクト削除実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void DeleteObject(Graphic2DObjectDeleteEventArgs e)
        {
            if (e.Cancel == true)
            {
                // 削除イベントがキャンセルされている場合は何もしない
                return;
            }

            switch (Mode)
            {
                case eGraphic2DControlMode.Default:
                    //何もしない
                    break;
                case eGraphic2DControlMode.Select:
                    //選択中のオブジェクトを削除する
                    RemoveSelectedObjects();
                    break;
                default:
                    break;
            }

            Invalidate();
        }

        /// <summary>
        /// 全ての2Dオブジェクトデータをクリアする。
        /// </summary>
        private void ClearAllData()
        {
            foreach (Layer2D layer2D in this.Layers)
            {
                layer2D.Points.Clear();
                layer2D.Lines.Clear();
                layer2D.Circles.Clear();
                layer2D.Polygons.Clear();
                layer2D.Arrows.Clear();
                layer2D.Texts.Clear();
                layer2D.Arcs.Clear();
                layer2D.Graphs.Clear();
                layer2D.MathGraphs.Clear();
                layer2D.Groups.Clear();
            }

            Layers.Clear();

            if (this.skControl == null)
            {
                return;
            }

            this.skControl.Invalidate();
        }

        /// <summary>
        /// XMLファイルからObject2DContainerをデシリアライズして読み込む。
        /// </summary>
        /// <param name="filePath">読み込み対象のファイルパス</param>
        /// <returns>復元されたObject2DContainer</returns>
        private Object2DContainer LoadFromXml(string filePath)
        {
            var serializer = new XmlSerializer(typeof(Object2DContainer));
            using (var reader = new StreamReader(filePath))
            {
                return (Object2DContainer)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// 新形式(JSON)のドキュメントファイルを読み込み、
        /// Document2D → Layer2D へデータを復元する。
        /// </summary>
        /// <param name="filePath">読み込む JSON ファイルのパス</param>
        /// <remarks>
        /// ・TypeNameHandling.Auto を使用して派生型を復元  
        /// ・Object2D.counter を保存データの値に同期  
        /// ・既存レイヤーがあれば上書き、なければ新規作成  
        /// ・ImportLayerDocument によりレイヤー内容を反映  
        /// </remarks>
        private void LoadNewData(string filePath)
        {
            // JSONファイルからデータを読み込む
            string json = File.ReadAllText(filePath);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;

            Document2D document = JsonConvert.DeserializeObject<Document2D>(json, settings);

            Object2D.counter = document.ObjectIDCounter;

            foreach (Layer2D_Document layerDoc in document.Layers)
            {
                Layer2D layer = Layers[layerDoc.LayerName];
                bool IsOnlyObjects = true;

                if (layer == null)
                {
                    layer = new Layer2D();
                    IsOnlyObjects = false;
                }

                layer.ImportLayerDocument(layerDoc, IsOnlyObjects);

                this.Layers.Add(layer);
            }
        }

        /// <summary>
        /// 指定されたファイルが JSON 形式として正しく解析できるか判定する。
        /// </summary>
        /// <param name="filePath">判定対象のファイルパス</param>
        /// <returns>
        /// JSON オブジェクトまたは配列としてパースできれば true、
        /// それ以外は false。
        /// </returns>
        /// <remarks>
        /// ・空ファイルは false  
        /// ・JToken.Parse により構文チェック  
        /// ・例外発生時は false を返す  
        /// </remarks>
        public static bool IsJson(string filePath)
        {
            // JSONファイルからデータを読み込む
            string json = File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            try
            {
                var token = Newtonsoft.Json.Linq.JToken.Parse(json);
                return token.Type == Newtonsoft.Json.Linq.JTokenType.Object ||
                       token.Type == Newtonsoft.Json.Linq.JTokenType.Array;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 旧形式(XML)のデータを読み込み、
        /// すべての図形を 1 つのレイヤーにまとめて追加する。
        /// </summary>
        /// <param name="filePath">読み込む XML ファイルのパス</param>
        /// <remarks>
        /// ・LoadFromXml により Object2DContainer を復元  
        /// ・読み込んだ図形群を新規レイヤーに集約  
        /// ・レイヤー名は "OldData-日時" 形式で自動生成  
        /// ・読み込み後に再描画を実行  
        /// </remarks>
        private void LoadOldData(string filePath)
        {
            // ファイルからデータを読み込む
            Object2DContainer container = LoadFromXml(filePath);

            Layer2D layer2D = new Layer2D();
            layer2D.LayerName = "OldData-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // 点図形
            layer2D.Points.AddRange(container.Points);
            // 線図形
            layer2D.Lines.AddRange(container.Lines);
            // 円図形
            layer2D.Circles.AddRange(container.Circles);
            // 多角形図形
            layer2D.Polygons.AddRange(container.Polygons);
            // 矢印線図形
            layer2D.Arrows.AddRange(container.Arrows);
            // テキスト図形
            layer2D.Texts.AddRange(container.Texts);
            // 円弧図形
            layer2D.Arcs.AddRange(container.Arcs);
            // グラフ図形
            layer2D.Graphs.AddRange(container.Graphs);
            // グループ図形
            layer2D.Groups.AddRange(container.Groups);

            this.Layers.Add(layer2D);

            // 再描画
            this.skControl.Invalidate();
        }

        /// <summary>
        /// バージョンを取得する
        /// </summary>
        /// <returns>バージョン</returns>
        private string GetVersion()
        {
            string version = Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            // "+" 以降を削除
            string pureVersion = version.Split('+')[0];

            // double に変換
            return pureVersion;
        }

        // ==========================================
        //　 ＊ウィンドウプロシージャオーバーライド＊
        // ==========================================

        /// <summary>
        /// ウィンドウプロシージャオーバーライド
        /// グラフィック2Dコントロールの独自イベントを発生させる
        /// また、既定の処理も呼び出す
        /// </summary>
        /// <param name="m">メッセージ</param>
        protected override void WndProc(ref Message m)
        {

            base.WndProc(ref m);

            switch (m.Msg)
            {
                // オブジェクト削除イベント
                case WinMsg.WM_OBJECT_DELETE:

                    List<Object2D> selectedObjects = SelectedObjects;

                    if (selectedObjects.Count == 0)
                    {
                        // 選択中のオブジェクトがない場合は何もしない
                        break;
                    }

                    // 選択中のオブジェクトがある場合は削除イベントを発生させる
                    Graphic2DObjectDeleteEventArgs DeleteArgs = MessageConverter.Convert(m, SelectedObjects);

                    // 既定の処理を呼び出す
                    OnDeleteObject(DeleteArgs);

                    ObjectDeleted?.Invoke(this, DeleteArgs);

                    // 削除を実行
                    DeleteObject(DeleteArgs);

                    break;

                // グラフオブジェクト数式計算開始
                case WinMsg.WM_SUSIKI_CALC_START:
                    int ID = (int)m.WParam;
                    Object2D CaclulatingObject = (Object2D)AllObjects.Where(obj => obj._ID == ID).FirstOrDefault();

                    Graphic2DObjectEventArgs eventArgs = MessageConverter.Convert(m, CaclulatingObject);

                    // 既定の処理を呼び出す
                    OnSusikiCaluculateStart(eventArgs);

                    SusikiCaluculateStart?.Invoke(this, eventArgs);

                    break;

                // グラフオブジェクト数式計算終了
                case WinMsg.WM_SUSIKI_CALC_END:
                    int End_ID = (int)m.WParam;
                    Object2D End_CaclulatingObject = (Object2D)AllObjects.Where(obj => obj._ID == End_ID).FirstOrDefault();

                    Graphic2DObjectEventArgs end_eventArgs = MessageConverter.Convert(m, End_CaclulatingObject);

                    // 既定の処理を呼び出す
                    OnSusikiCaluculateEnd(end_eventArgs);

                    SusikiCaluculateEnd?.Invoke(this, end_eventArgs);

                    break;

                // マウスオブジェクトエンターイベント
                case WinMsg.WM_MOUSE_ENTER_ON_OBJECT:
                    int Enter_ID = (int)m.WParam;
                    Object2D Enter_Object = (Object2D)AllObjects.Where(obj => obj._ID == Enter_ID).FirstOrDefault();

                    Graphic2DObjectEventArgs enter_eventArgs = MessageConverter.Convert(m, Enter_Object);

                    // 既定の処理を呼び出す
                    OnMouseEnterOnObject(enter_eventArgs);

                    MouseEnterOnObject?.Invoke(this, enter_eventArgs);
                    break;

                // マウスオブジェクトリーブイベント
                case WinMsg.WM_MOUSE_LEAVE_ON_OBJECT:
                    int Leave_ID = (int)m.WParam;
                    Object2D Leave_Object = (Object2D)AllObjects.Where(obj => obj._ID == Leave_ID).FirstOrDefault();

                    Graphic2DObjectEventArgs leave_eventArgs = MessageConverter.Convert(m, Leave_Object);

                    // 既定の処理を呼び出す
                    OnMouseLeaveOnObject(leave_eventArgs);

                    MouseLeaveOnObject?.Invoke(this, leave_eventArgs);
                    break;

                default:
                    break;
            }
        }

        // ====================================================================================
        //
        //　                            ＊SkiaShrp＊
        //
        // ====================================================================================

        /// <summary>
        /// SKControl の描画イベント。  
        /// 背景 → グリッド → モード別描画 → 情報テキスト の順で描画を行う。
        /// </summary>
        /// <param name="sender">イベント発生元の SKControl</param>
        /// <param name="e">描画情報を保持する SKPaintSurfaceEventArgs</param>
        /// <remarks>
        /// ・背景色でクリア  
        /// ・BackgroundImage が設定されていれば背景画像を描画  
        /// ・GridShow / GridTextShow に応じてグリッド線・座標テキストを描画  
        /// ・Mode に応じて個別の描画処理を実行  
        /// ・最後にマウス情報テキストを描画  
        /// </remarks>
        private void SkControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(BackColor.ToSKColor());

            /// 背景画像が設定されている場合、背景画像を描画する
            if (BackgroundImage != null)
            {
                _DRAW_ENGINE.DrawBackgroundImage(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            }

            // グリッド表示が有効な場合
            if (GridShow == true)
            {
                //// 背景のグリッド線を書き込む
                _DRAW_ENGINE.DrawBackGroundGrid(e.Surface.Canvas);
            }

            // グリッド座標テキスト表示が有効な場合
            if (GridTextShow == true)
            {
                // グリッド座標テキストを書き込む
                _DRAW_ENGINE.DrawBackGroundGridText(e.Surface.Canvas);
            }

            // モード別描画処理
            switch (Mode)
            {
                case eGraphic2DControlMode.Default:
                    OnPaint_DefaultMode(e);
                    break;
                case eGraphic2DControlMode.Select:
                    OnPaint_SelectMode(e);
                    break;
                default:
                    break;
            }

            // マウス座標テキストを描画
            if (IsShowInfoText == true)
            {
                _DRAW_ENGINE.DrawInfoText(e.Surface.Canvas);
            }
        }

        /// <summary>
        /// SKControl を生成し、描画・マウス・キー入力など  
        /// 必要なイベントハンドラをすべて登録する初期化処理。
        /// </summary>
        /// <remarks>
        /// ・Dock = Fill でコントロール全体に配置  
        /// ・TabStop = true によりキー入力を受け取れるようにする  
        /// ・PaintSurface / Mouse / Resize / Key 系イベントを登録  
        /// ・初期フォーカスを与えて起動直後からキー入力を受け付ける  
        /// </remarks>
        private void InitializaSKControl()
        {
            if (skControl == null)
            {
                skControl = new SKControl
                {
                    Dock = DockStyle.Fill,
                    TabStop = true // ← KeyDown / PreviewKeyDown 必須
                };

                // 描画
                skControl.PaintSurface += SkControl_PaintSurface;

                // マウス系
                skControl.MouseLeave += SkControl_MouseLeave;
                skControl.MouseDown += SkControl_MouseDown;
                skControl.MouseMove += SkControl_MouseMove;
                skControl.MouseUp += SkControl_MouseUp;
                skControl.MouseWheel += SkControl_MouseWheel;

                // サイズ変更
                skControl.Resize += SkControl_Resize;

                // キー入力系
                skControl.PreviewKeyDown += SkControl_PreviewKeyDown;
                skControl.KeyDown += SkControl_KeyDown;

                Controls.Add(skControl);
                skControl.Focus(); // ← 起動直後にキー入力を受けるため
            }

            this.sKBackGroundBitmap = this.BackgroundImage.ToSKBitmap();
        }

        /// <summary>
        /// マウスカーソルがコントロール外へ離れた際の処理。  
        /// ヒット中のオブジェクトをクリアし再描画する。
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベントデータ</param>
        /// <remarks>
        /// ・HitClientObject を null にして選択状態を解除  
        /// ・Invalidate により再描画を要求  
        /// </remarks>
        private void SkControl_MouseLeave(object sender, EventArgs e)
        {
            base.OnMouseLeave(e);

            // マウスヒットオブジェクトをクリア
            HitClientObject = null;

            this.skControl.Invalidate();
        }

        /// <summary>
        /// マウスダウン時の処理。  
        /// Graphic2DMouseEventArgs に変換し、  
        /// 通常のマウスダウン処理とオブジェクト上のマウスダウン処理を実行する。
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">MouseEventArgs</param>
        /// <remarks>
        /// ・MessageConverter により独自イベント引数へ変換  
        /// ・OnExMouseDown → ExMouseDown イベントを発生  
        /// ・HitClientObject がある場合はオブジェクト用イベントも発生  
        /// ・最後に拡張データへマウスダウン位置を記録  
        /// </remarks>
        private void SkControl_MouseDown(object sender, MouseEventArgs e)
        {
            Graphic2DMouseEventArgs args = MessageConverter.Convert(e, _MouseExtensionData);

            // 既定の処理を呼び出す
            OnExMouseDown(args);

            // マウスダウンイベントを発生させる
            ExMouseDown?.Invoke(this, args);

            if (HitClientObject != null)
            {
                // オブジェクト上でマウスダウンイベントを発生させる
                Graphic2DObjectEventArgs MouseDownOnObjectArgs = MessageConverter.Convert(e, HitClientObject);

                // 既定の処理を呼び出す
                OnMouseDownOnObject(MouseDownOnObjectArgs);

                MouseDownOnObject?.Invoke(this, MouseDownOnObjectArgs);
            }

            // 拡張データを更新
            _MouseExtensionData.Last_MouseDown_Point = new Point(args.X, args.Y);
        }

        /// <summary>
        /// マウス移動時の処理。  
        /// 通常の移動イベント、ドラッグ中のオブジェクト移動、
        /// ホバー中のオブジェクト通知などを行う。
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">MouseEventArgs</param>
        /// <remarks>
        /// ・MessageConverter により独自イベント引数へ変換  
        /// ・OnExMouseMove → ExMouseMove を発生  
        /// ・IsDraggingObject = true の場合はドラッグイベントを発生  
        /// ・ドラッグ中でなく、かつオブジェクト上にいる場合はホバーイベントを発生  
        /// ・最後に拡張データへ移動位置を記録  
        /// </remarks>
        private void SkControl_MouseMove(object sender, MouseEventArgs e)
        {
            Graphic2DMouseEventArgs moveArgs = MessageConverter.Convert(e, _MouseExtensionData);

            // 既定の処理を呼び出す
            OnExMouseMove(moveArgs);

            // マウス移動イベントを発生させる
            ExMouseMove?.Invoke(this, moveArgs);

            // オブジェクトをドラッグ操作中なら、ドラッグイベントを発生させる
            if (IsDraggingObject == true)
            {
                Graphic2DObjectDraggingEventArgs DragArgs = MessageConverter.Convert(e, _MouseExtensionData, HitClientObject);

                // 既定の処理を呼び出す
                OnObjectDragging(DragArgs);

                ObjectDragging?.Invoke(this, DragArgs);
            }
            // オブジェクトをドラッグ操作中出ない かつ　マウスがオブジェクト上にある場合
            else if (IsHoveringObject == true)
            {
                // オブジェクトホバーイベントを発生させる
                Graphic2DObjectEventArgs HoverArgs = MessageConverter.Convert(e, HitClientObject);

                // 既定の処理を呼び出す
                OnObjectHovered(HoverArgs);

                ObjectHovered?.Invoke(this, HoverArgs);
            }

            // 拡張データを更新
            _MouseExtensionData.Last_Move_Point = new Point(moveArgs.X, moveArgs.Y);
        }

        /// <summary>
        /// マウスボタンが離されたときの処理。  
        /// 通常のマウスアップ処理、オブジェクト上でのマウスアップ、
        /// クリック判定、オブジェクト選択／解除イベントを行う。
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">MouseEventArgs</param>
        /// <remarks>
        /// ・OnExMouseUp → ExMouseUp を発生  
        /// ・HitClientObject があればオブジェクト用 MouseUp イベントも発生  
        /// ・マウスダウン位置とアップ位置が一致すればクリック判定  
        /// ・クリック時、オブジェクトが選択状態なら ObjectSelected、
        ///   非選択なら ObjectUnSelected を発生  
        /// ・拡張データにマウスアップ位置を記録  
        /// </remarks>
        private void SkControl_MouseUp(object sender, MouseEventArgs e)
        {
            Graphic2DMouseEventArgs upArgs = MessageConverter.Convert(e, _MouseExtensionData);

            // 既定の処理を呼び出す
            OnExMouseUp(upArgs);

            // マウスアップイベントを発生させる
            ExMouseUp?.Invoke(this, upArgs);

            if (HitClientObject != null)
            {
                // オブジェクト上でマウスアップイベントを発生させる
                Graphic2DObjectEventArgs MouseUpOnObjectArgs = MessageConverter.Convert(e, HitClientObject);

                // 既定の処理を呼び出す
                OnMouseUpOnObject(MouseUpOnObjectArgs);

                MouseUpOnObject?.Invoke(this, MouseUpOnObjectArgs);
            }

            // 拡張データを更新
            _MouseExtensionData.Last_MouseUp_Point = new Point(upArgs.X, upArgs.Y);

            // マウスダウン位置とマウスアップ位置が同じならクリックイベントも発生させる
            if (_MouseExtensionData.Last_MouseDown_Point == _MouseExtensionData.Last_MouseUp_Point)
            {
                // 既定の処理を呼び出す
                OnExMouseClick(upArgs);

                // クリックイベントを発生させる
                ExMouseClick?.Invoke(this, upArgs);

                // クリックされた　かつ　マウスにオブジェクトがヒットしている場合
                if (HitClientObject != null)
                {
                    // オブジェクトが選択された場合、オブジェクト選択イベントを発生させる
                    if (HitClientObject.IsSelect == true)
                    {
                        Graphic2DObjectEventArgs SelectArgs = MessageConverter.Convert(e, HitClientObject);

                        // 既定の処理を呼び出す
                        OnObjectSelected(SelectArgs);

                        ObjectSelected?.Invoke(this, SelectArgs);
                    }
                    else
                    {
                        // オブジェクトが選択解除された場合、オブジェクト選択解除イベントを発生させる
                        Graphic2DObjectEventArgs UnSelectArgs = MessageConverter.Convert(e, HitClientObject);

                        // 既定の処理を呼び出す
                        OnObjectUnSelected(UnSelectArgs);

                        ObjectUnSelected?.Invoke(this, UnSelectArgs);
                    }
                }
            }
        }

        /// <summary>
        /// マウスホイール操作によるズーム処理。  
        /// ホイール方向に応じて 10% 拡大／縮小を行う。
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">MouseEventArgs</param>
        /// <remarks>
        /// ・e.Delta > 0 で拡大、< 0 で縮小  
        /// ・UserZoom を乗算してズーム値を更新  
        /// ・Invalidate により再描画を要求  
        /// </remarks>
        private void SkControl_MouseWheel(object sender, MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            float Zoom;

            if (e.Delta > 0)
            {
                // 10%拡大
                Zoom = 1.1f;
            }
            else
            {
                // 10%縮小
                Zoom = 0.9f;
            }

            // ユーザーズームを更新
            UserZoom *= Zoom;

            // 再描画
            this.skControl.Invalidate();
        }

        /// <summary>
        /// コントロールのサイズ変更時に呼ばれる処理。  
        /// 新しい中心座標（クライアント中央）を更新する。
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベントデータ</param>
        /// <remarks>
        /// ・Width / Height の中央を OriginalCenterPoint に設定  
        /// ・ズームや座標変換の基準点として利用される  
        /// </remarks>
        private void SkControl_Resize(object sender, EventArgs e)
        {
            base.OnResize(e);

            OriginalCenterPoint.X = (int)this.Width / 2;
            OriginalCenterPoint.Y = (int)this.Height / 2;
        }

        /// <summary>
        /// 特定のキー（矢印キー）を通常の入力キーとして扱うための前処理。  
        /// WinForms のデフォルト動作では矢印キーがフォーカス移動扱いになるため、
        /// それを無効化して KeyDown で受け取れるようにする。
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">PreviewKeyDownEventArgs</param>
        /// <remarks>
        /// ・Up / Down / Left / Right を IsInputKey = true に設定  
        /// ・これにより KeyDown イベントで矢印キーを処理可能になる  
        /// </remarks>
        private void SkControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            // 矢印キーを通常キーとして扱う
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.IsInputKey = true;
            }
        }

        /// <summary>
        /// キー入力時の処理。  
        /// 選択中オブジェクトの削除や、矢印キーによる微小移動を行う。
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">KeyEventArgs</param>
        /// <remarks>
        /// ・選択オブジェクトがない場合は何もしない  
        /// ・Delete キーで削除メッセージを送信  
        /// ・矢印キーで 1px 相当の移動（グリッド座標系に変換）  
        /// ・MoveObjects により複数オブジェクトの同時移動に対応  
        /// </remarks>
        private void SkControl_KeyDown(object sender, KeyEventArgs e)
        {
            List<Object2D> selectedObjects = SelectedObjects;

            if (selectedObjects.Count == 0)
            {
                // 選択中のオブジェクトがない場合は何もしない
                return;
            }

            // 1px移動量（グリッド座標系）
            float moveAmount = CalConvert.ConvertClientLengthToGridLength(1);

            switch (e.KeyCode)
            {
                case Keys.Delete:
                    WinAPI.PostMessage(hWnd, WinMsg.WM_OBJECT_DELETE, IntPtr.Zero, IntPtr.Zero);
                    break;
                case Keys.Up:
                    MoveObjects(selectedObjects, 0, moveAmount);
                    break;
                case Keys.Down:
                    MoveObjects(selectedObjects, 0, -moveAmount);
                    break;
                case Keys.Left:
                    MoveObjects(selectedObjects, -moveAmount, 0);
                    break;
                case Keys.Right:
                    MoveObjects(selectedObjects, moveAmount, 0);
                    break;
                default:
                    break;
            }
        }
    }
}
