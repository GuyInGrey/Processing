using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Processing;
using Starcraft2.ReplayParser;

namespace Processing_Test
{
    public class TimeGraph : ProcessingCanvas
    {
        List<int> Data;
        int Sections = 30;
        float HoursSpent = 0;

        public TimeGraph()
        {
            CreateCanvas(1000, 1000, 30);
        }

        public void LoadData()
        {
            var folder = @"C:\Users\NoahL\Documents\StarCraft II\Accounts\359663218\1-S2-1-8450479\Replays\Multiplayer";

            var times = new List<DateTime>();
            var i = 0;
            foreach (var file in Directory.GetFiles(folder, "*", SearchOption.AllDirectories))
            {
                if (i % 1 == 0)
                {
                    Console.WriteLine(i + " files scanned...");
                }
                times.Add(File.GetCreationTime(file));

                var replay = Replay.Parse(file, true);
                HoursSpent += (float)replay.GameLength.TotalHours;

                i++;
            }

            Console.WriteLine("File scanning complete.");

            var earliest = times.Min();

            Data = new List<int>();

            i = 0;
            foreach (var time in times)
            {
                if (i % 1000 == 0)
                {
                    Console.WriteLine(i + " time data points calculated...");
                }
                var v = (int)Math.Round((time - earliest).TotalDays);
                var v2 = (int)Math.Round((DateTime.Now - time).TotalDays);
                if (v2 < 30)
                {
                    Data.Add(v);
                }
                i++;
            }
            Data.Sort();
        }

        PSprite DrawnGraph;

        public void Setup()
        {
            LoadData();
            DrawGraph();
            Title("Starcraft 2 - Games Over Time");
        }

        public void DrawGraph()
        {
            DrawnGraph = new PSprite(Width, Height);
            DrawnGraph.Art.Background(new PColor(44, 47, 51));

            var sections = new int[Sections];
            var dataMax = Data.Max();

            var i = 0;
            foreach (var d in Data)
            {
                if (i % 1000 == 0)
                {
                    Console.WriteLine(i + " data sections calculated...");
                }

                var s = (int)PMath.Clamp((float)Math.Floor(Sections * (d / (float)dataMax)), 0, Sections - 1);
                sections[s]++;
                i++;
            }

            var maxFrequency = (float)sections.Max();
            maxFrequency = maxFrequency * 1.1f;

            var sectionPixelWidth = Width / Sections;

            DrawnGraph.Art.Fill(new PColor(114, 137, 218));
            DrawnGraph.Art.NoStroke();

            i = 0;
            foreach (var s in sections)
            {
                var sectionPixelHeight = (int)((s / maxFrequency) * Height);
                DrawnGraph.Art.Rect(i * sectionPixelWidth, Height - sectionPixelHeight, sectionPixelWidth, sectionPixelHeight);
                i++;
            }

            DrawnGraph.Art.Stroke(PColor.Green);
            DrawnGraph.Art.StrokeWeight(3);
            var averageLine = Height - ((int)((sections.Average() / maxFrequency) * Height));
            DrawnGraph.Art.Line(0, averageLine, Width, averageLine);

            DrawnGraph.Art.Fill(PColor.Red);
            DrawnGraph.Art.TextFont(DrawnGraph.Art.CreateFont("Arial", Height / 50f));

            DrawnGraph.Art.Text(((int)maxFrequency).ToString() + " games", 100, 30);
            DrawnGraph.Art.Text("0 games", 70, Height - 60);
            DrawnGraph.Art.Text(Data.Max() + " days ago", 130, Height - 30);
            DrawnGraph.Art.Text("0 days ago", Width - 100, Height - 30);

            DrawnGraph.Art.Text(Data.Count + " total games\n" + Sections + " bars\nTotal hours spent: " + HoursSpent, Width / 2, Height / 2);
        }

        public void Draw(float delta)
        {
            if (DrawnGraph != null)
            {
                Art.DrawImage(DrawnGraph, 0, 0, Width, Height);
            }
        }
    }
}