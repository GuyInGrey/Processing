using System;
using System.Collections.Generic;
using System.Diagnostics;
using Processing;

namespace Processing_Test
{
    public class CPUGraph : Canvas
    {
        public CPUGraph()
        {
            for (var i = 0; i < 15; i++)
            {
                counters.Add(new PerformanceCounter("Processor", "% Processor Time", "_Total", Environment.MachineName));
            }
            CreateCanvas(1000, 1000, 15);
        }

        List<int> History = new List<int>();
        List<PerformanceCounter> counters = new List<PerformanceCounter>();

        public int GetCpuUsage(int frame)
        {
            var counter = counters[frame % counters.Count];
            return (int)counter.NextValue();
        }

        public override void Draw(float delta)
        {
            var maxHeight = Height * 0.66f;

            History.Add(GetCpuUsage(Timing.TotalFramesRendered));
            if (History.Count > Width) { History.RemoveAt(0); }

            Art.Background(Paint.CornflowerBlue);
            Art.Fill(Paint.White);
            Art.Stroke(Paint.Black);
            Art.StrokeWeight(1);

            Art.BeginShape();

            for (var i = 0; i < History.Count; i++)
            {
                var p = (float)History[i];
                var x = ((float)i).Map(0, History.Count, 0, Width);
                var y = p.Map(0, 100, Height, maxHeight);
                Art.Vertex(x, y);
            }

            Art.Vertex(Width, Height);
            Art.Vertex(0, Height);

            Art.EndShape(EndShapeType.Close);

            Art.Line(0, maxHeight, Width, maxHeight);
        }
    }
}
