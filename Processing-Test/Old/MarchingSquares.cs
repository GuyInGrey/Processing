using System;
using Processing;

namespace Processing_Test
{
    public class MarchingSquares : ProcessingCanvas
    {
        float[,] Map;
        int Rez = 10;
        int ArrWidth;
        int ArrHeight;
        Random r;
        FastNoise noise;
        float Thresh = 0.5f;
        float z = 0.5f;

        public MarchingSquares()
        {
            r = new Random();
            noise = new FastNoise();
            CreateCanvas(1000, 1000, 15);
        }

        public void Setup()
        {
            ArrWidth = (Width / Rez) + 1;
            ArrHeight = (Height / Rez) + 1;
            Map = new float[ArrWidth, ArrHeight];

            for (var y = 0; y < ArrHeight; y++)
            {
                for (var x = 0; x < ArrWidth; x++)
                {
                    //Map[x, y] = (float)r.NextDouble();
                    Map[x, y] = PMath.Clamp(noise.GetPerlin(x * PMath.PI, y * PMath.PI, z) + 0.5f, 0, 1);
                }
            }

            AddKeyAction("W", (d) =>
            {
                if (!d) { return; }
                Thresh *= 1.05f;
                Thresh = PMath.Clamp(Thresh, 0, 1);
            });
            AddKeyAction("S", (d) =>
            {
                if (!d) { return; }
                Thresh *= 0.95f;
                Thresh = PMath.Clamp(Thresh, 0, 1);
            });
        }

        public void Draw(float delta)
        {
            z += delta * 50;

            for (var y = 0; y < ArrHeight; y++)
            {
                for (var x = 0; x < ArrWidth; x++)
                {
                    //Map[x, y] = (float)r.NextDouble();
                    Map[x, y] = PMath.Clamp(noise.GetPerlin(x * PMath.PI, y * PMath.PI, z) + 0.5f, 0, 1);
                }
            }

            Title(Thresh);
            Art.Background(PColor.Grey);
            Art.NoStroke();

            for (var y = 0; y < ArrHeight; y++)
            {
                for (var x = 0; x < ArrWidth; x++)
                {
                    Art.Fill(new PColor((int)(Map[x, y] * 200)));
                    Art.Rect(x * Rez, y * Rez, Rez + 1, Rez + 1);
                    //Art.Circle(x * Rez, y * Rez, Rez * 0.55f);
                }
            }

            Art.NoFill();
            Art.Stroke(PColor.Black);
            Art.StrokeWeight(2);

            for (var y = 0f; y < ArrHeight - 1; y++)
            {
                for (var x = 0f; x < ArrWidth - 1; x++)
                {
                    var a = ((x * Rez) + (Rez / 2f), (y * Rez));
                    var b = ((x * Rez) + Rez, (y * Rez) + (Rez / 2f));
                    var c = ((x * Rez) + (Rez / 2f), (y * Rez) + Rez);
                    var d = ((x * Rez), (y * Rez) + (Rez / 2f));



                    var which = 0;
                    if (Map[(int)x, (int)y] > Thresh) { which += 8; }
                    if (Map[(int)x + 1, (int)y] > Thresh) { which += 4; }
                    if (Map[(int)x + 1, (int)y + 1] > Thresh) { which += 2; }
                    if (Map[(int)x, (int)y + 1] > Thresh) { which += 1; }

                    if (which == 4 || which == 10 || which == 11) 
                    { Line(a, b); }
                    if (which == 6 || which == 9)
                    { Line(a, c); }
                    if (which == 5 || which == 7 || which == 8)
                    { Line(a, d); }
                    if (which == 2 || which == 5 || which == 13)
                    { Line(b, c); }
                    if (which == 3 || which == 12)
                    { Line(b, d); }
                    if (which == 1 || which == 10 || which == 14)
                    { Line(c, d); }

                    void Line((float, float) one, (float, float) two)
                    {
                        Art.Line(one.Item1, one.Item2, two.Item1, two.Item2);
                    }
                }
            }
        }
    }
}