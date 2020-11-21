using System.Collections.Generic;

namespace Processing
{
    public static class MathExtensions
    {
        public static float Sigmoid(this float a) => PMath.Sigmoid(a);
        public static float Lerp(this float t, float a, float b) => PMath.Lerp(a, b, t);
        public static float BezierBlend(this float a) => PMath.BezierBlend(a);
        public static float ParametricBlend(this float a) => PMath.ParametricBlend(a);
        public static float JumpingParametricBlend(this float a) => PMath.JumpingParametricBlend(a);
        public static float Map(this float v, float a1, float b1, float a2, float b2) => PMath.Map(v, a1, b1, a2, b2);
        public static float Sin(this float a) => PMath.Sin(a);
        public static float Cos(this float a) => PMath.Cos(a);
        public static float Tan(this float a) => PMath.Tan(a);
        public static float Pow(this float a, float b) => PMath.Pow(a, b);
        public static float Sqrt(this float a) => PMath.Sqrt(a);
        public static float Square(this float a) => a * a;
        public static float Clamp(this float a, float min, float max) => PMath.Clamp(a, min, max);
        public static float DegreesToRadians(this float d) => PMath.Radians(d);
        public static float RadiansToDegrees(this float r) => PMath.Degrees(r);
        public static float Round(this float a) => PMath.Round(a);
        public static float Floor(this float a) => PMath.Floor(a);
        public static void Shuffle<T>(this IList<T> list) => PMath.Shuffle(list);
    }
}
