namespace Processing
{
    public class ProcessingCanvas : Canvas
    {
        public bool DebugMode = false;

        public ProcessingCanvas()
        {

        }

        internal void DebugDraw(float delta)
        {
            if (DebugMode)
            {
                Art.Stroke(PColor.White);
                Art.StrokeWeight(2f);
                Art.Fill(PColor.Black);

                var maxDelta = (1000f / FrameRateTarget) / 1000f;
                var x = (int)PMath.Map(delta, 0, maxDelta * 2, 0, Width);
                x = (int)(PMath.Sigmoid(PMath.Map(x, 0, Width, -1, 1)) * Width);
                var y = (int)PMath.Map(FrameRateCurrent, 0, FrameRateTarget * 2, 0, Height);
                y = (int)(PMath.Sigmoid(PMath.Map(y, 0, Height, -1, 1)) * Height);
                Art.Circle(x, y, 20);
                Art.Stroke(PColor.Red);
                Art.Fill(PColor.White);
                Art.Circle(Width / 2, Height / 2, 5);

                Art.TextFont(Art.CreateFont("Arial", 12f));
                Art.Text("Delta Too Low", (Width / 2) - 75, Height / 2);
                Art.Text("Delta Too High", (Width / 2) + 75, Height / 2);
                Art.Text("Framerate Too Low", Width / 2, (Height / 2) - 75);
                Art.Text("Framerate Too High", Width / 2, (Height / 2) + 75);
            }
        }
    }
}