using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Processing
{
    public class CanvasArt
    {
        public Graphics Graphics => Mode == 0 ? Canvas._Graphics : Mode == 1 ? Sprite.Graphics : Wallpaper._Graphics;
        public Image CanvasImage => Mode == 0 ? Canvas.CanvasImage : Mode == 1 ? Sprite._Image : null;
        public int Width => CanvasImage.Width;
        public int Height => CanvasImage.Height;

        public Canvas Canvas;
        public PSprite Sprite;

        public int Mode = 0;

        public CanvasArt(Canvas canvas)
        {
            Canvas = canvas;
            Mode = 0;
        }

        public CanvasArt(PSprite sprite)
        {
            Sprite = sprite;
            Mode = 1;
        }

        internal CanvasArt()
        {
            Mode = 2;
        }

        private PColor _Stroke = PColor.Black;
        private PColor _Fill = null;
        private Pen StrokeColor => new Pen(_Stroke.ToColor(), _StrokeWeight) { EndCap = LineCap.Round, StartCap = LineCap.Round };
        private SolidBrush FillColor => new SolidBrush(_Fill.ToColor());

        private PFont _Font = null;
        private float _StrokeWeight = 1f;

        public List<(float, float)> Vertices;

        public bool Set(int x, int y, PColor color)
        {
            if (x < 0 || x >= Width) { return false; }
            if (y < 0 || y >= Height) { return false; }

            ((Bitmap)CanvasImage).SetPixel(x, y, color.ToColor());

            return true;
        }

        public void Background(PColor color)
        {
            var buffer = 10;
            Graphics.FillRectangle(new SolidBrush(color.ToColor()), new Rectangle(-buffer, -buffer, Width + (buffer * 2), Height + (buffer * 2)));
        }

        public void Text(string val, int x, int y)
        {
            if (_Fill == null || _Font == null) { return; }

            Graphics.DrawString(val, _Font.Font, FillColor, new Point(x, y),
                new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
        }

        public void Stroke(PColor p) { _Stroke = p; }
        public void NoStroke() { _Stroke = null; }
        public void StrokeWeight(float width) { _StrokeWeight = width; }

        public void Fill(PColor p) { _Fill = p; }
        public void NoFill() { _Fill = null; }

        public PFont CreateFont(string name, float size) =>
            new PFont() { Font = new Font(name, size) };

        public void TextFont(PFont f) { _Font = f; }

        public byte[] GetPixels()
        {
            var b = ((Bitmap)CanvasImage);
            var data = b.LockBits(
                new Rectangle(0, 0, Width, Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );

            var ptr = data.Scan0;
            var bytes = Math.Abs(data.Stride) * Height;
            var argbValues = new byte[bytes];
            Marshal.Copy(ptr, argbValues, 0, bytes);
            b.UnlockBits(data);

            return argbValues;
        }

        public void SetPixels(byte[] argbValues)
        {
            var b = ((Bitmap)CanvasImage);
            var data = b.LockBits(
                new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb
            );

            var ptr = data.Scan0;
            var bytes = Math.Abs(data.Stride) * Height;
            Marshal.Copy(argbValues, 0, ptr, bytes);

            b.UnlockBits(data);
        }

        public PColor GetPixel(int x, int y) =>
            PColor.FromColor(((Bitmap)CanvasImage).GetPixel(x, y));

        public void Rect(int x, int y, int width, int height)
        {
            if (_Fill != null)
            {
                Graphics.FillRectangle(FillColor, new Rectangle(x, y, width, height));
            }

            if (!(_Stroke is null))
            {
                Graphics.DrawRectangle(StrokeColor, new Rectangle(x, y, width, height));
            }
        }

        public void Circle(float x, float y, float radius)
        {
            if (_Fill != null)
            {
                Graphics.FillEllipse(FillColor, new RectangleF(x - radius, y - radius, radius * 2f, radius * 2f));
            }

            if (!(_Stroke is null))
            {
                Graphics.DrawEllipse(StrokeColor, new RectangleF(x - radius, y - radius, radius * 2f, radius * 2f));
            }
        }

        public void BeginShape()
        {
            Vertices = new List<(float, float)>();
        }

        public void Vertex(float x, float y)
        {
            if (Vertices == null) { return; }
            Vertices.Add((x, y));
        }

        public void EndShape(EndShapeType type)
        {
            if (Vertices == null || Vertices.Count < 2) { return; }
            if (type == EndShapeType.Close) { Vertices.Add(Vertices[0]); }
            var points = Vertices.ConvertAll(v => new PointF(v.Item1, v.Item2)).ToArray();

            if (_Fill != null)
            {
                Graphics.FillPolygon(FillColor, points);
            }
            if (_Stroke != null)
            {
                Graphics.DrawLines(StrokeColor, points);
            }
            Vertices = null;
        }

        public void Line(int x1, int y1, int x2, int y2)
        {
            if (_Stroke == null) { return; }
            Graphics.DrawLine(StrokeColor, x1, y1, x2, y2);
        }

        public void Arc(int x, int y, int w, int h, float s, float e)
        {
            if (_Stroke == null) { return; }
            Graphics.DrawArc(StrokeColor, x, y, w, h, PMath.Degrees(s), PMath.Degrees(e));
        }

        public void DrawImage(PSprite sprite, int x, int y, int width, int height)
        {
            if (sprite == null) { return; }
            Graphics.DrawImage(sprite._Image, x, y, width, height);
        }

        public void DrawImage(Canvas canvas, int x, int y, int width, int height)
        {
            Graphics.DrawImage(canvas.CanvasImage, x, y, width, height);
        }

        public void Clear()
        {
            Graphics.Clear(Color.Transparent);
        }
    }
}