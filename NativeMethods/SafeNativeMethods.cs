using System;
using System.Runtime.InteropServices;
using System.Security;

namespace GomuLibrary.Win32API
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressUnmanagedCodeSecurity()]
    internal static class SafeNativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess,
            uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
            uint dwFlagsAndAttributes, IntPtr hTemplateFile);
    }
}
