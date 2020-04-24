using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Processing;

namespace Processing_Test
{
    public class Recursion : ProcessingCanvas
    {
        int maxDepth = 0;

        public Recursion()
        {
            CreateCanvas(1000, 1000, 30);
        }

        public void Setup()
        {

        }

        float timePassed = 0;
        public void Draw(float delta)
        {
            Art.Background(PColor.FromColor(Color.CornflowerBlue));
            Art.NoStroke();
            Art.Fill(PColor.Blue);

            Divide(0, Height, Width, 0, maxDepth);
            //DrawCircle(Width / 2, Height / 2, Width / 2, 0);

            timePassed += delta;
            if (timePassed > 1f)
            {
                timePassed = 0;
                maxDepth++;
            }
        }

        public void DrawCircle(float x, float y, float d, int depth)
        {
            if (depth < maxDepth)
            {
                //Art.Stroke(d < 5 ? PColor.Black : PColor.FromColor(Color.CornflowerBlue));
                if (depth == maxDepth - 1)
                {
                    Art.Circle(x, y, d / 2);
                }
                if (d > 1)
                {
                    DrawCircle(x + d * 0.5f, y, d * 0.5f, depth + 1);
                    DrawCircle(x - d * 0.5f, y, d * 0.5f, depth + 1);
                    DrawCircle(x, y - d * 0.5f, d * 0.5f, depth + 1);
                    //DrawCircle(x, y + d * 0.5f, d * 0.5f, depth + 1);
                }
            }
        }

        public void Divide(float x, float y, float l, float depth, float maxDepth)
        {
            if (depth == maxDepth)
            {
                DrawTriangle(x, y, l);
                return;
            }

            Divide(x, y, l / 2f, depth + 1, maxDepth);
            Divide(x + l / 2f, y, l / 2f, depth + 1, maxDepth);
            Divide(x + l / 4, y - PMath.Sin(PMath.PI / 3f) * l / 2f, l / 2f, depth + 1, maxDepth);
        }

        public void DrawTriangle(float x, float y, float l)
        {
            Art.BeginShape();
            Art.Vertex(x, y);
            Art.Vertex(x + (l / 2f), y - PMath.Sin(PMath.PI / 3f) * l);
            Art.Vertex(x + l, y);
            Art.EndShape(EndShapeType.Open);
        }
    }
}
