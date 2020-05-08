using System;
using System.Collections.Generic;
using System.Linq;
using Processing;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;

//Guess and be a mazed
namespace Processing_Test
{
    public class MazeGenerator : ProcessingCanvas
    {
        Stack<Point> Stack;
        bool[,] Visited;
        int size = 100;
        Random r;
        PSprite ValidImage;

        bool[,] ValidSpaces;
        int ValidSpaceCount;

        PSprite Rendered;

        List<(Point, Point)> Connections;
        Node[,] Nodes;
        List<Node> NodesList;
        Point Start;
        Point End;

        public MazeGenerator()
        {
            CreateCanvas(1000, 1000, 30);
        }

        public void Setup()
        {
            AddKeyAction("Space", (a) =>
            {
                if (a)
                {
                    var maxDistance = -1;
                    Node start = null;
                    Node end = null;

                    var pathFinder = new PathFinder();
                    var v = Velocity.FromMetersPerSecond(1);

                    var nodesToCheck = new List<Node>();
                    foreach (var n in NodesList)
                    {
                        nodesToCheck.Add(n);
                    }

                    float i = 0;
                    foreach (var n in NodesList)
                    {
                        var mD = -1;
                        Node end2 = null;

                        foreach (var n2 in nodesToCheck)
                        {
                            var p = pathFinder.FindPath(n, n2, v);
                            if (p.Distance.Meters > mD)
                            {
                                mD = (int)p.Distance.Meters;
                                end2 = n2;
                            }
                        }

                        if (mD > maxDistance)
                        {
                            maxDistance = mD;
                            start = n;
                            end = end2;
                        }
                        i++;

                        nodesToCheck.Remove(n);
                        Console.WriteLine("Finding longest path: " + ((i / NodesList.Count) * 100).ToString("00.00") + "% - " + maxDistance);
                    }

                    Start = new Point((int)start.Position.X, (int)start.Position.Y);
                    End = new Point((int)end.Position.X, (int)end.Position.Y);
                }
            });

            SetQuality(RenderQuality.Low);
            Stack = new Stack<Point>();
            Visited = new bool[size, size];
            r = new Random();
            Stack.Push(new Point(50, 50));
            Visited[50, 50] = true;
            Connections = new List<(Point, Point)>();
            ValidImage = PSprite.FromFilePath("Maze\\outline3.png");
            ValidSpaces = new bool[size, size];
            Nodes = new Node[size, size];
            Nodes[50, 50] = new Node(new Position(50, 50));
            NodesList = new List<Node>();

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    if (ValidImage.Art.GetPixel(x, y).A > 0)
                    {
                        ValidSpaces[x, y] = true;
                        ValidSpaceCount++;
                    }
                }
            }
            ValidSpaces[50, 50] = false;
            ValidSpaceCount--;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    if (ValidSpaces[x, y])
                    {
                        var c = 0;
                        if (x < size - 1 && ValidSpaces[x + 1, y])
                        {
                            c++;
                        }
                        if (x > 0 && ValidSpaces[x - 1, y])
                        {
                            c++;
                        }
                        if (y < size - 1 && ValidSpaces[x, y + 1])
                        {
                            c++;
                        }
                        if (y > 0 && ValidSpaces[x, y - 1])
                        {
                            c++;
                        }

                        if (c == 0)
                        {
                            ValidSpaces[x, y] = false;
                            ValidSpaceCount--;
                        }
                    }
                }
            }

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    if (ValidSpaces[x, y])
                    {
                        var n = new Node(new Position(x, y));
                        Nodes[x, y] = n;
                        NodesList.Add(n);
                    }
                }
            }

            while (ValidSpaceCount > 0)
            {
                Tick();
            }

            Console.WriteLine("Rendering Image...");
            Render();
            Console.WriteLine("Generation Complete!");
            Form.FormPictureBox.MouseClick += FormPictureBox_MouseClick;
        }

        private void FormPictureBox_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Start = new Point(e.X / (Width / size), e.Y / (Width / size));
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                End = new Point(e.X / (Width / size), e.Y / (Width / size));
            }
        }

        public void Tick()
        {
            if (ValidSpaceCount == 0)
            {
                return;
            }

            var surrounding = new List<Point>();
            Point last = null;
            try
            {
                last = Stack.Peek();
            }
            catch
            {
                Console.WriteLine("Empty");
                return;
            }

            var p2 = new Point(last.X - 1, last.Y);
            if (last.X > 0 && !Visited[p2.X, p2.Y])
            {
                if (ValidSpaces[p2.X, p2.Y])
                {
                    surrounding.Add(p2);
                }
            }
            p2 = new Point(last.X, last.Y - 1);
            if (last.Y > 0 && !Visited[p2.X, p2.Y])
            {
                if (ValidSpaces[p2.X, p2.Y])
                {
                    surrounding.Add(p2);
                }
            }
            p2 = new Point(last.X + 1, last.Y);
            if (last.X < size - 1 && !Visited[p2.X, p2.Y])
            {
                if (ValidSpaces[p2.X, p2.Y])
                {
                    surrounding.Add(p2);
                }
            }
            p2 = new Point(last.X, last.Y + 1);
            if (last.Y < size - 1 && !Visited[p2.X, p2.Y])
            {
                if (ValidSpaces[p2.X, p2.Y])
                {
                    surrounding.Add(p2);
                }
            }

            if (surrounding.Count == 0)
            {
                Stack.Pop();
                return;
            }

            if (ValidSpaceCount % 50 == 0)
            {
                Console.WriteLine("Cells remaining: " + ValidSpaceCount);
            }

            var next = surrounding[r.Next(0, surrounding.Count)];
            var current = Stack.Peek();
            Nodes[current.X, current.Y].Connect(Nodes[next.X, next.Y], Velocity.FromMetersPerSecond(1));
            Nodes[next.X, next.Y].Connect(Nodes[current.X, current.Y], Velocity.FromMetersPerSecond(1));
            Connections.Add((current, next));
            ValidSpaces[next.X, next.Y] = false;
            ValidSpaceCount--;
            Stack.Push(next);
            Visited[next.X, next.Y] = true;
        }

        public void Draw(float delta)
        {
            Art.Background(PColor.Grey);
            Art.DrawImage(Rendered, 0, 0, Width, Height);

            if (!(Start is null) && 
                !(End is null) &&
                !(Nodes[Start.X, Start.Y] is null) && 
                !(Nodes[End.X, End.Y] is null))
            {
                var pathFinder = new PathFinder();
                var path = pathFinder.FindPath(Nodes[Start.X, Start.Y], Nodes[End.X, End.Y], Velocity.FromMetersPerSecond(1));

                var drawSize = Width / size;
                Art.Stroke(PColor.Pink);
                Art.StrokeWeight(3);
                foreach (var p in path.Edges)
                {
                    Art.Line((int)p.Start.Position.X * drawSize + drawSize / 2, (int)p.Start.Position.Y * drawSize + drawSize / 2,
                        (int)p.End.Position.X * drawSize + drawSize / 2, (int)p.End.Position.Y * drawSize + drawSize / 2);
                }
            }
        }

        public void Render()
        {
            Rendered = new PSprite(Width, Height);

            Rendered.Art.Background(PColor.CornflowerBlue);
            Rendered.Art.NoStroke();

            var drawSize = Width / size;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    if (Visited[x, y])
                    {
                        var color = ValidImage.Art.GetPixel(x, y);
                        color.A = 255;
                        Rendered.Art.Fill(color);
                        Rendered.Art.Rect(x * drawSize, y * drawSize, drawSize + 1, drawSize + 1);
                    }
                }
            }

            Rendered.Art.Stroke(PColor.Black);
            Rendered.Art.StrokeWeight(drawSize / 5);
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    var n = new Point(x, y);
                    if (Visited[n.X, n.Y])
                    {
                        var right = new Point(n.X + 1, n.Y);
                        var down = new Point(n.X, n.Y + 1);
                        var left = new Point(n.X - 1, n.Y);
                        var top = new Point(n.X, n.Y - 1);

                        var rightConnected = false;
                        var downConnected = false;
                        var leftConnected = false;
                        var topConnected = false;

                        foreach (var c in Connections)
                        {
                            if ((c.Item1 == n && c.Item2 == right) || (c.Item1 == right && c.Item2 == n))
                            {
                                rightConnected = true;
                            }
                        }

                        foreach (var c in Connections)
                        {
                            if ((c.Item1 == n && c.Item2 == down) || (c.Item1 == down && c.Item2 == n))
                            {
                                downConnected = true;
                            }
                        }

                        foreach (var c in Connections)
                        {
                            if ((c.Item1 == n && c.Item2 == left) || (c.Item1 == left && c.Item2 == n))
                            {
                                leftConnected = true;
                            }
                        }

                        foreach (var c in Connections)
                        {
                            if ((c.Item1 == n && c.Item2 == top) || (c.Item1 == top && c.Item2 == n))
                            {
                                topConnected = true;
                            }
                        }

                        if (!rightConnected)
                        {
                            Rendered.Art.Line((n.X * drawSize) + drawSize, n.Y * drawSize, (n.X * drawSize) + drawSize, (n.Y * drawSize) + drawSize);
                        }
                        if (!downConnected)
                        {
                            Rendered.Art.Line(n.X * drawSize, (n.Y * drawSize) + drawSize, (n.X * drawSize) + drawSize, (n.Y * drawSize) + drawSize);
                        }
                        if (!leftConnected)
                        {
                            Rendered.Art.Line((n.X * drawSize), n.Y * drawSize, (n.X * drawSize), (n.Y * drawSize) + drawSize);
                        }
                        if (!topConnected)
                        {
                            Rendered.Art.Line(n.X * drawSize, (n.Y * drawSize), (n.X * drawSize) + drawSize, (n.Y * drawSize));
                        }
                    }
                }
            }
        }
    }

    public class Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x; 
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point p)
            {
                return p.X == X && p.Y == Y;
            }
            return false;
        }

        public static bool operator ==(Point p, Point p2)
        {
            return p.X == p2.X && p.Y == p2.Y;
        }

        public static bool operator !=(Point p, Point p2)
        {
            if (p is null) { return p2 is null; }
            if (p2 is null) { return false; }
            return !(p.X == p2.X && p.Y == p2.Y);
        }
    }
}