using System;
using System.Drawing;

namespace Processing
{
    public static class Wallpaper
    {
        // Fetch the Progman window
        internal static IntPtr progman = W32.FindWindow("Progman", null);
        internal static Graphics _Graphics;

        /// <summary>
        /// DOES NOT WORK ON ALL SYSTEMS.
        /// </summary>
        public static CanvasArt Art { get; }

        static Wallpaper()
        {
            Start();
            Art = new CanvasArt();
        }

        public static void SetCanvas(ProcessingCanvas canvas)
        {
            //W32.SetParent(canvas.Form.Handle, workerw);
        }

        internal static void Start()
        {
            W32.SendMessageTimeout(
                progman,
                0x052C,
                new IntPtr(0),
                IntPtr.Zero,
                W32.SendMessageTimeoutFlags.SMTO_NORMAL,
                1000,
                out var result
            );

            var workerw = IntPtr.Zero;

            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                var p = W32.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    // Gets the WorkerW Window after the current one.
                    workerw = W32.FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               IntPtr.Zero);
                }

                return true;
            }), IntPtr.Zero);

            var dc = W32.GetDCEx(workerw, IntPtr.Zero, (W32.DeviceContextValues)0x403);
            if (dc != IntPtr.Zero)
            {
                _Graphics = Graphics.FromHdc(dc);
                //W32.ReleaseDC(workerw, dc);
            }
        }
    }
}