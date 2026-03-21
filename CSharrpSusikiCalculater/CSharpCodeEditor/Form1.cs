using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;


namespace CSharpCodeEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            scintilla1.Dock = DockStyle.Fill;

            // 行番号を表示
            scintilla1.Margins[0].Width = 40;

            // シンタックスハイライト (C#風)
            // デフォルトスタイルを黒背景に
            scintilla1.StyleResetDefault();
            scintilla1.Styles[Style.Default].BackColor = Color.Black;
            scintilla1.Styles[Style.Default].ForeColor = Color.White;
            scintilla1.Styles[Style.Default].Font = "Consolas";
            scintilla1.Styles[Style.Default].Size = 10;
            scintilla1.StyleClearAll();

            // コメントを緑色に
            scintilla1.Styles[Style.Cpp.Comment].ForeColor = Color.LightGreen;
            scintilla1.Styles[Style.Cpp.CommentLine].ForeColor = Color.LightGreen;

            // 数字をオレンジ色に
            scintilla1.Styles[Style.Cpp.Number].ForeColor = Color.Orange;

            // 文字列を黄色に
            scintilla1.Styles[Style.Cpp.String].ForeColor = Color.Yellow;
            scintilla1.Styles[Style.Cpp.Character].ForeColor = Color.Yellow;

            // キーワードを水色に
            scintilla1.Styles[Style.Cpp.Word].ForeColor = Color.DeepSkyBlue;
            scintilla1.Styles[Style.Cpp.Word].Bold = true;

            // プリプロセッサを紫色に
            scintilla1.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Violet;

            // 行番号の背景も黒に
            scintilla1.Styles[Style.LineNumber].BackColor = Color.Black;
            scintilla1.Styles[Style.LineNumber].ForeColor = Color.Gray;
            scintilla1.Margins[0].Width = 40;



            // キーワード登録 (C#の予約語)
            scintilla1.SetKeywords(0, "class public private void int string return if else for while using namespace const");

            scintilla1.SetProperty("fold", "1");
            scintilla1.Margins[2].Type = MarginType.Symbol;
            scintilla1.Margins[2].Mask = Marker.MaskFolders;
            scintilla1.Margins[2].Sensitive = true;
            scintilla1.Margins[2].Width = 20;

        }
    }
}
