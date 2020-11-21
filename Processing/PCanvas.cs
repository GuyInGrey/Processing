using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Processing
{
    public class PCanvas : CanvasFormUI
    {
        /// <summary>
        /// To change all things visual on the canvas, use this!
        /// </summary>
        public CanvasArt Art;

        /// <summary>
        /// Used to hide the window (frames will still be processed even if not displayed).
        /// </summary>
        public bool WindowVisible
        {
            get => Form.Visible;
            set { Form.Visible = value; if (value) { Form.Hide(); } }
        }

        internal Image CanvasImage;

        /// <summary>
        /// Width of the window.
        /// </summary>
        public int Width => CanvasImage.Width;
        /// <summary>
        /// Height of the window.
        /// </summary>
        public int Height => CanvasImage.Height;

        /// <summary>
        /// System.Drawing.Graphics object. Don't mess with this unless you're trying to do something this library does not support.
        /// </summary>
        public Graphics _Graphics;

        /// <summary>
        /// The frame rate your application is trying to run at.
        /// </summary>
        public int FrameRateTarget = 60;
        /// <summary>
        /// The current framerate the application is running at.
        /// </summary>
        public int FrameRateCurrent = 0;
        /// <summary>
        /// The frames that have been rendered since the application started.
        /// </summary>
        public int TotalFrameCount = 0;

        internal int FramesInLastSecond = 0;
        internal Stopwatch TimeSinceLastFrameRateUdate = Stopwatch.StartNew();

        internal float LastFrameTimeMilliseconds = 0f;

        /// <summary>
        /// The time since the last frame was rendered in seconds.
        /// </summary>
        public float Delta;

        public virtual void Setup() { }
        public virtual void Draw(float delta) { }

        // V1 Commented out

        //internal MethodBase DrawMethod;
        //internal MethodBase DebugDrawMethod;
        //internal MethodBase SetupMethod;

        public PCanvas()
        {
            // V1 Commented out
            
            //var childName = GetType().FullName;
            //var childType = GetType().Assembly.GetType(childName);
            //if (childType == null) { return; }
            //DrawMethod = childType.GetMethod("Draw", BindingFlags.NonPublic | BindingFlags.Instance);
            //if (DrawMethod == null)
            //{
            //    DrawMethod = childType.GetMethod("Draw", BindingFlags.Public | BindingFlags.Instance);
            //}
            //SetupMethod = childType.GetMethod("Setup", BindingFlags.NonPublic | BindingFlags.Instance);
            //if (SetupMethod == null)
            //{
            //    SetupMethod = childType.GetMethod("Setup", BindingFlags.Public | BindingFlags.Instance);
            //}
            //DebugDrawMethod = childType.GetMethod("DebugDraw", BindingFlags.NonPublic | BindingFlags.Instance);
            //if (DebugDrawMethod == null)
            //{
            //    DebugDrawMethod = childType.GetMethod("DebugDraw", BindingFlags.Public | BindingFlags.Instance);
            //}

            Art = new CanvasArt(this);
        }

        /// <summary>
        /// Creates the canvas window.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="frameRateTarget">The frame rate your application is trying to run at.</param>
        public void CreateCanvas(int width, int height, int frameRateTarget)
        {
            Initialize(width, height);

            FrameRateTarget = frameRateTarget;
            CanvasImage = new Bitmap(width, height);
            Form.pictureBox.Image = new Bitmap(width, height);
            _Graphics = Graphics.FromImage(CanvasImage);

            Form.Load += Form_Load;
            Form.pictureBox.Paint += PictureBox_Paint;

            BeginForm();
        }

        Stopwatch frameTimer = Stopwatch.StartNew();

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (!Running) { return; }


            lock (CanvasImage)
            {
                _Graphics.Dispose();
                _Graphics = Graphics.FromImage(CanvasImage);
                _Graphics.SmoothingMode = SmoothingMode.HighQuality;

                //Draw(Delta);
                try
                {
                    // V1 Commented out
                    // /*DrawMethod*/?.Invoke(this, new object[] { Delta });
                    Draw(Delta);
                }
                catch
                {
                    Console.WriteLine("You need to have `float delta` as an argument to your draw method!");
                }

                e.Graphics.DrawImage(CanvasImage, 0, 0, Width, Height);
            }

            TotalFrameCount++;
            FramesInLastSecond++;

            if (TimeSinceLastFrameRateUdate.Elapsed.TotalMilliseconds > 1000)
            {
                TimeSinceLastFrameRateUdate = Stopwatch.StartNew();
                FrameRateCurrent = FramesInLastSecond;
                FramesInLastSecond = 0;
            }

            Delta = ((float)frameTimer.Elapsed.TotalMilliseconds) / 1000f;

            var timeWantedPerFrameMS = 1000f / FrameRateTarget;
            if (timeWantedPerFrameMS > 1)
            {
                frameTimer.Stop();
                var timeToWait = (int)Math.Floor(timeWantedPerFrameMS - (float)frameTimer.Elapsed.TotalMilliseconds);
                if (timeToWait > 0)
                {
                    Thread.Sleep(timeToWait);
                    Delta = ((float)frameTimer.Elapsed.TotalMilliseconds + timeToWait) / 1000f;
                }
            }
            frameTimer = Stopwatch.StartNew();

            Form.pictureBox.Invalidate();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            lock (CanvasImage)
            {
                _Graphics = Graphics.FromImage(CanvasImage);

                // V1 Commented out
                //SetupMethod?.Invoke(this, new object[] { });
                Setup();

                _Graphics.Dispose();
            }
        }

        /// <summary>
        /// Pause the application thread for the given time. Only use if you know what you're doing, can hang application.
        /// </summary>
        /// <param name="ms">The time to pause for.</param>
        public void Delay(int ms) { if (ms > 0) { Thread.Sleep(ms); } }

        /// <summary>
        /// Set the window's title text. Advised not to run every frame.
        /// </summary>
        /// <param name="s"></param>
        public void Title(object s)
        {
            Form.Text = s.ToString();
        }

        /// <summary>
        /// Set the application's render quality.
        /// </summary>
        /// <param name="mode">The quality to render at.</param>
        public void SetQuality(RenderQuality mode)
        {
            switch (mode)
            {
                case RenderQuality.High:
                    ToHighQuality();
                    break;
                case RenderQuality.Low:
                    ToLowQuality();
                    break;
            }
        }

        internal void ToLowQuality()
        {
            _Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            _Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            _Graphics.SmoothingMode = SmoothingMode.None;
            _Graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            _Graphics.PixelOffsetMode = PixelOffsetMode.Half;
        }

        internal void ToHighQuality()
        {
            _Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            _Graphics.CompositingQuality = CompositingQuality.HighQuality;
            _Graphics.SmoothingMode = SmoothingMode.HighQuality;
            _Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            _Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }
    }
}