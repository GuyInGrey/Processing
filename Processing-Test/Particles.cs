using System;
using System.Collections.Generic;
using System.Linq;
using Processing;

namespace Processing_Test
{
    public class ParticleSimulator : Canvas
    {
        Random Random;
        //float[,] Particles;
        List<Particle> Particles;
        bool MouseIsDown;
        Vector2 MousePosition;

        List<Sprite> PreviousFrames = new List<Sprite>();
        int TailSize = 1;

        public ParticleSimulator()
        {
            CreateCanvas(1000, 1000, 60);
        }

        public override void Setup()
        {
            Random = new Random();

            var speedRange = 50;

            Particles = new List<Particle>();
            for (var i = 0; i < 1000; i++)
            {
                Particles.Add(new Particle()
                {
                    Position = new Vector2(Random.Next(0, Width - 1), Random.Next(0, Height - 1)),
                    Velocity = new Vector2(Random.Next(-speedRange, speedRange), Random.Next(-speedRange, speedRange)),
                });
            }

            Form.FormPictureBox.MouseDown += (a, b) => MouseIsDown = true;
            Form.FormPictureBox.MouseUp += (a, b) => MouseIsDown = false;
            Form.FormPictureBox.MouseMove += (a, b) => MousePosition = new Vector2(b.X, b.Y);
        }

        public override void Draw(float delta)
        {
            //Title($"FPS: {Timing.ActualFramesPerSecond}/{Timing.TargetFramesPerSecond}");
            Art.Background(Paint.Black);

            var mag = 0f;
            Particles.ForEach(p => mag += p.Velocity.SquareMagnitude);
            mag /= Particles.Count;
            mag *= 1;

            var frame = new Sprite(Width, Height);
            var pixels = frame.Art.GetPixels();
            foreach (var p in Particles)
            {
                //p.Position = new Vector2()
                //{
                //    X = p.Position.X % Width,
                //    Y = p.Position.Y % Height,
                //};

                //p.Position = new Vector2()
                //{
                //    X = p.Position.X < 0 ? p.Position.X + Width - 1 : p.Position.X,
                //    Y = p.Position.Y < 0 ? p.Position.Y + Height - 1 : p.Position.Y,
                //};

                var pass = ((int)p.Position.X >= Width ||
                    (int)p.Position.Y >= Height ||
                    (int)p.Position.X < 0 ||
                    (int)p.Position.Y < 0);

                if (!pass)
                {
                    var pixelIndex = ((int)p.Position.X + ((int)p.Position.Y * Width)) * 4;

                    var color = (p.Velocity.SquareMagnitude / mag) * 255;
                    color = color > 255 ? 255 : color < 0 ? 0 : color;

                    pixels[pixelIndex] = 255;
                    pixels[pixelIndex + 1] = 255;
                    pixels[pixelIndex + 2] = 255;
                    pixels[pixelIndex + 3] = 255;
                }

                p.Position += p.Velocity * delta;

                //if (MouseIsDown)
                //{
                    var positions = new List<Vector2>()
                    {
                        MousePosition,
                        //new Vector2(0, 0),
                    };
                    var forces = positions.ConvertAll(
                        p2 => 
                            Vector2.Right.Rotate(Vector2.Zero, p.Position.AngleToRadians(p2)) / (p.Position.DistanceFrom(p2).Square() / 1000)
                    );

                    var average = Vector2.Zero;
                    foreach (var f in forces) { average += f; }
                    average /= forces.Count;
                    average *= 10;
                    
                    p.Velocity += (average * Timing.TargetFramesPerSecond) * delta;
                //}

                p.Velocity *= 0.99f;
            }
            frame.Art.SetPixels(pixels);

            PreviousFrames.Add(frame);
            if (PreviousFrames.Count > TailSize) { PreviousFrames.RemoveAt(0); }

            foreach (var f in PreviousFrames)
            {
                Art.DrawImage(f, 0, 0, Width, Height);
            }

            //var magSum = 0f;
            //Particles.ForEach(p => magSum += p.Velocity.Magnitude);
            //magSum /= Particles.Count;
            //Title(magSum);

            //Art.Fill(Paint.White);
            //Art.Stroke(Paint.Black);
            //Art.StrokeWeight(1f);
            //Art.TextFont("Airal", 36f);
            //Art.Text(MousePosition + " " + MouseIsDown, 300, 300);
        }
    }

    public class Particle
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
    }
}
