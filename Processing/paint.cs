using System.Drawing;

namespace Processing
{
    public class Paint
    {
        public int R = 0;
        public int G = 0;
        public int B = 0;
        public int A = 0;

        public Paint() : this(0, 0, 0, 255) { }
        public Paint(int rgb) : this(rgb, rgb, rgb, 255) { }
        public Paint(int rgb, int a) : this(rgb, rgb, rgb, a) { }
        public Paint(int r, int g, int b) : this(r, g, b, 255) { }
        public Paint(int r, int g, int b, int a)
        {
            R = (int)PMath.Clamp(r, 0, 255);
            G = (int)PMath.Clamp(g, 0, 255);
            B = (int)PMath.Clamp(b, 0, 255);
            A = (int)PMath.Clamp(a, 0, 255);
        }

        public override bool Equals(object obj) => !(obj is Paint c) ? false : c == this;

        public override int GetHashCode() => R * G * B * A;

        public static Paint FromPixels(byte[] pixels, int b)
            => new Paint(pixels[b], pixels[b + 1], pixels[b + 2], pixels[b + 3]);

        public Color ToColor() => Color.FromArgb(A, R, G, B);
        public static Paint FromColor(Color c) => new Paint(c.R, c.G, c.B, c.A);

        public static Paint Lerp(Paint a, Paint b, float inter)
        {
            inter = PMath.Clamp(inter, 0, 1);
            return new Paint(
                (int)PMath.Lerp(a.R, b.R, inter),
                (int)PMath.Lerp(a.G, b.G, inter),
                (int)PMath.Lerp(a.B, b.B, inter),
                (int)PMath.Lerp(a.A, b.A, inter));
        }

        public static Paint LerpMultiple(Paint[] colors, float colorPercent)
        {
            colorPercent = PMath.Clamp(colorPercent, 0, 0.999999f);
            var index = (int)(colors.Length * colorPercent);
            var nextIndex = index == colors.Length - 1 ? 0 : index + 1;
            var worth = 1f / colors.Length;
            var r = (colorPercent - (worth * index)) / worth;
            return Lerp(colors[index], colors[nextIndex], r);
        }

        public static Paint Transparent => new Paint(255, 255, 255, 0);
        public static Paint White => new Paint(255, 255, 255);
        public static Paint Black => new Paint(0, 0, 0);
        public static Paint Red => new Paint(255, 0, 0);
        public static Paint Green => new Paint(0, 255, 0);
        public static Paint Blue => new Paint(0, 0, 255);
        public static Paint Grey => new Paint(128, 128, 128);
        public static Paint DarkGrey => new Paint(64, 64, 64);
        public static Paint LightGrey => new Paint(192, 192, 192);
        public static Paint Purple => new Paint(128, 0, 128);
        public static Paint Pink => new Paint(255, 20, 147);
        public static Paint Indigo => new Paint(75, 0, 130);
        public static Paint Teal => new Paint(51, 153, 255);
        public static Paint DarkTurquoise => new Paint(0, 40, 80);
        public static Paint Orange => new Paint(255, 127, 0);
        public static Paint Yellow => new Paint(255, 255, 0);
        public static Paint CornflowerBlue => FromColor(Color.CornflowerBlue);

        public static Paint[] Rainbow = new Paint[] 
            { Red, Orange, Yellow, Green, Blue, Indigo, Purple, };

        public static Paint operator /(Paint p, float f) =>
            new Paint((int)(p.R / f), (int)(p.G / f), (int)(p.B / f));

        public static bool operator ==(Paint a, Paint b) =>
            a is null ? false : b is null ? false :
            a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;

        public static bool operator !=(Paint a, Paint b) =>  !(a == b);

        /// <summary>
        /// Distance between colors, ranged between 0 and 1
        /// </summary>
        public static float DistanceSquared(Paint a, Paint b) =>
            ((a.R * b.R) + (a.G * b.G) + (a.B * b.B)) / 195075f;
    }
}