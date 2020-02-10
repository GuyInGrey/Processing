using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Processing;

namespace Processing_Test
{
    public class MandelbrotV1 : ProcessingCanvas
    {
        int maxDepth = 150;

        PColor[] pallet;
        string folder = @"C:\Mandelbrot\";

        public MandelbrotV1()
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var di = new DirectoryInfo(folder);

            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (var dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            pallet = new PColor[maxDepth];
            for (var i = 0; i < maxDepth; i++)
            {
                var per = i / (float)maxDepth;
                pallet[i] = PColor.LerpMultiple(new[] { PColor.Black, PColor.White, PColor.White, PColor.White }, per);
            }

            CreateCanvas(980, 980, 60);
        }

        public void Setup()
        {
            Console.Write("Enter video framerate: ");
            var framerate = Console.ReadLine();
            Console.Write("Enter total frame count: ");
            var frames = int.Parse(Console.ReadLine());
            Console.Write("Enter output video file name (no extension): ");
            var name = Console.ReadLine().Trim();
            Console.Write("Enter resolution separated by a comma (480,480): ");
            var res = Console.ReadLine().Split(',').ToList().ConvertAll(resPart => int.Parse(resPart)).ToArray();
            Console.Write("Enter mandelbrot power range (x in f(z)=z^x + c) separated by a comma (1,10): ");
            var range = Console.ReadLine().Split(',').ToList().ConvertAll(rangePart => int.Parse(rangePart)).ToArray();
            var s = Stopwatch.StartNew();
            Console.WriteLine("Generation is starting, please wait...");
            var done = 0;

            var items = Enumerable.Range(0, frames).ToArray();
            var chunks = items
                                .Select((s2, i) => new { Value = s2, Index = i })
                                .GroupBy(x => x.Index / 10)
                                .Select(grp => grp.Select(x => x.Value).ToArray()).ToList();

            var threads = new List<Thread>();

            for (var t = 0; t < Environment.ProcessorCount - 1; t++)
            {
                var t2 = t;
                var thread = new Thread(() =>
                {
                    start:;
                    if (chunks.Count == 0) { goto exit; }

                    var assignedChunk = chunks[0];
                    chunks.RemoveAt(0);
                    foreach (var a in assignedChunk)
                    {
                        var pow = PMath.Map(a, 1, frames, range[0], range[1]);
                        var Image = new PSprite(res[0], res[1]);

                        var pixels = Image.Art.GetPixels();
                        for (var y = 0; y < Image.Height; y++)
                        {
                            for (var x = 0; x < Image.Width; x++)
                            {
                                var xTarget = 4f;
                                var yTarget = (xTarget / Image.Width) * Image.Height;

                                var x2 = (x / (Image.Width / (xTarget))) - (xTarget / 2);
                                var y2 = (y / (Image.Height / (yTarget))) - (yTarget / 2);

                                var c = new Complex(x2, y2);
                                var z = Complex.Zero;
                                var max = -1f;
                                var maxWasHit = false;
                                for (var i = 0; i < maxDepth; i++)
                                {
                                    z = (Complex.Pow(z, pow)) + c;
                                    if ((z.Real * z.Real) + (z.Imaginary * z.Imaginary) > 16 && max == -1f)
                                    {
                                        max = i;
                                        maxWasHit = true;
                                        break;
                                    }
                                }

                                var color = PColor.Black;

                                if (maxWasHit)
                                {
                                    var floorMax = (int)Math.Floor(max);
                                    var c1 = pallet[floorMax % pallet.Length];
                                    var c2 = pallet[(floorMax + 1) % pallet.Length];
                                    color = PColor.Lerp(c1, c2, 0.5f);
                                }

                                var pos = ((y * Image.Width) + x) * 4;

                                pixels[pos] = (byte)(color.R);
                                pixels[pos + 1] = (byte)(color.G);
                                pixels[pos + 2] = (byte)(color.B);
                                pixels[pos + 3] = 255;
                            }
                        }
                        Image.Art.SetPixels(pixels);
                        Image.Save(folder + a.ToString("000000") + ".png");
                        var percent = (done / (float)frames);
                        var remaining = (1f - percent) * 100;
                        var secondsPerPercent = s.Elapsed.TotalSeconds / (percent * 100f);
                        var minutesRemaining = ((secondsPerPercent * remaining) / 60f);
                        var estimatedTime = DateTime.Now.AddMinutes(done == 0 ? 0 : minutesRemaining);
                        Console.WriteLine("Progress: " + done + "/" + frames + "  " + (percent * 100f).ToString("0.00") + 
                            "%   Estimated completion time: " + estimatedTime);
                        done++;
                    }
                    goto start;
                    exit:;
                })
                {
                    Name = "Mandelbrot: Core " + (t + 1),
                    Priority = ThreadPriority.AboveNormal,
                };
                threads.Add(thread);
                thread.Start();
            };

            var info = new ProcessStartInfo()
            {
                FileName = "ffmpeg",
                Arguments = string.Format(" -framerate {0} -i %06d.png -c:v libx264 -r {0} {1}.mp4", framerate, name),
                WorkingDirectory = folder,
            };
            
            var activeThreads = 1;

            do
            {
                Console.Title = "Active thread count: " + activeThreads;
                activeThreads = threads.Where(t => t.IsAlive).Count();
                Thread.Sleep(1000);
                Title(activeThreads);
            }
            while (activeThreads > 0);

            Console.Title = "Active thread count: 0";

            var p = Process.Start(info);
            p.WaitForExit();

            Console.WriteLine(@"Saved as " + folder + "out.mp4");
            Console.ReadLine();
            Close();
        }

        public void Draw(float delta)
        {
            //Art.DrawImage(Image, 0, 0, Width, Height);
        }

        public static ProcessThread GetProcessThreadFromWin32ThreadId(int threadId)
        {
            if (threadId == 0)
            {
                threadId = GetCurrentWin32ThreadId();
            }

            foreach (var processThread in from process in Process.GetProcesses()
                                          from ProcessThread processThread in process.Threads
                                          where processThread.Id == threadId
                                          select processThread)
            {
                return processThread;
            }

            Console.WriteLine("Error: No thread matching thread Id " + threadId + " was found.");
            return null;
        }

        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        public static extern Int32 GetCurrentWin32ThreadId();
    }
}