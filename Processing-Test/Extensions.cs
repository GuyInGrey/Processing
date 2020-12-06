using System;

namespace Processing_Test
{
    public static class Extensions
    {
        private static readonly Random random = new Random();

        public static float RandomNumberBetween(float minValue, float maxValue)
        {
            var next = random.NextDouble();

            return minValue + ((float)next * (maxValue - minValue));
        }
    }
}
