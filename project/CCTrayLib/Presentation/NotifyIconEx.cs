using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    /// <summary>
    /// This interface is used internally by the <see cref="NotifyIconEx.NotifyIconTarget"/>
    /// to communicate events and notifiactions back out.
    /// </summary>
    internal interface INotifyIconEx
    {
        uint IconId { get; }

        void NotifyClick();
        void NotifyDoubleClick();
        void NotifyShowContextMenu();
        void NotifyClickBalloon();
        void NotifyRecreateIcon();
        void NotifyBalloonShown();
        void NotifyBalloonHidden();
    }

    /// <summary>
    /// Class originally by Joel Matthias, and published into the public domain on
    /// CodeProject.com at:
    /// http://www.codeproject.com/cs/miscctrl/notifyiconex.asp
    /// 
    /// Adapted for use with ccnet-tray by Drew Noakes.
    /// 
    /// MSDN Article on Taskbar and Notification Icons:
    /// http://msdn.microsoft.com/en-us/library/cc144179.aspx
    /// </summary>
    public class NotifyIconEx : Component, INotifyIconEx
    {
        private const uint MSG_ICON_CALLBACK = WM_USER;
        private const uint WM_USER = 0x400;

        private static readonly INotifyIconManager manager = new NotifyIconTarget();

        /// <summary>
        /// The ID assigned to this icon by the <see cref="INotifyIconManager"/>.
        /// </summary>
        private uint icon_id;
        private ContextMenu contextMenu;
        private Icon icon;
        private string tooltip = string.Empty;
        private bool visible;
        private bool m_doubleClick; // fix for extra mouse up message we want to discard

        private bool HasBeenCreated
        {
            get { return icon_id != 0; }
        }

        public string Text
        {
            set
            {
                if (tooltip != value)
                {
                    tooltip = value;
                    CreateOrUpdate();
                }
            }
            get { return tooltip; }
        }

        public Icon Icon
        {
            set
            {
                icon = value;
                CreateOrUpdate();
            }
            get { return icon; }
        }

        public ContextMenu ContextMenu
        {
            set { contextMenu = value; }
            get { return contextMenu; }
        }

        public bool Visible
        {
            set
            {
                if (visible != value)
                {
                    visible = value;
                    CreateOrUpdate();
                }
            }
            get { return visible; }
        }

        uint INotifyIconEx.IconId
        {
            get { return icon_id; }
        }

        void INotifyIconEx.NotifyClick()
        {
            if (!m_doubleClick)
            {
                // Only run the click handler if we're not in the middle of a double click.
                // A double click raises a DoubleClick event, but also two separate SingleClick events.
                if (Click != null) Click(this, EventArgs.Empty);
            }
            m_doubleClick = false;
        }

        void INotifyIconEx.NotifyDoubleClick()
        {
            m_doubleClick = true;
            if (DoubleClick != null) DoubleClick(this, EventArgs.Empty);
        }

        void INotifyIconEx.NotifyShowContextMenu()
        {
            if (contextMenu == null)
                return;

            POINT point = new POINT();
            GetCursorPos(ref point);

            // this ensures that if we show the menu and then click on another window the menu will close
            SetForegroundWindow(manager.WindowHandle);

            // call non public member of ContextMenu
            contextMenu.GetType().InvokeMember(
                "OnPopup",
                BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance,
                null, contextMenu, new Object[] {EventArgs.Empty});

            TrackPopupMenuEx(contextMenu.Handle, 64, point.x, point.y, manager.WindowHandle, IntPtr.Zero);
        }

        void INotifyIconEx.NotifyClickBalloon()
        {
            if (BalloonClick != null) BalloonClick(this, EventArgs.Empty);
        }

        void INotifyIconEx.NotifyRecreateIcon()
        {
            if (HasBeenCreated)
                Create();
        }

        void INotifyIconEx.NotifyBalloonShown()
        {
            if (BalloonShown != null) BalloonShown(this, EventArgs.Empty);
        }

        void INotifyIconEx.NotifyBalloonHidden()
        {
            if (BalloonDismissed != null) BalloonDismissed(this, EventArgs.Empty);
        }

        public event EventHandler Click;
        public event EventHandler DoubleClick;
        /// <summary>
        /// Raised when the user clicks anywhere in a Balloon except the close button.
        /// </summary>
        public event EventHandler BalloonClick;
        /// <summary>
        /// Raised when a Balloon is shown.
        /// </summary>
        public event EventHandler BalloonShown;
        /// <summary>
        /// Raised when a Balloon is closed by timing out or by the user
        /// clicking the close button.
        /// </summary>
        public event EventHandler BalloonDismissed;

        /// <summary>
        /// Sets fields of the given <see cref="NotifyIconData"/> structure
        /// that are used when Adding or Modifying the tray icon.
        /// </summary>
        private void SetComonNotifyIconFields(ref NotifyIconData data)
        {
            // this null check has been introduced by Drew... I don't know why it's needed
            // but the app was crashing sometimes -- especially when ccnet is calling through
            // to a nant task that's crashed, for example.
            // this can cause the icon to disappear until the remoting starts to work again,
            // but this is preferable to having the app crash
            if (icon != null)
                data.hIcon = icon.Handle;
            data.uFlags |= NotifyFlags.Icon;

            data.szTip = tooltip;
            data.uFlags |= NotifyFlags.Tip;
            data.uFlags |= NotifyFlags.State;

            if (!visible)
                data.dwState = NotifyState.Hidden;

            data.dwStateMask |= NotifyState.Hidden;
        }

        private void CreateNewIcon()
        {
            if (HasBeenCreated)
                return;

            if (icon == null) 
                return;

            icon_id = manager.GetNextId();
            Create();
            SetIconVersion();
            manager.RegisterIcon(this);
        }

        /// <summary>
        /// Updates this icon in the tasbar, or creates it if hasn't been created yet.
        /// </summary>
        private void CreateOrUpdate()
        {
            if (DesignMode)
                return;

            if (HasBeenCreated)
                Update();
            else
                CreateNewIcon();
        }

        private NotifyIconData NotifyIconDataFactory()
        {
            NotifyIconData data = new NotifyIconData();
            data.cbSize = (uint) Marshal.SizeOf(data);
            data.hWnd = manager.WindowHandle;
            data.uID = icon_id;
            return data;
        }

        private void SetIconVersion()
        {
            NotifyIconData data = NotifyIconDataFactory();
            data.uTimeoutOrVersion = (uint) NotifyVersion.Windows2000;
            Shell_NotifyIcon(NotifyCommand.SetVersion, ref data);
        }

        private void Create()
        {
            NotifyIconData data = NotifyIconDataFactory();

            data.uCallbackMessage = MSG_ICON_CALLBACK;
            data.uFlags |= NotifyFlags.Message;

            SetComonNotifyIconFields(ref data);
            Shell_NotifyIcon(NotifyCommand.Add, ref data);
        }

        private void Update()
        {
            NotifyIconData data = NotifyIconDataFactory();
            SetComonNotifyIconFields(ref data);
            Shell_NotifyIcon(NotifyCommand.Modify, ref data);
        }

        protected override void Dispose(bool disposing)
        {
            Remove();
            base.Dispose(disposing);
        }

        public void Remove()
        {
            if (!HasBeenCreated) return;

            manager.UnregisterIcon(this);
            NotifyIconData data = NotifyIconDataFactory();
            Shell_NotifyIcon(NotifyCommand.Delete, ref data);
            icon_id = 0;
        }

        public void ShowBalloon(string title, string text, NotifyInfoFlags type, int timeoutInMilliseconds)
        {
            if (timeoutInMilliseconds < 0)
                throw new ArgumentException("The parameter must be positive", "timeoutInMilliseconds");

            // From MSDN:
            // "The system minimum and maximum timeout values are currently set at 10 seconds and 30 seconds, respectively."

            NotifyIconData data = NotifyIconDataFactory();

            data.uFlags = NotifyFlags.Info;
            data.uTimeoutOrVersion = (uint) timeoutInMilliseconds;
            data.szInfoTitle = title;
            data.szInfo = text;
            data.dwInfoFlags = type;

            Shell_NotifyIcon(NotifyCommand.Modify, ref data);
        }

        private interface INotifyIconManager
        {
            IntPtr WindowHandle { get; }
            uint GetNextId();

            void RegisterIcon(INotifyIconEx icon);
            void UnregisterIcon(INotifyIconEx icon);
        }

        /// <summary>
        /// These correspond to the NIM_ constants in the Windows API.
        /// </summary>
        private enum NotifyCommand
        {
            Add  =  0,
            Modify = 1,
            Delete = 2,
            SetVersion = 4,
        }

        /// <summary>
        /// These correspond to the NIF_ constants in the Windows API.
        /// </summary>
        [Flags]
        private enum NotifyFlags
        {
            Message = 0x01,
            Icon = 0x02,
            Tip = 0x04,
            State = 0x08,
            Info = 0x10,
        }

        [Flags]
        private enum NotifyState
        {
            Hidden = 0x01
        }

        /// <summary>
        /// These correspond to the NOTIFYICON_VERSION constants in the Windows API.
        /// </summary>
        private enum NotifyVersion
        {
            /// <summary>
            /// Pre-Windows 2000 behavior, the default.
            /// </summary>
            Old = 0,
            /// <summary>
            /// Use enhancements present in Windows 2000 and XP.
            /// </summary>
            Windows2000 = 3,
            /// <summary>
            /// Use Windows Vista enhancements.
            /// </summary>
            Vista = 4,
        }

        private class NotifyIconTarget : Form, INotifyIconManager
        {
            #region NotifyIcon Notifications
            /// <summary>
            /// This notification is sent if the balloon is hidden by a system event,
            /// such as the icon being removed from the tray. It is NOT sent in response
            /// to the user clicking the "X" button.
            /// </summary>
            private const uint NIN_BALLOONHIDE = WM_USER+3;
            /// <summary>
            /// This notifcation is sent when the balloon is shown.
            /// </summary>
            private const uint NIN_BALLOONSHOW = WM_USER+2;
            /// <summary>
            /// This notification is sent if the balloon is hidden by timing out
            /// OR if the user clicks the "X" button to manually dismiss the baloon.
            /// </summary>
            private const uint NIN_BALLOONTIMEOUT = WM_USER+4;
            /// <summary>
            /// This notification is sent if the user clicks anywhere in the balloon EXCEPT
            /// the "X" button.
            /// </summary>
            private const uint NIN_BALLOONUSERCLICK = WM_USER+5;
            #endregion

            #region Standard Windows messages
            private const uint WM_LBUTTONDBLCLK = 0x203;
            private const uint WM_LBUTTONDOWN = 0x201;
            private const uint WM_LBUTTONUP = 0x202;
            private const uint WM_MOUSEMOVE = 0x200;
            private const uint WM_RBUTTONUP = 0x205;
            #endregion

            private static uint next_icon_id;
            private readonly List<INotifyIconEx> icons = new List<INotifyIconEx>();

            /// <summary>
            /// Applications should handle this message to re-add notification icons after,
            /// for instance, an Explorer restart.
            /// </summary>
            private readonly uint WM_TASKBARCREATED = RegisterWindowMessage("TaskbarCreated");

            private IntPtr cachedWindowHandle = IntPtr.Zero;

            public NotifyIconTarget()
            {
                Text = "Hidden NotifyIconTarget Window";
            }

            public uint GetNextId()
            {
                next_icon_id++;
                return next_icon_id;
            }

            public void RegisterIcon(INotifyIconEx notifyIcon)
            {
                icons.Add(notifyIcon);
            }

            public void UnregisterIcon(INotifyIconEx notifyIcon)
            {
                icons.Remove(notifyIcon);
            }

            /// <summary>
            /// Cache the associated Window Handle when it is created,
            /// so we can expose the same handle back out even if the Form
            /// has already been disposed.
            /// </summary>
            public IntPtr WindowHandle
            {
                get
                {
                    if (cachedWindowHandle == IntPtr.Zero)
                        cachedWindowHandle = Handle;

                    return cachedWindowHandle;
                }
            }

            protected override void DefWndProc(ref Message msg)
            {
                if (msg.Msg == MSG_ICON_CALLBACK)
                {
                    ForwardEvents(msg);
                }
                else if (msg.Msg == WM_TASKBARCREATED)
                {
                    icons.ForEach(delegate(INotifyIconEx x) {x.NotifyRecreateIcon(); });
                }
                else
                {
                    base.DefWndProc(ref msg);
                }
            }

            private void ForwardEvents(Message msg)
            {
                uint msgId = (uint) msg.LParam;
                uint which_icon = (uint) msg.WParam;

                INotifyIconEx icon = icons.Find(delegate(INotifyIconEx x) { return x.IconId == which_icon;  });
                if (icon == null) return;

                switch (msgId)
                {
                    case WM_LBUTTONUP:
                        icon.NotifyClick();
                        break;

                    case WM_LBUTTONDBLCLK:
                        icon.NotifyDoubleClick();
                        break;

                    case WM_RBUTTONUP:
                        icon.NotifyShowContextMenu();
                        break;

                    case NIN_BALLOONUSERCLICK:
                        icon.NotifyClickBalloon();
                        break;

                    case NIN_BALLOONSHOW:
                        icon.NotifyBalloonShown();
                        break;

                    case NIN_BALLOONTIMEOUT:
                        icon.NotifyBalloonHidden();
                        break;
                }
            }
        }

        #region Platform Invoke

        [DllImport("shell32.dll")]
        private static extern Int32 Shell_NotifyIcon(NotifyCommand cmd, ref NotifyIconData data);

        [DllImport("user32.dll")]
        private static extern Int32 TrackPopupMenuEx(IntPtr hMenu,
                                                     UInt32 uFlags,
                                                     Int32 x,
                                                     Int32 y,
                                                     IntPtr hWnd,
                                                     IntPtr ignore);

        [DllImport("user32.dll")]
        private static extern Int32 GetCursorPos(ref POINT point);

        [DllImport("user32.dll")]
        private static extern Int32 SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint RegisterWindowMessage(string lpString);

        [StructLayout(LayoutKind.Sequential)]
        private struct NotifyIconData
        {
            public UInt32 cbSize; // DWORD
            public IntPtr hWnd; // HWND
            public UInt32 uID; // UINT
            public NotifyFlags uFlags; // UINT
            public UInt32 uCallbackMessage; // UINT
            public IntPtr hIcon; // HICON
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public String szTip;
            public NotifyState dwState; // DWORD
            public NotifyState dwStateMask; // DWORD
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public String szInfo;
            public UInt32 uTimeoutOrVersion; // UINT
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public String szInfoTitle;
            public NotifyInfoFlags dwInfoFlags; // DWORD
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public Int32 x;
            public Int32 y;
        }

        #endregion
    }
}
