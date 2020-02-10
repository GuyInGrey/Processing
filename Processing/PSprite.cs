using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Processing
{
    public class PSprite
    {
        internal Bitmap _Image;
        internal Graphics Graphics;
        public CanvasArt Art;

        public int Width => _Image.Width;
        public int Height => _Image.Height;

        public string Name;

        public static PSprite FromFilePath(string path)
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
            var toReturn = new PSprite(b.Width, b.Height);
            toReturn.Name = Path.GetFileNameWithoutExtension(path);
            toReturn._Image = new Bitmap(b.Width, b.Height);
            toReturn.Graphics = Graphics.FromImage(toReturn._Image);
            toReturn.Graphics.DrawImage(b, 0, 0, b.Width, b.Height);
            return toReturn;
        }

        public PSprite(int width, int height)
        {
            _Image = new Bitmap(width, height);
            Name = "New PSprite";
            Graphics = Graphics.FromImage(_Image);
            Art = new CanvasArt(this);
        }

        internal PSprite()
        {
            Art = new CanvasArt(this);
        }

        public PSprite(Canvas canvas)
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

        public static PSprite FromScreen(int screenIndex)
        {
            var s = Screen.AllScreens[screenIndex];

            var screenshot = new PSprite(s.Bounds.Width, s.Bounds.Height);

            screenshot.Graphics.CopyFromScreen(s.Bounds.X, s.Bounds.Y, 0, 0, 
                s.Bounds.Size, CopyPixelOperation.SourceCopy);

            return screenshot;
        }
    }
}