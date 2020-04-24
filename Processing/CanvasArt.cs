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
        /// <summary>
        /// The System.Drawing.Graphics object. Don't touch unless you know what you're doing.
        /// </summary>
        public Graphics Graphics => Mode == 0 ? Canvas._Graphics : Sprite.Graphics;
        /// <summary>
        /// The System.Drawing.Image object. Don't touch unless you know what you're doing.
        /// </summary>
        public Image CanvasImage => Mode == 0 ? Canvas.CanvasImage : Mode == 1 ? Sprite._Image : null;
        /// <summary>
        /// The width of the image.
        /// </summary>
        public int Width => CanvasImage.Width;
        /// <summary>
        /// The height of the image.
        /// </summary>
        public int Height => CanvasImage.Height;

        internal Canvas Canvas;
        internal PSprite Sprite;

        internal int Mode = 0;

        internal CanvasArt(Canvas canvas)
        {
            Canvas = canvas;
            Mode = 0;
        }

        internal CanvasArt(PSprite sprite)
        {
            Sprite = sprite;
            Mode = 1;
        }

        private PColor _Stroke = PColor.Black;
        private PColor _Fill = null;
        private Pen StrokeColor => new Pen(_Stroke.ToColor(), _StrokeWeight) { EndCap = LineCap.Round, StartCap = LineCap.Round };
        private SolidBrush FillColor => new SolidBrush(_Fill.ToColor());

        private PFont _Font = null;
        private float _StrokeWeight = 1f;

        internal List<(float, float)> Vertices;

        /// <summary>
        /// Set pixel to color. This method is very slow when setting lots of pixels.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool Set(int x, int y, PColor color)
        {
            if (x < 0 || x >= Width) { return false; }
            if (y < 0 || y >= Height) { return false; }

            ((Bitmap)CanvasImage).SetPixel(x, y, color.ToColor());

            return true;
        }

        /// <summary>
        /// Fill the entire canvas with the specified color.
        /// </summary>
        /// <param name="color"></param>
        public void Background(PColor color)
        {
            var buffer = 10;
            Graphics.FillRectangle(new SolidBrush(color.ToColor()), new Rectangle(-buffer, -buffer, Width + (buffer * 2), Height + (buffer * 2)));
        }

        /// <summary>
        /// Draw text on the screen.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Text(string val, int x, int y)
        {
            if (_Fill == null || _Font == null) { return; }

            Graphics.DrawString(val, _Font.Font, FillColor, new Point(x, y),
                new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
        }

        /// <summary>
        /// Set the stroke color for lines.
        /// </summary>
        /// <param name="p"></param>
        public void Stroke(PColor p) { _Stroke = p; }
        /// <summary>
        /// Remove stroke color.
        /// </summary>
        public void NoStroke() { _Stroke = null; }
        /// <summary>
        /// Set the stroke line width.
        /// </summary>
        /// <param name="width"></param>
        public void StrokeWeight(float width) { _StrokeWeight = width; }

        /// <summary>
        /// Set the fill color for filled shapes.
        /// </summary>
        /// <param name="p"></param>
        public void Fill(PColor p) { _Fill = p; }
        /// <summary>
        /// Remove the fill color.
        /// </summary>
        public void NoFill() { _Fill = null; }

        /// <summary>
        /// Create a PFont object using system fonts.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public PFont CreateFont(string name, float size) =>
            new PFont() { Font = new Font(name, size) };

        /// <summary>
        /// Set the font to the given PFont object. Used for <see cref="Text(string, int, int)"/>
        /// </summary>
        /// <param name="f"></param>
        public void TextFont(PFont f) { _Font = f; }

        /// <summary>
        /// Get all the pixels of the canvas in RGBA format. Fastest way to set lots of pixels.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Set the image pixels to the given pixel array taken from GetPixels. Fastest way to set lots of pixels.
        /// </summary>
        /// <param name="argbValues"></param>
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

        /// <summary>
        /// Get the color of a pixel. Slow for lots of pixels.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public PColor GetPixel(int x, int y) =>
            PColor.FromColor(((Bitmap)CanvasImage).GetPixel(x, y));

        /// <summary>
        /// Draw a rectangle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
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

        /// <summary>
        /// Draw a circle. x and y are center of circle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
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

        /// <summary>
        /// Begin a new vertex shape.
        /// </summary>
        public void BeginShape()
        {
            Vertices = new List<(float, float)>();
        }

        /// <summary>
        /// Sets a corner on the vertext shape.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Vertex(float x, float y)
        {
            if (Vertices == null) { return; }
            Vertices.Add((x, y));
        }

        /// <summary>
        /// End the vertex shape.
        /// </summary>
        /// <param name="type"></param>
        public void EndShape(EndShapeType type)
        {
            if (Vertices == null || Vertices.Count < 2) { return; }
            if (type == EndShapeType.Close) { Vertices.Add(Vertices[0]); }
            var points = Vertices.ConvertAll(v => new PointF(v.Item1, v.Item2)).ToArray();

            if (!(_Fill is null))
            {
                Graphics.FillPolygon(FillColor, points);
            }
            if (!(_Stroke is null))
            {
                Graphics.DrawLines(StrokeColor, points);
            }
            Vertices = null;
        }

        /// <summary>
        /// Draw a line between two points.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void Line(int x1, int y1, int x2, int y2)
        {
            if (_Stroke == null) { return; }
            Graphics.DrawLine(StrokeColor, x1, y1, x2, y2);
        }

        /// <summary>
        /// Draw an arc.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="e"></param>
        public void Arc(int x, int y, int w, int h, float s, float e)
        {
            if (_Stroke == null) { return; }
            Graphics.DrawArc(StrokeColor, x, y, w, h, PMath.Degrees(s), PMath.Degrees(e));
        }

        /// <summary>
        /// Draw an image to the canvas.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void DrawImage(PSprite sprite, int x, int y, int width, int height)
        {
            if (sprite == null) { return; }
            Graphics.DrawImage(sprite._Image, x, y, width, height);
        }

        /// <summary>
        /// Draw a different canvas to this canvas. Technically you can provide the same canvas but that'll just make a weird looping effect.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void DrawImage(Canvas canvas, int x, int y, int width, int height)
        {
            Graphics.DrawImage(canvas.CanvasImage, x, y, width, height);
        }

        /// <summary>
        /// Completely clear the canvas of color.
        /// </summary>
        public void Clear()
        {
            Graphics.Clear(Color.Transparent);
        }
    }
}