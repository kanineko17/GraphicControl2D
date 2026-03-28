using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace graphicbox2d
{
    /// <summary>
    /// メッセージ変換クラス
    /// </summary>
    internal class MessageConverter
    {
        /// <summary>
        /// Windows メッセージを基に、2Dオブジェクト削除イベント引数を生成する。
        /// </summary>
        /// <param name="m"></param>
        /// <param name="extensionData"></param>
        /// <returns></returns>
        public static Graphic2DObjectDeleteEventArgs Convert(Message m, in List<Object2D> extensionData)
        {
            return new Graphic2DObjectDeleteEventArgs(extensionData);
        }

        /// <summary>
        /// Windows メッセージを基に、2Dオブジェクト関連のイベント引数を生成する。
        /// </summary>
        /// <param name="m">処理対象の Windows メッセージ。</param>
        /// <param name="extensionData">イベントに付随する Object2D 拡張データ。</param>
        /// <returns>生成された Graphic2DObjectEventArgs。</returns>
        public static Graphic2DObjectEventArgs Convert(Message m, in Object2D extensionData)
        {
            return new Graphic2DObjectEventArgs(extensionData);
        }

        /// <summary>
        /// Windows メッセージを基に、マウスイベント関連の拡張付きドラッグ操作中イベント引数を生成する。
        /// 内部で基本的な MouseEventArgs を生成し、拡張データを付加する。
        /// </summary>
        /// <param name="m">処理対象の Windows メッセージ。</param>
        /// <param name="extensionData">イベントに付随するマウス拡張データ。</param>
        /// <param name="_Object">ドラッグ操作中オブジェクト</param>
        /// <returns></returns>
        public static Graphic2DObjectDraggingEventArgs Convert(Message m, in Graphic2DMouseEventExtensionData extensionData, in Object2D _Object)
        {
            MouseEventArgs baseArgs = Convert(m);
            return new Graphic2DObjectDraggingEventArgs(baseArgs, extensionData, _Object);
        }

        /// <summary>
        /// Windows メッセージを基に、マウスイベント関連の拡張付きイベント引数を生成する。
        /// 内部で基本的な MouseEventArgs を生成し、拡張データを付加する。
        /// </summary>
        /// <param name="m">処理対象の Windows メッセージ。</param>
        /// <param name="extensionData">イベントに付随するマウス拡張データ。</param>
        /// <returns>生成された Graphic2DMouseEventArgs。</returns>
        public static Graphic2DMouseEventArgs Convert(Message m, in Graphic2DMouseEventExtensionData extensionData)
        {
            MouseEventArgs baseArgs = Convert(m);
            return new Graphic2DMouseEventArgs(baseArgs, extensionData);
        }

        /// <summary>
        /// Windows メッセージを基に、2Dオブジェクト削除イベント引数を生成する。
        /// </summary>
        /// <param name="m"></param>
        /// <param name="extensionData"></param>
        /// <returns></returns>
        public static Graphic2DObjectDeleteEventArgs Convert(EventArgs m, in List<Object2D> extensionData)
        {
            return new Graphic2DObjectDeleteEventArgs(extensionData);
        }

        /// <summary>
        /// Windows メッセージを基に、2Dオブジェクト関連のイベント引数を生成する。
        /// </summary>
        /// <param name="m">処理対象の Windows メッセージ。</param>
        /// <param name="extensionData">イベントに付随する Object2D 拡張データ。</param>
        /// <returns>生成された Graphic2DObjectEventArgs。</returns>
        public static Graphic2DObjectEventArgs Convert(EventArgs m, in Object2D extensionData)
        {
            return new Graphic2DObjectEventArgs(extensionData);
        }

        /// <summary>
        /// Windows メッセージを基に、マウスイベント関連の拡張付きドラッグ操作中イベント引数を生成する。
        /// 内部で基本的な MouseEventArgs を生成し、拡張データを付加する。
        /// </summary>
        /// <param name="baseArgs">処理対象の MouseEventArgs。</param>
        /// <param name="extensionData">イベントに付随するマウス拡張データ。</param>
        /// <param name="_Object">ドラッグ操作中オブジェクト</param>
        /// <returns></returns>
        public static Graphic2DObjectDraggingEventArgs Convert(MouseEventArgs baseArgs, in Graphic2DMouseEventExtensionData extensionData, in Object2D _Object)
        {
            return new Graphic2DObjectDraggingEventArgs(baseArgs, extensionData, _Object);
        }

        /// <summary>
        /// Windows メッセージを基に、マウスイベント関連の拡張付きイベント引数を生成する。
        /// 内部で基本的な MouseEventArgs を生成し、拡張データを付加する。
        /// </summary>
        /// <param name="baseArgs">処理対象の MouseEventArgs。</param>
        /// <param name="extensionData">イベントに付随するマウス拡張データ。</param>
        /// <returns>生成された Graphic2DMouseEventArgs。</returns>
        public static Graphic2DMouseEventArgs Convert(MouseEventArgs baseArgs, in Graphic2DMouseEventExtensionData extensionData)
        {
            return new Graphic2DMouseEventArgs(baseArgs, extensionData);
        }

        /// <summary>
        /// Windows メッセージを基に、MouseEventArgs を生成する。
        /// lParam からマウス座標を抽出し、メッセージ種別に応じてボタン・クリック数・ホイール回転量を判定するして設定する。
        /// </summary>
        /// <param name="m">処理対象の Windows メッセージ。</param>
        /// <returns>生成された MouseEventArgs。</returns>
        public static MouseEventArgs Convert(Message m)
        {
            // マウス座標は lParam に入っている
            int x = (short)ComonUtil.GetLower16Bit(m.LParam.ToInt32());
            int y = (short)ComonUtil.GetUpper16Bit(m.LParam.ToInt32());

            // ボタン判定
            MouseButtons button = MouseButtons.None;
            switch (m.Msg)
            {
                case WinMsg.WM_LBUTTONDOWN:
                case WinMsg.WM_LBUTTONUP:
                    button = MouseButtons.Left;
                    break;
                case WinMsg.WM_RBUTTONDOWN:
                case WinMsg.WM_RBUTTONUP:
                    button = MouseButtons.Right;
                    break;
                case WinMsg.WM_MBUTTONDOWN:
                case WinMsg.WM_MBUTTONUP:
                    button = MouseButtons.Middle;
                    break;
            }

            // ボタンを押下なら1クリックとみなす
            int clicks = (m.Msg == WinMsg.WM_LBUTTONDOWN || m.Msg == WinMsg.WM_RBUTTONDOWN || m.Msg == WinMsg.WM_MBUTTONDOWN) ? 1 : 0;

            // ホイール回転量（WM_MOUSEWHEEL の場合）
            int delta = 0;
            if (m.Msg == WinMsg.WM_MOUSEWHEEL)
            {
                delta = (short)ComonUtil.GetUpper16Bit(m.WParam.ToInt32());
            }

            // MouseEventArgs を生成
            return new MouseEventArgs(button, clicks, x, y, delta);
        }
    }
}
