using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graphicbox2d
{
    // ===============================================================================
    //   公開 イベントハンドラー
    // ===============================================================================

    /// <summary>
    /// Graphic2DMouseEventArgs 専用のイベントハンドラー
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void Graphic2DMouseEventHandler(object sender, Graphic2DMouseEventArgs e);

    /// <summary>
    /// graphic2DObjectEventArgs 専用のイベントハンドラー
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void Graphic2DObjectEventHandler(object sender, Graphic2DObjectEventArgs e);

    /// <summary>
    /// graphic2DObjectDeleteEventArgs 専用のイベントハンドラー
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void Graphic2DObjectDleteEventHandler(object sender, Graphic2DObjectDeleteEventArgs e);


    /// <summary>
    /// graphic2DObjectDraggingEventArgs 専用のイベントハンドラー
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void Graphic2DObjectDraggingEventHandler(object sender, Graphic2DObjectDraggingEventArgs e);

    // ===============================================================================
    //   公開 イベントメッセージ
    // ===============================================================================

    /// <summary>
    /// 2Dグラフィックコントロール用のオブジェクト削除操作イベント引数クラス。
    /// </summary>
    public class Graphic2DObjectDeleteEventArgs : EventArgs
    {
        /// <summary>
        /// 削除されたオブジェクトリスト
        /// </summary>
        public List<Object2D> DeleteObject;

        /// <summary>
        /// 削除操作キャンセルフラグ
        /// </summary>
        public bool Cancel = false;

        /// <summary>
        /// コンストラクタ。
        /// ドラッグ操作中のオブジェクト情報を取り込む。
        /// </summary>
        /// <param name="_DeleteObject">削除されたオブジェクトリスト</param>
        public Graphic2DObjectDeleteEventArgs(List<Object2D> _DeleteObject)
        {
            // 削除されたオブジェクトリストをコピー
            DeleteObject = _DeleteObject;
        }
    }

    /// <summary>
    /// 2Dグラフィックコントロール用の操作中オブジェクト操作イベント引数クラス。
    /// </summary>
    public class Graphic2DObjectEventArgs : EventArgs
    {
        /// <summary>
        /// 操作中のオブジェクト
        /// </summary>
        public Object2D Object;

        /// <summary>
        /// コンストラクタ。
        /// ドラッグ操作中のオブジェクト情報を取り込む。
        /// </summary>
        /// <param name="_Object">ドラッグ操作中オブジェクト</param>
        public Graphic2DObjectEventArgs(Object2D _Object)
        {
            // 操作中のオブジェクトをコピー
            Object = _Object;
        }
    }

    /// <summary>
    /// 2Dグラフィックコントロール用のオブジェクトドラッグ操作中イベント引数クラス。
    /// </summary>
    public class Graphic2DObjectDraggingEventArgs : Graphic2DMouseEventArgs
    {
        /// <summary>
        /// 操作中のオブジェクト
        /// </summary>
        public Object2D Object;

        /// <summary>
        /// コンストラクタ。
        /// ドラッグ操作中のオブジェクト情報を取り込む。
        /// </summary>
        /// <param name="_Object">ドラッグ操作中オブジェクト</param>
        /// <param name="e">マウスイベント引数</param>
        /// <param name="extensionData">イベントに付随するマウス拡張データ</param>
        public Graphic2DObjectDraggingEventArgs(MouseEventArgs e, in Graphic2DMouseEventExtensionData extensionData, Object2D _Object)
            : base(e , in extensionData)
        {
            // 操作中のオブジェクトをコピー
            Object = _Object;
        }
    }

    /// <summary>
    /// 2Dグラフィックコントロール用に拡張されたマウスイベント引数クラス。
    /// 標準の MouseEventArgs に加えて、直前のマウス操作座標を保持する。
    /// </summary>
    public class Graphic2DMouseEventArgs : MouseEventArgs
    {
        /// <summary>
        /// 直前のマウスダウン位置（X座標）
        /// </summary>
        public int Last_MouseDown_X;

        /// <summary>
        /// 直前のマウスダウン位置（Y座標）
        /// </summary>
        public int Last_MouseDown_Y;

        /// <summary>
        /// 直前のマウスアップ位置（X座標）
        /// </summary>
        public int Last_MouseUp_X;

        /// <summary>
        /// 直前のマウスアップ位置（Y座標）
        /// </summary>
        public int Last_MouseUp_Y;

        /// <summary>
        /// 直前のマウス移動位置（X座標）
        /// </summary>
        public int Last_Move_X;

        /// <summary>
        /// 直前のマウス移動位置（Y座標）
        /// </summary>
        public int Last_Move_Y;

        /// <summary>
        /// コンストラクタ。
        /// 標準の MouseEventArgs を継承しつつ、拡張データから直前の座標情報を取り込む。
        /// </summary>
        /// <param name="e">現在のマウスイベントデータ</param>
        /// <param name="extensionData">拡張マウスイベントデータ（直前の座標情報などを保持）</param>
        public Graphic2DMouseEventArgs(MouseEventArgs e, in Graphic2DMouseEventExtensionData extensionData)
            : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {
            // 拡張データから直前の座標をコピー
            Last_MouseDown_X = extensionData.Last_MouseDown_Point.X;
            Last_MouseDown_Y = extensionData.Last_MouseDown_Point.Y;

            Last_MouseUp_X = extensionData.Last_MouseUp_Point.X;
            Last_MouseUp_Y = extensionData.Last_MouseUp_Point.Y;

            Last_Move_X = extensionData.Last_Move_Point.X;
            Last_Move_Y = extensionData.Last_Move_Point.Y;
        }
    }
}
