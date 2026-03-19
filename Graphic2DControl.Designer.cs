using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace graphicbox2d
{
    partial class Graphic2DControl
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Graphic2DControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ForeColor = System.Drawing.Color.White;
            this.DoubleBuffered = true;
            this.Name = "Graphic2DControl";
            this.Size = new System.Drawing.Size(500, 500);
            this.ResumeLayout(false);

        }

        #endregion

        #region 追加のコントロールプロパティ

        /// <summary>
        /// グリッドを表示する
        /// </summary>
        [Browsable(true)]
        [Description("グリッドを表示します。")]
        [Category("表示")]
        public bool GridShow { get { return _GridShow; } set { SetWithInvalidate(ref _GridShow, in value); } }
        private bool _GridShow = true;

        /// <summary>
        /// グリッド座標テキストを表示する
        /// </summary>
        [Browsable(true)]
        [Description("グリッド座標テキストを表示します。")]
        [Category("表示")]
        public bool GridTextShow { get { return _GridTextShow; } set { SetWithInvalidate(ref _GridTextShow, in value); } }
        private bool _GridTextShow = true;

        /// <summary>
        /// グリッドの幅
        /// </summary>
        [Browsable(true)]
        [Description("グリッドの幅を指定します。")]
        [Category("表示")]
        public int GridWidth { get { return _GridWidth; } set { SetWithInvalidate(ref _GridWidth, in value); } }
        private int _GridWidth = 50;

        /// <summary>
        /// グリッドの色
        /// </summary>
        [Browsable(true)]
        [Description("グリッドの色を指定します。")]
        [Category("表示")]
        public Color GridColor { get { return _GridColor; } set { SetWithInvalidate(ref _GridColor, in value); } }
        private Color _GridColor = Color.DimGray;

        /// <summary>
        /// X軸の色
        /// </summary>
        [Browsable(true)]
        [Description("X軸の色を指定します")]
        [Category("表示")]
        public Color XAxisColor { get { return _XAxisColor; } set { SetWithInvalidate(ref _XAxisColor, in value); } }
        private Color _XAxisColor = Color.LightGreen;

        /// <summary>
        /// Y軸の色
        /// </summary>
        [Browsable(true)]
        [Description("Y軸の色を指定します")]
        [Category("表示")]
        public Color YAxisColor{ get { return _YAxisColor; } set { SetWithInvalidate(ref _YAxisColor, in value); } }
        private Color _YAxisColor = Color.Yellow;

        /// <summary>
        /// テキストの位置
        /// </summary>
        [Browsable(true)]
        [Description("グリッド座標テキストの位置を指定します")]
        [Category("表示")]
        public eGridTextPosition TextPosition { get { return _TextPosition; } set { SetWithInvalidate(ref _TextPosition, in value); } }
        private eGridTextPosition _TextPosition = eGridTextPosition.BottomRight;

        /// <summary>
        /// グリッド座標テキストのオフセットX値
        /// </summary>
        [Browsable(true)]
        [Description("グリッド座標テキストのオフセットX値を指定します")]
        [Category("表示")]
        public int TextOffsetX { get { return _TextOffsetX; } set { SetWithInvalidate(ref _TextOffsetX, in value); } }
        private int _TextOffsetX = 10;

        /// <summary>
        /// グリッド座標テキストのオフセットY値
        /// </summary>
        [Browsable(true)]
        [Description("グリッド座標テキストのオフセットY値を指定します")]
        [Category("表示")]
        public int TextOffsetY { get { return _TextOffsetY; } set { SetWithInvalidate(ref _TextOffsetY, in value); } }
        private int _TextOffsetY = 5;

        /// <summary>
        /// モード変更
        /// </summary>
        [Browsable(true)]
        [Description("動作モードを指定します")]
        [Category("動作")]
        public eGraphic2DControlMode Mode { get { return _Mode; } set { SetMode(value); } }
        private eGraphic2DControlMode _Mode = eGraphic2DControlMode.Select;

        /// <summary>
        /// マウス座標などを表示する
        /// </summary>
        [Browsable(true)]
        [Description("マウス座標やズーム率等の情報をコントロールに表示します")]
        [Category("動作")]
        public bool IsShowInfoText { get { return _IsShowInfoText; } set { SetWithInvalidate(ref _IsShowInfoText, in value); } }
        private bool _IsShowInfoText = true;

        /// <summary>
        /// 情報テキストのフォントを指定します
        /// </summary>
        [Browsable(true)]
        [Description("情報テキストのフォントを指定します。")]
        [Category("動作")]
        public Font InfoTextFont { get { return _InfoTextFont; } set { SetWithInvalidate(ref _InfoTextFont, in value); } }
        private Font _InfoTextFont = new Font("Yu Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);

        /// <summary>
        /// セレクトボックスの色
        /// </summary>
        [Browsable(true)]
        [Description("セレクトボックスの色を指定します。")]
        [Category("動作")]
        public Color SelectBoxColor { get { return _SelectBoxColor; } set { SetWithInvalidate(ref _SelectBoxColor, in value); } }
        private Color _SelectBoxColor = Color.White;

        /// <summary>
        /// グリッド座標テキストの色
        /// </summary>
        public override Color ForeColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        /// <summary>
        /// 値を設定し、変更があった場合はInvalidateを呼び出します。
        /// </summary>
        /// <typeparam name="T">データ型</typeparam>
        /// <param name="Property">変更対象の変数</param>
        /// <param name="value">更新後データ</param>
        private void SetWithInvalidate<T>(ref T Property, in T value)
        {
            if(Property.Equals(value) == false)
            {
                Property = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// モード設定
        /// </summary>
        /// <param name="value"></param>
        private void SetMode(in eGraphic2DControlMode value)
        {
            _Mode = value;

            switch(_Mode)
            {
                case eGraphic2DControlMode.Default:
                    DefaultCuror = MouseCursors.NoneModeCursor;
                    break;
                case eGraphic2DControlMode.Select:
                    DefaultCuror = MouseCursors.SelectModeCursor;
                    break;
                default:
                    DefaultCuror = MouseCursors.NoneModeCursor;
                    break;
            }

            this.Cursor = DefaultCuror;
        }

        #endregion
    }
}
