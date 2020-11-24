using System;
using System.Collections.Generic;
using System.Linq;
using Processing;

namespace Processing_Test
{
    public class SnakeGame : Canvas
    {
        List<Point> Body;
        Direction Direction;
        Direction Next;
        Point Food;
        Random Random;

        float TimeSinceTick;
        int CellCount = 40;
        int CellSize => Width / CellCount;
        float TickTime = 0.1f;
        bool Lost = false;
        float PulseTime;

        public SnakeGame() => CreateCanvas(1000, 1000, 60);

        public override void Setup()
        {
            Random = new Random();
            Body = new List<Point>()
            {
                new Point(20, 20),
                new Point(20, 21),
                new Point(20, 22),
            };
            Direction = Direction.Up;
            Next = Direction.Up;

            GenFood();

            AddKeyAction("Up", b => { if (!b) { return; }
                Next = Direction.Up; });
            AddKeyAction("Down", b => { if (!b) { return; }
                Next = Direction.Down; });
            AddKeyAction("Left", b => { if (!b) { return; }
                Next = Direction.Left; });
            AddKeyAction("Right", b => { if (!b) { return; }
                Next = Direction.Right; });

            Art.TextFont("Arial", 30f);
        }

        public override void Draw(float delta)
        {
            Title("FPS: " + Timing.ActualFramesPerSecond);
            if (Lost) { TimeSinceTick = float.MinValue; }
            TimeSinceTick += delta;
            if (TimeSinceTick > TickTime) { Tick(); TimeSinceTick = 0f; }

            PulseTime += delta;

            Art.Background(Paint.CornflowerBlue);
            Art.NoStroke();

            Art.Fill(Paint.White);
            Body.ForEach(b => Art.Rect(b.X * CellSize, b.Y * CellSize, CellSize + 1, CellSize + 1));

            DrawSnakeEdges();

            Art.Fill(Paint.Red);
            Art.Rect(Food.X * CellSize, Food.Y * CellSize, CellSize, CellSize);

            Art.Stroke(Paint.Black);
            Art.StrokeWeight(1f);

            if (Lost)
            {
                Art.Fill(Paint.Red);
                Art.Text("You've lost!", Width / 2, Height / 2);
            }

            Art.Fill(Paint.LerpMultiple(new[] { Paint.Black, Paint.White, Paint.Black }, ((PulseTime * Body.Count) / 5) % 1));
            Art.Text(Body.Count.ToString(), Width / 2, 15);
        }

        public void DrawSnakeEdges()
        {
            for (var i = 0; i < Body.Count; i++)
            {
                var b = Body[i];
                var conns = new List<Point>();
                if (i > 0) { conns.Add(Body[i-1]); }
                if (i < Body.Count - 1) { conns.Add(Body[i + 1]); }

                Direction connections = 0;
                conns = conns.ConvertAll(c => c - b).ToList();
                conns.ForEach(c =>
                {
                    if (c == Point.Left) { connections |= Direction.Left; }
                    if (c == Point.Right) { connections |= Direction.Right; }
                    if (c == Point.Up) { connections |= Direction.Up; }
                    if (c == Point.Down) { connections |= Direction.Down; }
                });

                connections = ~connections;

                var bX = b.X * CellSize;
                var bY = b.Y * CellSize;

                Art.Stroke(Paint.Black);
                Art.StrokeWeight(3f);

                if ((connections & Direction.Up) == Direction.Up)  
                { Art.Line(bX, bY, bX + CellSize, bY); }
                if ((connections & Direction.Down) == Direction.Down)
                { Art.Line(bX, bY + CellSize, bX + CellSize, bY + CellSize); }
                if ((connections & Direction.Left) == Direction.Left)
                { Art.Line(bX, bY, bX, bY + CellSize); }
                if ((connections & Direction.Right) == Direction.Right)
                { Art.Line(bX + CellSize, bY, bX + CellSize, bY + CellSize); }
            }
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
            var oldTail = Body[Body.Count - 1];
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

            for (var i = 0; i < Body.Count; i++)
            {
                for (var j = 0; j < Body.Count; j++)
                {
                    // Two snake parts intersect
                    if (j != i && Body[i] == Body[j])
                    {
                        Lost = true;
                    }
                }
                // Out of bounds
                if (Body[i].X < 0 || Body[i].Y < 0 || Body[i].X >= CellCount || Body[i].Y >= CellCount)
                {
                    Lost = true;
                }
            }

            // EAT
            if (Body[0] == Food)
            {
                Body.Add(oldTail);
                GenFood();
            }
        }

        public void GenFood()
        {
            var spaces = new List<Point>();
            for (var y = 0; y < CellCount; y++)
            {
                for (var x = 0; x < CellCount; x++)
                {
                    spaces.Add(new Point(x, y));
                }
            }

            spaces.RemoveAll(p => Body.Contains(p));
            Food = spaces[Random.Next(0, spaces.Count)];
        }
    }

    public class Point
    {
        public int X;
        public int Y;

        public Point() { }
        public Point(int x, int y) { X = x; Y = y; }

        public override bool Equals(object obj) =>
            obj is Point p ? p == this : false;

        public override int GetHashCode() => X * Y;

        public static Point operator +(Point a, Point b) =>
            new Point() { X = a.X + b.X, Y = a.Y + b.Y };

        public static Point operator -(Point a, Point b) =>
            new Point() { X = a.X - b.X, Y = a.Y - b.Y };

        public static bool operator ==(Point a, Point b) =>
            a is null ? false : b is null ? false : a.X == b.X && a.Y == b.Y;

        public static bool operator !=(Point a, Point b) => !(a == b);

        public static Point Up = new Point(0, -1);
        public static Point Down = new Point(0, 1);
        public static Point Right = new Point(1, 0);
        public static Point Left = new Point(-1, 0);
        public static Point Zero = new Point(0, 0);
    }

    public enum Direction : ushort
    {
        Up = 1,
        Right = 2, 
        Down = 4,
        Left = 8,
    }
}
