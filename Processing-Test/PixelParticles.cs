using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Processing;
using MathNet.Numerics.Interpolation;
using System.Linq;
using System.Collections.Specialized;
using System.Collections;

namespace Processing_Test
{
    public class PixelParticles : ProcessingCanvas
    {
        string StartImagePath = @"C:\Processing\PixelParticles\Start2.png";
        string EndImagePath = @"C:\Processing\PixelParticles\End2.png";
        string OutputImagesFolderPath = @"C:\Processing\PixelParticles\OutputImages\";
        string OutputVideoPath = @"C:\Processing\PixelParticles\Output";
        float TimeToTakeSeconds = 15f;
        int PixelLength = 4;
        int BufferFrames = 60;
        int TimeToTakeFrames => (int)(TimeToTakeSeconds * FrameRateTarget);
        bool Count;
        float Offset = 1.0f;
        int width;
        int height;

        string ExportMode = "mkv"; // "mkv", "gif", "mp4"

        PSprite StartImage;
        PSprite EndImage;
        PSprite BackgroundImage;

        OrderedDictionary AvailablePositions;
        bool[,] Available2;

        List<Pixel> pixels;

        public PixelParticles(string[] args)
        {
            var mode = "";
            foreach (var a in args)
            {
                if (a.Trim() == string.Empty) { continue; }
                if (a.StartsWith("-")) { mode = a.Substring(1); }
                else
                {
                    switch(mode)
                    {
                        case "s": StartImagePath = a; break;
                        case "e": EndImagePath = a; break;
                        case "f": OutputImagesFolderPath = a; break;
                        case "o": OutputVideoPath = a; break;
                        case "t": TimeToTakeSeconds = float.Parse(a); break;
                        case "p": PixelLength = int.Parse(a); break;
                        case "b": BufferFrames = int.Parse(a); break;
                        case "x": ExportMode = a; break;
                        case "r": FrameRateTarget = int.Parse(a); break;
                        case "i": BackgroundImage = PSprite.FromFilePath(a); break;
                    }
                }

                if (a == "-count") { Count = true; }
            }

            CreateCanvas(1920, 1080, FrameRateTarget);
        }

        void CalculateStart(int width, int height)
        {
            this.width = width;
            this.height = height;

            AvailablePositions = new OrderedDictionary();
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (StartImage.Art.GetPixel(x, y).A != 0)
                    {
                        AvailablePositions.Add((x, y), (x, y));
                    }
                    if (x == 0 && y % 10 == 0)
                    {
                        ClearLine();
                        Console.WriteLine("Calculating Start Image Pixels...  " + y + " / " + Height);
                    }
                }
            }

            Available2 = new bool[width, height];
            var i = 0;
            foreach (DictionaryEntry p2 in AvailablePositions)
            {
                var p = ((int, int))p2.Key;
                Available2[p.Item1, p.Item2] = true;

                if (i % (AvailablePositions.Count / 100) == 0)
                {
                    ClearLine();
                    Console.WriteLine("Re-calculating Start Image Pixels...  " + i + " / " + AvailablePositions.Count);
                }
                i++;
            }

            var r = new Random();
            var two = AvailablePositions.Cast<DictionaryEntry>()
                    .OrderBy(x => r.Next())
                    .ToDictionary(c => c.Key, d => d.Value);

            AvailablePositions = new OrderedDictionary();

            foreach (var two2 in two)
            {
                AvailablePositions.Add(two2.Key, two2.Value);
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

            if (Count)
            {
                var startCount = 0;
                var endCount = 0;

                for (var y = 0; y < Height; y++)
                {
                    for (var x = 0; x < Width; x++)
                    {
                        if (StartImage.Art.GetPixel(x, y).A > 0)
                        {
                            startCount++;
                        }
                        if (EndImage.Art.GetPixel(x, y).A > 0)
                        {
                            endCount++;
                        }
                    }
                }

                Console.WriteLine("Start: " + startCount);
                Console.WriteLine("End: " + endCount);
                Console.ReadLine();

                return;
            }

            var startPixels = StartImage.Art.GetPixels();

            AvailablePositions = new OrderedDictionary();
            pixels = new List<Pixel>(Width * Height);

            CalculateStart(Width, Height);

            var imagePixels = EndImage.Art.GetPixels();

            var totalDone = 0;

            var width = Width;
            var height = Height;
            var generating = false;
            var count = 0;

            var distance = 0;

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

                        var pos = (-1, -1);

                        distance -= 2;
                        if (distance < 0) { distance = 0; }
                        goto endWhile;
                        do
                        {
                            //for (var y2 = y - distance; y2 < y + distance + 1; y2++)
                            //{
                            //    for (var x2 = x - distance; x2 < x + distance + 1; x2++)
                            //    {
                            //        if (x2 > 0 && x2 < width && y2 > 0 && y2 < height)
                            //        {
                            //            pos = Check(x2, y2);
                            //            if (pos != (-1, -1)) { goto endWhile; }
                            //        }
                            //    }
                            //}

                            for (var y2 = (int)PMath.Clamp(y - distance, 0, height - 1); y2 < (int)PMath.Clamp(y + distance + 1, 0, height - 1); y2++)
                            {
                                var minus = (int)PMath.Clamp(x - distance, 0, width - 1);
                                var tempPos = (minus, y2);
                                pos = Available2[minus, y2] ? tempPos : (-1, -1);
                                if (pos != (-1, -1)) { goto endWhile; }

                                var plus = (int)PMath.Clamp(x + distance, 0, width - 1);
                                tempPos = (plus, y2);
                                pos = Available2[plus, y2] ? tempPos : (-1, -1);
                                if (pos != (-1, -1)) { goto endWhile; }
                            }

                            for (var x2 = (int)PMath.Clamp(x - distance, 0, width - 1); x2 < (int)PMath.Clamp(x + distance + 1, 0, width - 1); x2++)
                            {
                                var minus = (int)PMath.Clamp(y - distance, 0, height - 1);
                                var tempPos = (x2, minus);
                                pos = Available2[x2, minus] ? tempPos : (-1, -1);
                                if (pos != (-1, -1)) { goto endWhile; }

                                var plus = (int)PMath.Clamp(y + distance, 0, height - 1);
                                tempPos = (x2, plus);
                                pos = Available2[x2, plus] ? tempPos : (-1, -1);
                                if (pos != (-1, -1)) { goto endWhile; }
                            }

                            if (distance > width)
                            {
                                CalculateStart(width, height);
                                distance = 0;
                            }

                            distance++;
                        }
                        while (pos == (-1, -1));
                        endWhile:;
                        pos = ((int, int))AvailablePositions[0];

                        AvailablePositions.RemoveAt(0);
                        Available2[pos.Item1, pos.Item2] = false;
                        var b2 = (pos.Item1 + (pos.Item2 * width)) * 4;

                        pixels.Add(new Pixel() { StartColor = PColor.FromPixels(startPixels, b2), EndColor = c, StartPos = pos, EndPos = (x, y), Offset = PMath.Random(0,Offset) });
                    }
                    totalDone++;

