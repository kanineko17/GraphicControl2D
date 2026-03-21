using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// WindowsAPI
    /// </summary>
    internal class WinAPI
    {
        // SendMessage の宣言
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        // PostMessage の宣言
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    }

    /// <summary>
    /// Windows メッセージ
    /// </summary>
    internal static class WinMsg
    {
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_MOUSEMOVE   = 0x0200;
        public const int WM_LBUTTONUP   = 0x0202;
        public const int WM_RBUTTONUP   = 0x0205;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP   = 0x0208;
        public const int WM_MOUSEWHEEL  = 0x020A;
        public const int WM_KEYDOWN     = 0x0100;

        public const int WM_USER = 0x0400;
        public const int WM_SUSIKI_CALC_START     = WM_USER + 1;
        public const int WM_SUSIKI_CALC_END       = WM_USER + 2;
        public const int WM_MOUSE_ENTER_ON_OBJECT = WM_USER + 3;
        public const int WM_MOUSE_LEAVE_ON_OBJECT = WM_USER + 4;
        public const int WM_OBJECT_DELETE         = WM_USER + 5;
    }

    /// <summary>
    /// Windows Param
    /// </summary>
    internal static class WinParam
    {
        public const int VK_DELETE = 0x02E;
    }
}
