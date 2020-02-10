using System.Collections.Generic;
using Processing;

namespace Processing_Test
{
    class Bubba : ProcessingCanvas
    {
        List<Drop> droplets = new List<Drop>();
        int dropSec = 60;
        int growthRate = 4;
        PSprite rainbow;

        int drawMode = 0;
        int maxDrawMode = 2;

        public Bubba()
        {
            CreateCanvas(1000, 1000, 60);
        }

        public void Setup()
        {
            rainbow = new PSprite(Width, Height);

            for (var x = 0; x < Width; x++)
            {
                var colorPercent = x / (float)Width;
                var current = PColor.LerpMultiple(PColor.Rainbow, colorPercent);

                rainbow.Art.StrokeWeight(1);
                rainbow.Art.NoFill();
                rainbow.Art.Stroke(current);
                rainbow.Art.Line(x, 0, x, Height);
            }

            AddKeyAction("Q", b =>
            {
                if (!b) { drawMode++; }
                if (drawMode > maxDrawMode) { drawMode = 0; }
            });
        }

        public void Draw(float delta)
        {
            SetQuality(RenderQuality.Low);
            Title(FrameRateCurrent + "/" + FrameRateTarget);
            Art.DrawImage(rainbow, 0, 0, Width, Height);
            Art.StrokeWeight(10);
            var colorPercent = (TotalFrameCount % 480f) / 480f;
            PColor current;

            for (var i = 0; i < droplets.Count && dropSec != 0; i++)
            {
                var drop = droplets[i];

                if (drawMode == 0)
                {
                    Art.Circle(drop.x, drop.y, drop.diameter / 2);
                }
                if (drawMode == 1)
                {
                    switch (drop.shape)
                    {
                        case 0:
                            current = PColor.LerpMultiple(PColor.Rainbow, colorPercent);
                            Art.Fill(current);
                            Art.Circle(drop.x, drop.y, drop.diameter / 2);
                            break;
                        case 1:
                            current = PColor.LerpMultiple(PColor.Rainbow, 1 - colorPercent);
                            Art.Fill(current);
                            Art.Rect(drop.x - (drop.diameter / 2), drop.y - (drop.diameter / 2), drop.diameter, drop.diameter);
                            break;
                    }
                }

                if (drop.diameter < drop.maxDiameter)
                {
                    drop.diameter += growthRate;
                    drop.diameter = drop.diameter > drop.maxDiameter ? drop.maxDiameter : drop.diameter;
                }

                if (drop.diameter == drop.maxDiameter)
                {
                    droplets.RemoveAt(i);
                    i--;
                }
            }

            if (TotalFrameCount % (FrameRateTarget / (float)dropSec) == 0)
            {
                droplets.Add(new Drop(Width, Height));
            }

            current = PColor.LerpMultiple(PColor.Rainbow, colorPercent);

            colorPercent = (TotalFrameCount % 480f) / 480f;
            var x = (int)PMath.Map(colorPercent, 0, 1, 0, Width);
            Art.Fill(current);
            Art.Line(x, 0, x, Height);
            Art.Line(Width - x, 0, Width - x, Height);
            var boxHeight = 50;
            Art.Rect(0, (Height / 2) - (boxHeight / 2), Width, boxHeight);
        }
    }

    class Drop
    {
        public int diameter;
        public int maxDiameter;
        public int x, y;
        public int frameDeletion = -1;
        public int shape = 0;

        public Drop(int width, int height)
        {
            x = (int)PMath.Random(width);
            y = (int)PMath.Random(height);
            maxDiameter = (int)PMath.Random(0, 300);
            shape = (int)PMath.Random(2);
        }

        public bool Intersects(Drop b)
        {
            return Circle(x, y, b.x, b.y, maxDiameter / 3, b.maxDiameter / 3) == 0;
        }

        public static int Circle(int x1, int y1, int x2, int y2, int r1, int r2)
        {
            var distSq = (x1 - x2) * (x1 - x2) +
                         (y1 - y2) * (y1 - y2);
            var radSumSq = (r1 + r2) * (r1 + r2);

            return distSq == radSumSq ? 1 : distSq > radSumSq ? -1 : 0;
        }
    }
}