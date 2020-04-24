using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Processing;

namespace Physics
{
    public class _10Print : ProcessingCanvas
    {
        public _10Print()
        {
            CreateCanvas(1000, 1000, 120);
        }

        int x = 0;
        int y = 0;
        int spacing = 20;

        public void Setup()
        {
            Art.Background(PColor.FromColor(Color.CornflowerBlue));
        }

        public void Draw(float delta)
        {
            var distance = PMath.Sqrt(x * x + y * y);
            var maxDistance = PMath.Sqrt(Width * Width + Height * Height);
            var percent = PMath.Map(distance, 0, maxDistance, 0, 1);
            Title(percent);

            //Art.Stroke(PColor.Lerp(PColor.White, PColor.Black, percent));

            if (PMath.Random(1) > percent)
            {
                Art.Line(x, y, x + spacing, y + spacing);
            }
            else
            {
                Art.Line(x, y + spacing, x + spacing, y);
            }

            x += spacing;
            if (x > Width) { x = 0; y += spacing; }
        }
    }
}
