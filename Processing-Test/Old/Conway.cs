using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Processing;

namespace Processing_Test
{
    public class Conway : ProcessingCanvas
    {
        public Conway()
        {
            CreateCanvas(1000, 1000, 60);
        }

        bool[,] cells;
        int w => cells.GetLength(0);
        int h => cells.GetLength(1);

        int cellWidth => Width / w;
        int cellHeight => Height / h;

        PColor off = PColor.Black;
        PColor on = PColor.White;

        public void Setup()
        {
            cells = new bool[50, 50];

            AddKeyAction("Space", i =>
            {
                if (i) { Generation();  }
            });

            Form.FormPictureBox.MouseClick += FormPictureBox_MouseClick; ;
        }

        private void FormPictureBox_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            cells[e.X / cellWidth, e.Y / cellHeight] ^= true;
        }

        public void Draw(float delta)
        {
            Title(FrameRateCurrent);
            Art.Background(off);
            Art.NoStroke();
            Art.Fill(on);
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    if (cells[x, y])
                    {
                        Art.Rect(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                    }
                }
            }
        }

        public void Generation()
        {
            var newCells = new bool[w, h];
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    var n = 0;
                    for (var y2 = -1; y2 <= 1; y2++)
                    {
                        for (var x2 = -1; x2 <= 1; x2++)
                        {
                            if (x2 == 0 && y2 == 0) { continue; }
                            var nx = x + x2;
                            var ny = y + y2;
                            if (ny < 0 || ny >= h || nx < 0 || nx >= w) { continue; }
                            n = cells[x2 + x, y2 + y] ? n + 1 : n;
                        }
                    }

                    newCells[x, y] =
                        cells[x, y] ? (n == 2 || n == 3) :
                        n == 3;
                }
            }

            cells = newCells;
        }
    }
}
