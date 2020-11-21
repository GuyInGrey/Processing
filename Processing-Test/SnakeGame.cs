using System.Collections.Generic;
using Processing;

namespace Processing_Test
{
    public class SnakeGame : Canvas
    {
        List<Point> Body;
        Direction Direction;
        Direction Next;

        float TimeSinceTick;
        int CellSize => Width / 40;
        float TickTime = 0.25f;
        bool Lost = false;

        public SnakeGame() =>
            CreateCanvas(1000, 1000, 60);

        public override void Setup()
        {
            Body = new List<Point>()
            {
                new Point(20, 20),
                new Point(20, 21),
                new Point(20, 22),
            };
            Direction = Direction.Up;
            Next = Direction.Up;

            AddKeyAction("Up", b => { if (!b) { return; }
                Next = Direction.Up; });
            AddKeyAction("Down", b => { if (!b) { return; }
                Next = Direction.Down; });
            AddKeyAction("Left", b => { if (!b) { return; }
                Next = Direction.Left; });
            AddKeyAction("Right", b => { if (!b) { return; }
                Next = Direction.Right; });
        }

        public override void Draw(float delta)
        {
            if (Lost) { TimeSinceTick = float.MinValue; }
            TimeSinceTick += delta;
            if (TimeSinceTick > TickTime) { Tick(); TimeSinceTick = 0f; }

            Art.Background(Paint.CornflowerBlue);
            Art.Stroke(Paint.Black);
            Art.Fill(Paint.White);
            Body.ForEach(b => Art.Rect(b.X * CellSize, b.Y * CellSize, CellSize, CellSize));
        }

        public void Tick()
        {
            var invalid =
                (Next == Direction.Up && Direction == Direction.Down) ||
                (Next == Direction.Down && Direction == Direction.Up) ||
                (Next == Direction.Left && Direction == Direction.Right) ||
                (Next == Direction.Right && Direction == Direction.Left);

            Direction = invalid ? Direction : Next;

            // Shift Snake
            for (var i = Body.Count - 1; i > 0; i--)
            {
                Body[i] = Body[i - 1];
            }
            Body[0] = Body[0] + (
                Direction == Direction.Up ? Point.Up :
                Direction == Direction.Down ? Point.Down :
                Direction == Direction.Left ? Point.Left :
                Direction == Direction.Right ? Point.Right : Point.Zero
            );

            if ((Direction == Direction.Up || Direction == Direction.Down) && (Next == Direction.Up || Next == Direction.Down))
            { 
                
            }
        }
    }

    public class Point
    {
        public int X;
        public int Y;

        public Point() { }
        public Point(int x, int y) { X = x; Y = y; }

        public static Point operator +(Point a, Point b) =>
            new Point() { X = a.X + b.X, Y = a.Y + b.Y };

        public static Point Up = new Point(0, -1);
        public static Point Down = new Point(0, 1);
        public static Point Right = new Point(1, 0);
        public static Point Left = new Point(-1, 0);
        public static Point Zero = new Point(0, 0);
    }

    public enum Direction
    {
        Up, Right, Down, Left,
    }
}
