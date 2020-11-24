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
    public class Canvas : CanvasFormUI
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

        public Timing Timing = new Timing();

        internal int FramesInLastSecond = 0;
        internal Stopwatch TimeSinceLastFrameRateUdate = Stopwatch.StartNew();

        internal float LastFrameTimeMilliseconds = 0f;

        /// <summary>
        /// The time since the last frame was rendered in seconds.
        /// </summary>
        public float Delta;

        public virtual void Setup() { }
        public virtual void Draw(float delta) { }

        public Canvas()
        {
            Art = new CanvasArt(this);
        }

        /// <summary>
        /// Creates the canvas window.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="targetFramesPerSecond">The frame rate your application is trying to run at.</param>
        public void CreateCanvas(int width, int height, int targetFramesPerSecond)
        {
            Initialize(width, height);

            Timing.TargetFramesPerSecond = targetFramesPerSecond;
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

                Draw(Delta);

                e.Graphics.DrawImage(CanvasImage, 0, 0, Width, Height);
            }

            Timing.FrameRendered();

            Delta = ((float)frameTimer.Elapsed.TotalMilliseconds) / 1000f;

            var timeWantedPerFrameMS = 1000f / Timing.TargetFramesPerSecond;
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