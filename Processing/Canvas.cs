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
        public CanvasArt Art;

        public bool WindowVisible
        {
            get => Form.Visible;
            set { Form.Visible = value; if (value) { Form.Hide(); } }
        }

        internal Image CanvasImage;
        public int Width => CanvasImage.Width;
        public int Height => CanvasImage.Height;

        public Graphics _Graphics;

        public int FrameRateTarget = 60;
        public int FrameRateCurrent = 0;
        public int TotalFrameCount = 0;

        internal int FramesInLastSecond = 0;
        internal Stopwatch TimeSinceLastFrameRateUdate = Stopwatch.StartNew();

        internal float LastFrameTimeMilliseconds = 0f;

        public float Delta;

        public MethodBase DrawMethod;
        public MethodBase DebugDrawMethod;
        public MethodBase SetupMethod;

        public Canvas()
        {
            var childName = GetType().FullName;
            var childType = GetType().Assembly.GetType(childName);
            if (childType == null) { return; }
            DrawMethod = childType.GetMethod("Draw", BindingFlags.NonPublic | BindingFlags.Instance);
            if (DrawMethod == null)
            {
                DrawMethod = childType.GetMethod("Draw", BindingFlags.Public | BindingFlags.Instance);
            }
            SetupMethod = childType.GetMethod("Setup", BindingFlags.NonPublic | BindingFlags.Instance);
            if (SetupMethod == null)
            {
                SetupMethod = childType.GetMethod("Setup", BindingFlags.Public | BindingFlags.Instance);
            }
            DebugDrawMethod = childType.GetMethod("DebugDraw", BindingFlags.NonPublic | BindingFlags.Instance);
            if (DebugDrawMethod == null)
            {
                DebugDrawMethod = childType.GetMethod("DebugDraw", BindingFlags.Public | BindingFlags.Instance);
            }

            Art = new CanvasArt(this);
        }

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
                    DrawMethod?.Invoke(this, new object[] { Delta });
                    DebugDrawMethod?.Invoke(this, new object[] { Delta });
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
                SetupMethod?.Invoke(this, new object[] { });
                _Graphics.Dispose();
            }
        }

        public void Delay(int ms) { if (ms > 0) { Thread.Sleep(ms); } }

        public void Title(object s)
        {
            Form.Text = s.ToString();
        }

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