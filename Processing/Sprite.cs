using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Processing
{
    public class Sprite : IDisposable
    {
        internal Bitmap _Image;
        internal Graphics Graphics;
        public CanvasArt Art;

        public int Width => _Image.Width;
        public int Height => _Image.Height;

        public string Name;

        public static Sprite FromFilePath(string path)
        {
            //var b = new Bitmap(path);
            //var name = Path.GetFileNameWithoutExtension(path);
            //return new PSprite()
            //{
            //    _Image = b,
            //    Name = name,
            //    Graphics = Graphics.FromImage(b),
            //};

            var b = new Bitmap(path);
            var toReturn = new Sprite(b.Width, b.Height)
            {
                Name = Path.GetFileName(path),
                _Image = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppArgb)
            };
            toReturn.Graphics = Graphics.FromImage(toReturn._Image);
            toReturn.Graphics.DrawImage(b, 0, 0, b.Width, b.Height);
            return toReturn;
        }

        public Sprite(int width, int height)
        {
            _Image = new Bitmap(width, height);
            Name = "New PSprite";
            Graphics = Graphics.FromImage(_Image);
            Art = new CanvasArt(this);
        }

        public Sprite()
        {
            Art = new CanvasArt(this);
        }

        public Sprite(Canvas canvas)
        {
            _Image = (Bitmap)((Bitmap)canvas.CanvasImage).Clone();
            Name = "PSprite From Canvas";
            Graphics = Graphics.FromImage(_Image);
            Art = new CanvasArt(this);
        }

        public void Save()
        {
            var d = new SaveFileDialog()
            {
                Filter = "PNG Image (*.png)|*.png",
                RestoreDirectory = true,
            };

            if (d.ShowDialog() == DialogResult.OK)
            {
                _Image.Save(d.FileName);
            }
            d.Dispose();
        }

        public void Save(string path)
        {
            try
            {
                _Image.Save(path);
            }
            catch
            {
                Console.WriteLine("Error!");
            }
        }

        public void Clear()
        {
            _Image = new Bitmap(_Image.Width, _Image.Height);
            Graphics = Graphics.FromImage(_Image);
        }

        public void Dispose()
        {
            Graphics.Dispose();
            _Image.Dispose();
        }

        public static Sprite FromScreen(int screenIndex)
        {
            var s = Screen.AllScreens[screenIndex];

            var screenshot = new Sprite(s.Bounds.Width, s.Bounds.Height);

            screenshot.Graphics.CopyFromScreen(s.Bounds.X, s.Bounds.Y, 0, 0, 
                s.Bounds.Size, CopyPixelOperation.SourceCopy);

            return screenshot;
        }

        public static Sprite FromCanvas(Canvas c)
        {
            var p = new Sprite(c.Width, c.Height);
            p.Graphics.DrawImage(c.CanvasImage, 0, 0);
            return p;
        }
    }
}