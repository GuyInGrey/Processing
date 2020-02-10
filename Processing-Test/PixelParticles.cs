using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Processing;
using MathNet.Numerics.Interpolation;

namespace Processing_Test
{
    public class PixelParticles : ProcessingCanvas
    {
        const string StartImagePath = @"C:\Processing\PixelParticles\Start.png";
        const string EndImagePath = @"C:\Processing\PixelParticles\End.png";
        const string OutputImagesFolderPath = @"C:\Processing\PixelParticles\OutputImages\";
        const string OutputVideoPath = @"C:\Processing\PixelParticles\Output";
        const float TimeToTakeSeconds = 3f;
        const int PixelLength = 2;
        int BufferFrames => 20;
        int TimeToTakeFrames => (int)(TimeToTakeSeconds * FrameRateTarget);

        const string ExportMode = "mkv"; // "mkv", "gif", "mp4"

        PSprite StartImage;
        PSprite EndImage;

        List<(int, int)> AvailablePositions;
        List<Pixel> pixels;

        public PixelParticles()
        {
            CreateCanvas(1920, 1080, 30);
        }

        void CalculateStart(int width, int height)
        {
            AvailablePositions = new List<(int, int)>();
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (StartImage.Art.GetPixel(x, y).A != 0)
                    {
                        AvailablePositions.Add((x, y));
                    }
                    if (x == 0 && y % 10 == 0)
                    {
                        ClearLine();
                        Console.WriteLine("Calculating Start Image Pixels...  " + y + " / " + Height);
                    }
                }
            }
        }

        public static void ClearLine()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, 0);
        }

        public void Setup()
        {
            StartImage = PSprite.FromFilePath(StartImagePath);
            EndImage = PSprite.FromFilePath(EndImagePath);
            var startPixels = StartImage.Art.GetPixels();

            AvailablePositions = new List<(int, int)>();
            pixels = new List<Pixel>(Width * Height);

            CalculateStart(Width, Height);

            var imagePixels = EndImage.Art.GetPixels();

            var totalDone = 0;

            var width = Width;
            var height = Height;
            var generating = false; ;

            //Parallel.For(0, height, (y) =>
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var b = (x + (y * width)) * 4;
                    var c = PColor.FromPixels(imagePixels, b);

                    if (c.A > 0)
                    {
                        if (AvailablePositions.Count == 0) 
                        { 
                            if (generating)
                            {
                                while (generating) { }
                                goto b;
                            }

                            generating = true; 
                            CalculateStart(width, height); 
                            generating = false; 
                        }

                        b:;

                        var index = (int)PMath.Random(0, AvailablePositions.Count - 1);
                        var pos = AvailablePositions[index];
                        AvailablePositions.RemoveAt(index);
                        var b2 = (pos.Item1 + (pos.Item2 * width)) * 4;

                        pixels.Add(new Pixel() { StartColor = PColor.FromPixels(startPixels, b2), EndColor = c, StartPos = pos, EndPos = (x, y) });
                    }
                    totalDone++;

                    if (totalDone % width == 0)
                    {
                        var percent = (totalDone / (float)(width * height)) * 100;
                        ClearLine();
                        Console.WriteLine("Initial Pixel Object Generation - " + percent.ToString("00.00") + "% complete. " + totalDone + "/" + (width * height) + "    " + y + "/" + height);
                    }
                }
            }//);

            pixels.TrimExcess();

            var di = new DirectoryInfo(OutputImagesFolderPath);
            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }

            for (var i = 0; i < BufferFrames; i++)
            {
                StartImage.Save(OutputImagesFolderPath + i.ToString("000000") + ".png");
            }
        }

        bool videoRendered = false;
        List<PSprite> PreviousFrames;

        public void Draw(float delta)
        {
            if (PreviousFrames == null) { PreviousFrames = new List<PSprite>(); }

            Title(TotalFrameCount + " / " + TimeToTakeFrames);
            var percent = PMath.Clamp((TotalFrameCount) / (float)(TimeToTakeFrames), 0, 1);
            percent = PMath.ParametricBlend(percent);

            var screen = Art.GetPixels();
            for (var i = 0; i < screen.Length; i++)
            {
                screen[i] = 0;
            }

            foreach (var p in pixels)
            {
                var pos = (
                    (int)PMath.Clamp(PMath.Lerp(p.StartPos.Item1, p.EndPos.Item1, percent), 0, Width - 1), 
                    (int)PMath.Clamp(PMath.Lerp(p.StartPos.Item2, p.EndPos.Item2, percent), 0, Height - 1)
                );

                var color = PColor.Lerp(p.StartColor, p.EndColor, percent);
                var b = (pos.Item1 + (pos.Item2 * Width)) * 4;

                screen[b] = (byte)color.R;
                screen[b + 1] = (byte)color.G;
                screen[b + 2] = (byte)color.B;
                screen[b + 3] = (byte)color.A;
            }

            Art.SetPixels(screen);

            if (TotalFrameCount <= TimeToTakeFrames)
            {
                var fromScreen = new PSprite(this);
                PreviousFrames.Add(fromScreen);

                if (PreviousFrames.Count > PixelLength)
                {
                    PreviousFrames.RemoveAt(0);
                }

                var toSave = new PSprite(Width, Height);

                Art.Clear();
                for (var i = 0; i < PreviousFrames.Count; i++)
                {
                    toSave.Art.DrawImage(PreviousFrames[i], 0, 0, Width, Height);
                }

                Art.DrawImage(toSave, 0, 0, Width, Height);

                toSave.Save(OutputImagesFolderPath + (TotalFrameCount + BufferFrames).ToString("000000") + ".png");
                Console.WriteLine("Saved frame " + TotalFrameCount);
            }

            if (TotalFrameCount > TimeToTakeFrames && !videoRendered)
            {
                videoRendered = true;
                WindowVisible = false;

                Task.Run(() =>
                {
                    for (var i = TimeToTakeFrames + BufferFrames; i < TimeToTakeFrames + (BufferFrames * 2); i++)
                    {
                        EndImage.Save(OutputImagesFolderPath + i.ToString("000000") + ".png");
                    }

                    var args = string.Format(" -y -framerate {0} -i %06d.png {3}-r {0} {1}.{2}", 
                        FrameRateTarget, OutputVideoPath, ExportMode, ExportMode != "gif" ? "-c:v libx264 " : "");

                    var info = new ProcessStartInfo()
                    {
                        FileName = "ffmpeg",
                        Arguments = args,
                        WorkingDirectory = OutputImagesFolderPath,
                        UseShellExecute = false,
                    };

                    var p = Process.Start(info);
                    p.WaitForExit();

                    Console.WriteLine("\n\nVideo Exported:\n" + OutputVideoPath + "." + ExportMode + "\n\nPress enter to close.");
                    Console.ReadLine();
                    Close();
                });
            }
        }
    }

    public class Pixel
    {
        public (int, int) StartPos;
        public (int, int) EndPos;
        public PColor StartColor;
        public PColor EndColor;
    }
}