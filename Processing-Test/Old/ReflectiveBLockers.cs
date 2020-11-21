using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Processing;

namespace Processing_Test
{
    public class ReflectiveBlockers : ProcessingCanvas
    {
        Point2D A = new Point2D(50, 300);
        Point2D B = new Point2D(900, 800);

        bool leftDown;
        bool rightDown;

        public ReflectiveBlockers() => CreateCanvas(1000, 1000, 30);

        public void Setup()
        {
            Form.FormPictureBox.MouseDown += (a, b) =>
            {
                if (b.Button == MouseButtons.Left) { leftDown = true; }
                if (b.Button == MouseButtons.Right) { rightDown = true; }
            };

            Form.FormPictureBox.MouseUp += (a, b) =>
            {
                if (b.Button == MouseButtons.Left) { leftDown = false; }
                if (b.Button == MouseButtons.Right) { rightDown = false; }
            };

            Form.FormPictureBox.MouseMove += (a, b) =>
            {
                if (leftDown) { A = new Point2D(b.X, b.Y); }
                if (rightDown) { B = new Point2D(b.X, b.Y); }
            };
        }

        public void Draw(float delta)
        {
            Title(FrameRateCurrent + " - " + TotalFrameCount);
            var p = new List<Point2D>();

            p.Add(new Point2D((A.X + B.X) / 2, (A.Y + B.Y) / 2));
            p.Add(new Point2D(1000 - p[0].X, p[0].Y));
            p.Add(new Point2D(p[1].X + (p[1].X - p[0].X), p[0].Y));
            p.Add(new Point2D(p[0].X - (p[1].X - p[0].X), p[0].Y));

            p.Add(new Point2D(p[0].X, 1000 - p[0].Y));
            p.Add(new Point2D(p[0].X, p[4].Y + (p[4].Y - p[0].Y)));
            p.Add(new Point2D(p[0].X, p[0].Y - (p[4].Y - p[0].Y)));

            p.Add(new Point2D(p[3].X, p[4].Y));
            p.Add(new Point2D(p[3].X, p[5].Y));
            p.Add(new Point2D(p[3].X, p[6].Y));

            p.Add(new Point2D(p[1].X, p[4].Y));
            p.Add(new Point2D(p[1].X, p[5].Y));
            p.Add(new Point2D(p[1].X, p[6].Y));

            p.Add(new Point2D(p[2].X, p[4].Y));
            p.Add(new Point2D(p[2].X, p[5].Y));
            p.Add(new Point2D(p[2].X, p[6].Y));

            Art.Background(PColor.Black);
            Art.NoStroke();
            Art.Fill(PColor.White);
            Art.TextFont(Art.CreateFont("Arial", 10f));
            var i = 0;
            foreach (var p2 in p)
            {
                Art.Circle(p2.X, p2.Y, 10);
                Art.Text(i.ToString(), (int)(p2.X + 20), (int)p2.Y);
                i++;
            }

            Art.Fill(PColor.Orange);
            Art.Circle(A.X, A.Y, 10);

            Art.Fill(PColor.Indigo);
            Art.Circle(B.X, B.Y, 10);
        }
    }
}
