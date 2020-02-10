using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processing
{
    public class PColor
    {
        public int R = 0;
        public int G = 0;
        public int B = 0;
        public int A = 0;

        public PColor() : this(0, 0, 0, 255) { }
        public PColor(int rgb) : this(rgb, rgb, rgb, 255) { }
        public PColor(int rgb, int a) : this(rgb, rgb, rgb, a) { }
        public PColor(int r, int g, int b) : this(r, g, b, 255) { }
        public PColor(int r, int g, int b, int a)
        {
            R = (int)PMath.Clamp(r, 0, 255);
            G = (int)PMath.Clamp(g, 0, 255);
            B = (int)PMath.Clamp(b, 0, 255);
            A = (int)PMath.Clamp(a, 0, 255);
        }

        public static PColor FromPixels(byte[] pixels, int b)
            => new PColor(pixels[b], pixels[b + 1], pixels[b + 2], pixels[b + 3]);

        internal Color ToColor() =>
            Color.FromArgb(A, R, G, B);
        internal static PColor FromColor(Color c) =>
            new PColor(c.R, c.G, c.B, c.A);

        public static PColor Lerp(PColor a, PColor b, float inter)
        {
            inter = PMath.Clamp(inter, 0, 1);
            return new PColor(
                (int)PMath.Lerp(a.R, b.R, inter),
                (int)PMath.Lerp(a.G, b.G, inter),
                (int)PMath.Lerp(a.B, b.B, inter));
        }

        public static PColor LerpMultiple(PColor[] colors, float colorPercent)
        {
            colorPercent = PMath.Clamp(colorPercent, 0, 0.999999f);
            var index = (int)(colors.Length * colorPercent);
            var nextIndex = index == colors.Length - 1 ? 0 : index + 1;
            var worth = 1f / colors.Length;
            var r = (colorPercent - (worth * index)) / worth;
            return Lerp(colors[index], colors[nextIndex], r);
        }

        public static PColor Transparent => new PColor(255, 255, 255, 0);
        public static PColor White => new PColor(255, 255, 255);
        public static PColor Black => new PColor(0, 0, 0);
        public static PColor Red => new PColor(255, 0, 0);
        public static PColor Green => new PColor(0, 255, 0);
        public static PColor Blue => new PColor(0, 0, 255);
        public static PColor Grey => new PColor(128, 128, 128);
        public static PColor DarkGrey => new PColor(64, 64, 64);
        public static PColor LightGrey => new PColor(192, 192, 192);
        public static PColor Purple => new PColor(128, 0, 128);
        public static PColor Pink => new PColor(255, 20, 147);
        public static PColor Indigo => new PColor(75, 0, 130);
        public static PColor Teal => new PColor(51, 153, 255);
        public static PColor DarkTurquoise => new PColor(0, 40, 80);
        public static PColor Orange => new PColor(255, 127, 0);
        public static PColor Yellow => new PColor(255, 255, 0);

        public static PColor[] Rainbow = new PColor[] 
            { Red, Orange, Yellow, Green, Blue, Indigo, Purple, };

        public static PColor operator /(PColor p, float f)
        {
            return new PColor((int)(p.R / f), (int)(p.G / f), (int)(p.B / f));
        }
    }
}