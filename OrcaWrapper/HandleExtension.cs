using System.Text;
using System.Runtime.InteropServices;

public static class HandleExtension
{
    // http://tarukichi.chu.jp/codetips/sendkeys.html
    [DllImport("user32.dll")]
    static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string? lpszClass, string? lpszWindow);
    [DllImport("user32.dll")]
    static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [DllImport("user32.dll")]
    static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [DllImport("user32.dll")]
    static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, StringBuilder lParam);
    [DllImport("user32.dll")]
    static extern int VkKeyScan(char ch);
    /// <summary>
    /// <see href="http://pinvoke.net/default.aspx/user32/SetWindowLongPtr.html"/>
    /// </summary>
    static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        if (IntPtr.Size == 8)
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        else
            return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));

        [DllImport("user32.dll", EntryPoint="SetWindowLong")]
        static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint="SetWindowLongPtr")]
        static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
    }
    /// <summary>
    /// <see href="http://pinvoke.net/default.aspx/user32/GetWindowLongPtr.html"/>
    /// </summary>
    static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {
        if (IntPtr.Size == 8)
            return GetWindowLongPtr64(hWnd, nIndex);
        else
            return GetWindowLongPtr32(hWnd, nIndex);

        [DllImport("user32.dll", EntryPoint="GetWindowLong")]
        static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint="GetWindowLongPtr")]
        static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);
    }

    [DllImport("user32.dll")]
    static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
    [DllImport("user32.dll")]
    static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    [DllImport("user32.dll", SetLastError=true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

    /// <summary>
    /// クラス名とキャプションを指定して、直下の子ウィンドウを取得する
    /// </summary>
    public static IntPtr GetChildHandle(this IntPtr hwndParent, string? lpszClass, string? lpszWindow)
    {
        var ret = FindWindowEx(hwndParent, IntPtr.Zero, lpszClass, lpszWindow);
        if (ret == IntPtr.Zero) throw new Exception(String.Format("No such child. {0}, {1}", lpszClass, lpszWindow));
        return ret;
    }

    /// <summary>
    /// コントロールを左クリックする
    /// </summary>
    public static void LeftClick(this IntPtr handle)
    {
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;

        PostMessage(handle, WM_LBUTTONDOWN, 1, 0);
        Thread.Sleep(5);
        PostMessage(handle, WM_LBUTTONUP, 1, 0);
    }

    /// <summary>
    /// <see href="https://docs.microsoft.com/windows/win32/inputdev/virtual-key-codes"/>
    /// </summary>
    public static void SendKey(this IntPtr handle, char ch) { handle.SendKey(VkKeyScan(ch)); }
    /// <summary>
    /// <see href="https://docs.microsoft.com/windows/win32/inputdev/virtual-key-codes"/>
    /// </summary>
    public static void SendKey(this IntPtr handle, int keyCode)
    {
        const int WM_KEYDOWN = 0x0100;
        //const int WM_KEYUP = 0x0101;

        PostMessage(handle, WM_KEYDOWN, keyCode, 0);
        // Thread.Sleep(5);
        // PostMessage(handle, WM_KEYUP, keyCode, 0);
    }

    /// <summary>
    /// コントロールに対応するテキストを取得する<br/>
    /// <see href="https://docs.microsoft.com/windows/win32/winmsg/wm-gettext"/>
    /// </summary>
    public static string GetText(this IntPtr handle)
    {
        const int WM_GETTEXT = 0x000D;

        StringBuilder sb = new StringBuilder(256);
        // 本当はいろいろある
        // ポインタの渡し方としては乱暴すぎる
        // https://ikorin2.hatenablog.jp/entry/2020/03/10/181417
        SendMessage(handle, WM_GETTEXT, sb.Capacity, sb);
        return sb.ToString();
    }

    /// <summary>
    /// コントロールにテキストを設定する<br/>
    /// <see href="https://docs.microsoft.com/windows/win32/winmsg/wm-settext"/>
    /// </summary>
    public static void SetText(this IntPtr handle, string str)
    {
        const int WM_SETTEXT = 0x000C;

        SendMessage(handle, WM_SETTEXT, 0, new StringBuilder(str));
    }

    /// <summary>
    /// 「閉じる」ボタン(x)を無効化する<br/>
    /// <see href="https://devblogs.microsoft.com/oldnewthing/20100604-00/?p=13803"/>
    /// </summary>
    public static void DisableCloseButton(this IntPtr handle)
    {
        const int SC_CLOSE = 0xF060;
        const int MF_BYCOMMAND = 0x00000000;
        const int MF_DISABLED = 0x00000002;
        const int MF_GRAYED = 0x00000001;

        EnableMenuItem(GetSystemMenu(handle, false), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
    }

    /// <summary>
    /// 「最小化」ボタンを無効化する<br/>
    /// <see href="https://devblogs.microsoft.com/oldnewthing/20100604-00/?p=13803"/>
    /// </summary>
    public static void DisableMinimizeButton(this IntPtr handle)
    {
        const int GWL_STYLE = -16;
        const int WS_MINIMIZEBOX = 0x00020000;

        SetWindowLongPtr(handle, GWL_STYLE, (IntPtr)((int)GetWindowLongPtr(handle, GWL_STYLE) & ~WS_MINIMIZEBOX));
    }
    /// <summary>
    /// 「最大化」ボタンを無効化する<br/>
    /// <see href="https://devblogs.microsoft.com/oldnewthing/20100604-00/?p=13803"/>
    /// </summary>
    public static void DisableMaximizeButton(this IntPtr handle)
    {
        const int GWL_STYLE = -16;
        const int WS_MAXIMIZEBOX = 0x00010000;

        SetWindowLongPtr(handle, GWL_STYLE, (IntPtr)((int)GetWindowLongPtr(handle, GWL_STYLE) & ~WS_MAXIMIZEBOX));
    }

    /// <summary>
    /// ウィンドウを最前面に固定する
    /// </summary>
    public static void SetWindowTopMost(this IntPtr handle)
    {
        const int HWND_TOPMOST = -1;
        const int SWP_SHOWWINDOW = 0x40;
        const int SWP_NOSIZE = 0x1;
        const int SWP_NOMOVE = 0x2;
        
        SetWindowPos(handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_SHOWWINDOW | SWP_NOSIZE | SWP_NOMOVE);
    }
}