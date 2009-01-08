using System;
using Microsoft.Win32;

namespace Validator
{
    public class WindowStateEventArgs : EventArgs
    {
        public readonly RegistryKey Key;

        public WindowStateEventArgs(RegistryKey key)
        {
            Key = key;
        }
    }

    // event info that allows form to persist extra window state data
    public delegate void WindowStateEventHandler(object sender, WindowStateEventArgs e);
}
