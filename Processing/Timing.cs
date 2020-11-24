using System;
using System.Collections.Generic;

namespace Processing
{
    public class Timing
    {
        public int TotalFramesRendered { get; private set; }
        public int TargetFramesPerSecond { get; set; }
        public int ActualFramesPerSecond { get; private set; }

        private List<DateTime> Frames = new List<DateTime>();

        public void FrameRendered()
        {
            Frames.Add(DateTime.Now);
            Frames.RemoveAll(f => f < DateTime.Now.Subtract(new TimeSpan(0, 0, 1)));
            ActualFramesPerSecond = Frames.Count;

            TotalFramesRendered++;
        }
    }
}
