using System;
using System.Collections.Generic;

namespace Processing
{
    public static class PMath
    {
        public static FastNoise _FastNoise;

        private readonly static Random _Random;

        static PMath()
        {
            _Random = new Random();
            _FastNoise = new FastNoise();
        }
        public static float Sigmoid(float value) =>
            (float)(1.0 / (1.0 + Math.Pow(Math.E, -value)));

        public static float Lerp(float a, float b, float t) =>
            a * (1 - t) + b * t;

        public static float BezierBlend(float t)
        {
            return t * t * (3.0f - 2.0f * t);
        }

        public static float ParametricBlend(float t)
        {
            var sqt = t * t;
            return sqt / (2.0f * (sqt - t) + 1.0f);
        }
        
        public static float JumpingParametricBlend(float t)
        {
            return ParametricBlend(t) + Sin(28 * (0.5f * PI) * t) / 20f;
        }

        public static float Map(float val, float a1, float b1, float a2, float b2) =>
            Lerp(a2, b2, (val - a1) / (b1 - a1));

        public static float Sin(float a) =>
            (float)Math.Sin(a);
        public static float Cos(float a) =>
            (float)Math.Cos(a);
        public static float Tan(float a) =>
            (float)Math.Tan(a);
        public static float Pow(float a, float b) =>
            (float)Math.Pow(a, b);
        public static float Sqrt(float a) =>
            (float)Math.Sqrt(a);
        public static float Square(float a) =>
            a * a;

        public static float Clamp(float val, float min, float max) =>
            val > max ? max : val < min ? min : val;

        public static float Noise(float x) =>
            Noise(x, 0, 0);

        public static float Noise(float x, float y) =>
            Noise(x, y, 0);

        public static float Noise(float x, float y, float z) =>
            Map(_FastNoise.GetPerlin(x * 1.1f, y * 1.1f, z * 1.1f), -0.5f, 0.5f, 0, 1);

        public static float Radians(float degrees) =>
            degrees * ((float)Math.PI / 180f);

        public static float Degrees(float radians)
            => radians * (180f / (float)Math.PI);

        public static float Random(float min, float max)
            => Map((float)_Random.NextDouble(), 0, 1, min, max);

        public static float Random(float max)
            => Random(0, max);

        public static float Round(float input) =>
            (float)Math.Round(input);
        public static float Floor(float input) =>
            (float)Math.Floor(input);

        public static float FOUR_PI => PI * 4f;
        public static float TWO_PI => PI * 2f;
        public static float PI => (float)Math.PI;
        public static float HALF_PI => PI / 2f;
        public static float QUARTER_PI => PI / 4f;

        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}