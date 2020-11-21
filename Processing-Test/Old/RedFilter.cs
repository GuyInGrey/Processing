using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Processing;

namespace Processing_Test
{
    public class RedFilter : ProcessingCanvas
    {
        string[] Images;
        int i;

        public RedFilter()
        {
            CreateCanvas(1920, 1080, 60);
        }

        public void Setup()
        {
            Images = Directory.GetFiles(@"LargestStar\InImages");
        }

        public void Draw(float delta)
        {
            var spritePath = Images[i];
            var sprite = PSprite.FromFilePath(spritePath);
            var pixels = sprite.Art.GetPixels();
            //for (var i = 0; i < Width * Height; i++)
            //{
            //    var index = (i * 4) + 2;
            //    pixels[index] = 0;
            //}
            var max = Width * Height * 4;
            for (var index = 2; index < max; index += 4)
            {
                pixels[index] = 0;
            }
            sprite.Art.SetPixels(pixels);

            sprite.Save($@"LargestStar\OutImages\{i:000000}.png");
            Art.DrawImage(sprite, 0, 0, Width, Height);

            i++;
        }
    }
}
