using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Processing;

namespace Processing_Test
{
    public class ImageGenerator : ProcessingCanvas
    {
        PSprite o; // Original
        PSprite e; // Edge Detected

        int w;
        int h;

        public ImageGenerator()
        {
            CreateCanvas(1280, 720, 10000);
        }

        public void Setup()
        {
            o = PSprite.FromFilePath("edge.png");
            w = o.Width;
            h = o.Height;
            e = new PSprite(w, h);

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    var edge = false;

                    var d1 = 0f;
                    var d2 = 0f;
                    var d3 = 0f;

                    var c1 = o.Art.GetPixel(x, y);

                    if (x < Width - 1)
                    {
                        d1 = PColor.DistanceSquared(c1, o.Art.GetPixel(x + 1, y));

                        if (y < Height - 1)
                        {
                            d2 = PColor.DistanceSquared(c1, o.Art.GetPixel(x + 1, y + 1));
                        }
                    }

                    if (y < Height - 1)
                    {
                        d3 = PColor.DistanceSquared(c1, o.Art.GetPixel(x, y + 1));
                    }

                    var care = 0.7f;
                    edge = (d1 > care || d2 > care || d3 > care);
                    e.Art.Set(x, y, PColor.Lerp(PColor.Black,PColor.White, (d3)));
                }
            }
        }

        public void Draw(float delta)
        {
            Art.DrawImage(e, 0, 0, Width, Height);
        }
    }
}