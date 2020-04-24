using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Processing;

namespace Processing_Test
{
    public class Epilepsy : ProcessingCanvas
    {
        int imagesPerFrame = 4;
        int imageCount = 0;
        int completed = 0;

        public Epilepsy()
        {
            CreateCanvas(1900, 800, 30);
        }

        public void Setup()
        {
            var i = 1;
            var name = "image-" + i.ToString("00000") + ".png";
            while (File.Exists(@"E\" + name))
            {
                imageCount++;
                i++;
                name = "image-" + i.ToString("00000") + ".png";
            }

            Art.DrawImage(Convert(PSprite.FromFilePath("b.png")), 0, 0, Width, Height);
        }

        public void Draw(float delta)
        {
            PSprite last = null;

            Parallel.For(completed, completed + imagesPerFrame + 1, i =>
            {
                if (i < imageCount)
                {
                    var input = PSprite.FromFilePath(@"E\image-" + (i + 1).ToString("00000") + ".png");
                    input = Convert(input);
                    input.Save(@"E\out-" + i.ToString("00000") + ".png");
                    last = input;
                }
                else
                {
                    Console.WriteLine("A");
                }
            });

            if (last != null)
            {
                Art.DrawImage(last, 0, 0, Width, Height);
            }

            completed += imagesPerFrame;
        }

        public PSprite Convert(PSprite i2)
        {
            var pixels = i2.Art.GetPixels();

            //double newValue = 0;
            //var c = (100.0 + -15) / 100.0;

            //c *= c;

            //for (int i = 0; i < pixels.Length; i++)
            //{
            //    newValue = pixels[i];

            //    newValue /= 255.0;
            //    newValue -= 0.5;
            //    newValue *= c;
            //    newValue += 0.5;
            //    newValue *= 255;

            //    if (newValue < 0)
            //        newValue = 0;
            //    if (newValue > 255)
            //        newValue = 255;

            //    pixels[i] = (byte)newValue;
            //}

            for (var y = 0; y < i2.Height; y++)
            {
                for (var x = 0; x < i2.Width; x++)
                {
                    var index = (x + (y * i2.Width)) * 4;
                    var co = new PColor();
                    co.A = 255;
                    co.B = pixels[index];
                    co.G = pixels[index + 1];
                    co.R = pixels[index + 2];
                    var c2 = Convert(co);
                    pixels[index] = (byte)c2.B;
                    pixels[index + 1] = (byte)c2.G;
                    pixels[index + 2] = (byte)c2.R;
                    pixels[index + 3] = 255;

                    //i.Art.Set(x, y, Convert(i.Art.GetPixel(x, y)));
                }
            }

            i2.Art.SetPixels(pixels);

            return i2;
        }

        public PColor Convert(PColor i)
        {
            i.G -= (int)(i.R * 0.67f);
            i.G = (int)PMath.Clamp(i.G, 0, 255);

            i.B -= (int)(i.R * 0.33f);
            i.B = (int)PMath.Clamp(i.B, 0, 255);

            i.R = 0;

            RgbToHls(i.R, i.G, i.B, out var h, out var l, out var s);

            //h = PMath.Clamp((float)h, 70, 300);
            //h = PMath.Map((float)h, 0, 360, 20, 280);
            l = PMath.Clamp((float)l, 0f, 0.3f);
            //l = PMath.Map((float)l, 0f, 0.3f, 0f, 1f);

            HlsToRgb(h, l, s, out i.R, out i.G, out i.B);

            return new PColor((int)PMath.Map(i.R, 0, 255, 0, i.G + i.B), i.G, i.B);
        }

        public static void RgbToHls(int r, int g, int b,
    out double h, out double l, out double s)
        {
            // Convert RGB to a 0.0 to 1.0 range.
            double double_r = r / 255.0;
            double double_g = g / 255.0;
            double double_b = b / 255.0;

            // Get the maximum and minimum RGB components.
            double max = double_r;
            if (max < double_g) max = double_g;
            if (max < double_b) max = double_b;

            double min = double_r;
            if (min > double_g) min = double_g;
            if (min > double_b) min = double_b;

            double diff = max - min;
            l = (max + min) / 2;
            if (Math.Abs(diff) < 0.00001)
            {
                s = 0;
                h = 0;  // H is really undefined.
            }
            else
            {
                if (l <= 0.5) s = diff / (max + min);
                else s = diff / (2 - max - min);

                double r_dist = (max - double_r) / diff;
                double g_dist = (max - double_g) / diff;
                double b_dist = (max - double_b) / diff;

                if (double_r == max) h = b_dist - g_dist;
                else if (double_g == max) h = 2 + r_dist - b_dist;
                else h = 4 + g_dist - r_dist;

                h = h * 60;
                if (h < 0) h += 360;
            }
        }

        // Convert an HLS value into an RGB value.
        public static void HlsToRgb(double h, double l, double s,
            out int r, out int g, out int b)
        {
            double p2;
            if (l <= 0.5) p2 = l * (1 + s);
            else p2 = l + s - l * s;

            double p1 = 2 * l - p2;
            double double_r, double_g, double_b;
            if (s == 0)
            {
                double_r = l;
                double_g = l;
                double_b = l;
            }
            else
            {
                double_r = QqhToRgb(p1, p2, h + 120);
                double_g = QqhToRgb(p1, p2, h);
                double_b = QqhToRgb(p1, p2, h - 120);
            }

            // Convert RGB to the 0 to 255 range.
            r = (int)(double_r * 255.0);
            g = (int)(double_g * 255.0);
            b = (int)(double_b * 255.0);
        }

        private static double QqhToRgb(double q1, double q2, double hue)
        {
            if (hue > 360) hue -= 360;
            else if (hue < 0) hue += 360;

            if (hue < 60) return q1 + (q2 - q1) * hue / 60;
            if (hue < 180) return q2;
            if (hue < 240) return q1 + (q2 - q1) * (240 - hue) / 60;
            return q1;
        }
    }
}
