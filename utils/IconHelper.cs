using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SoundFader.utils
{
    internal class IconHelper
    {
        private static object lockobj = new();

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [Out, MarshalAs(UnmanagedType.Interface)] out IShellItemImageFactory ppv);

        [ComImport]
        [Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItemImageFactory
        {
            [PreserveSig]
            int GetImage(
                [In, MarshalAs(UnmanagedType.Struct)] SIZE size,
                [In] SIIGBF flags,
                out IntPtr phbm);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SIZE
        {
            public int cx;
            public int cy;

            public SIZE(int cx, int cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        [Flags]
        private enum SIIGBF
        {
            ResizeToFit = 0x00,
            BiggerSizeOk = 0x01,
            MemoryOnly = 0x02,
            IconOnly = 0x04,
            ThumbnailOnly = 0x08,
            InCacheOnly = 0x10,
        }

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        internal static Bitmap GetHighResolutionIcon(string filePath, int width, int height)
        {
            try
            {
                lock (lockobj)
                {
                    Guid shellItemImageFactoryGuid = typeof(IShellItemImageFactory).GUID;

                    SHCreateItemFromParsingName(filePath, IntPtr.Zero, shellItemImageFactoryGuid, out IShellItemImageFactory imageFactory);

                    SIZE size = new SIZE(width, height);
                    IntPtr hBitmap;
                    SIIGBF flags = SIIGBF.ResizeToFit | SIIGBF.BiggerSizeOk;

                    int hr = imageFactory.GetImage(size, flags, out hBitmap);
                    if (hr != 0)
                    {
                        throw Marshal.GetExceptionForHR(hr);
                    }

                    Bitmap bitmap = Image.FromHbitmap(hBitmap);

                    // HBITMAPのリソースを解放
                    DeleteObject(hBitmap);

                    return bitmap;
                }
            }
            catch
            {
                return null;
            }
        }

        internal static Bitmap GetAppIcon(string filePath)
        {
            var icon = Icon.ExtractAssociatedIcon(filePath);
            if (icon != null)
            {
                return icon.ToBitmap();
            }
            return null;
        }
    }
}