                    if (totalDone % width == 0)
                    {
                        var percent = (totalDone / (float)(width * height)) * 100;
                        ClearLine();
                        Console.WriteLine("Initial Pixel Object Generation - " + percent.ToString("00.00") + "% complete. " + totalDone + "/" + (width * height) + "    " + y + "/" + height);
                        Console.Title = AvailablePositions.Count + "   " + count + "    " + distance;
                    }
                }
            }

            pixels.TrimExcess();

            var di = new DirectoryInfo(OutputImagesFolderPath);
            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }

            PSprite toSave;

            for (var i = 0; i < BufferFrames; i++)
            {
                toSave = new PSprite(Width, Height);
                toSave.Art.DrawImage(BackgroundImage, 0, 0, Width, Height);
                toSave.Art.DrawImage(StartImage, 0, 0, Width, Height);
                toSave.Save(OutputImagesFolderPath + i.ToString("000000") + ".png");
                toSave.Dispose();
            }
        }

        bool videoRendered = false;
        List<PSprite> PreviousFrames;

        public void Draw(float delta)
        {
            if (Count) { Close(); return; }
            var t1 = Offset + 1f;

            if (PreviousFrames == null) { PreviousFrames = new List<PSprite>(); }

            Title(TotalFrameCount + " / " + (TimeToTakeFrames * t1));
            var percent = (TotalFrameCount) / (float)TimeToTakeFrames;
            //percent = PMath.ParametricBlend(percent);

            var screen = Art.GetPixels();
            for (var i = 0; i < screen.Length; i++)
            {
                screen[i] = 0;
            }

            foreach (var p in pixels)
            {
                var percent2 = PMath.Clamp(percent - p.Offset, 0, 1);
                percent2 = PMath.ParametricBlend(percent2);

                var pos = (
                    (int)PMath.Round(PMath.Clamp(PMath.Lerp(p.StartPos.Item1, p.EndPos.Item1, percent2), 0, Width - 1)), 
                    (int)PMath.Round(PMath.Clamp(PMath.Lerp(p.StartPos.Item2, p.EndPos.Item2, percent2), 0, Height - 1))
                );

                var color = PColor.Lerp(p.StartColor, p.EndColor, percent2);
                var b = (pos.Item1 + (pos.Item2 * Width)) * 4;

                screen[b] = (byte)color.R;
                screen[b + 1] = (byte)color.G;
                screen[b + 2] = (byte)color.B;
                screen[b + 3] = (byte)color.A;
            }

            Art.SetPixels(screen);

            if (TotalFrameCount <= TimeToTakeFrames * t1)
            {
                var fromScreen = new PSprite(this);
                PreviousFrames.Add(fromScreen);

                if (PreviousFrames.Count > PixelLength)
                {
                    PreviousFrames.RemoveAt(0);
                }

                var toSave = new PSprite(Width, Height);

                Art.Clear();
                toSave.Art.DrawImage(BackgroundImage, 0, 0, Width, Height);
                for (var i = 0; i < PreviousFrames.Count; i++)
                {
                    toSave.Art.DrawImage(PreviousFrames[i], 0, 0, Width, Height);
                }

                Art.DrawImage(toSave, 0, 0, Width, Height);

                toSave.Save(OutputImagesFolderPath + (TotalFrameCount + BufferFrames).ToString("000000") + ".png");
                Console.WriteLine("Saved frame " + TotalFrameCount);
            }

            if (TotalFrameCount > TimeToTakeFrames * t1 && !videoRendered)
            {
                videoRendered = true;
                WindowVisible = false;

                Task.Run(() =>
                {
                    PSprite toSave;
                    for (var i = (int)(TimeToTakeFrames * t1) + BufferFrames; i < (TimeToTakeFrames * t1) + (BufferFrames * 2); i++)
                    {
                        toSave = new PSprite(width, height);
                        toSave.Art.DrawImage(BackgroundImage, 0, 0, Width, Height);
                        toSave.Art.DrawImage(EndImage, 0, 0, Width, Height);
                        toSave.Save(OutputImagesFolderPath + i.ToString("000000") + ".png");
                        toSave.Dispose();
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
        public float Offset;
    }
}