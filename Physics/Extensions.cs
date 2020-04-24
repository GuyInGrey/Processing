using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Box2DNet.Dynamics;
using Processing;

namespace Physics
{
    public static class Extensions
    {
        public static float Scale;
        public static int ScreenSize;

        public static Color ToBoxColor(this PColor c) =>
            new Color(c.R / 255f, c.G / 255f, c.B / 255f);

        public static PColor ToPColor(this Color c) =>
            new PColor((int)(c.R * 255f), (int)(c.G * 255f), (int)(c.B * 255f));

        public static Vector2 WorldToScreen(this Vector2 v) => 
            new Vector2(v.X.WorldToScreen(), PMath.Map(v.Y.WorldToScreen(), 0, ScreenSize, ScreenSize, 0));

        public static float WorldToScreen(this float f) =>
            PMath.Map(f, -Scale, Scale, 0, ScreenSize);

        public static Vector2 ScreenToWorld(this Vector2 v) =>
            new Vector2(v.X.ScreenToWorld(), PMath.Map(v.Y.ScreenToWorld(), -Scale, Scale, Scale, -Scale));

        public static float ScreenToWorld(this float f) =>
            PMath.Map(f, 0, ScreenSize, -Scale, Scale);

        public static Vector2[] ScreenToWorld(this Vector2[] v) =>
            v.ToList().ConvertAll(c => c.ScreenToWorld()).ToArray();

        public static Vector2[] WorldToScreen(this Vector2[] v) =>
            v.ToList().ConvertAll(c => c.WorldToScreen()).ToArray();
    }
}