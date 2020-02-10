using System;

namespace Processing
{
    public class NoiseLoop : FastNoise
    {
        public float Diameter;
        public float Min;
        public float Max;
        public float CenterX;
        public float CenterY;

        public NoiseLoop(float d, float min, float max)
        {
            Diameter = d;
            Min = min;
            Max = max;
            CenterX = PMath.Random(1000);
            CenterY = PMath.Random(1000);
        }

        public float Value(float a)
        {
            var xOff = PMath.Map(PMath.Cos(a), -1, 1, CenterX, Diameter + CenterX);
            var yOff = PMath.Map(PMath.Sin(a), -1, 1, CenterY, Diameter + CenterY);

            SetNoiseType(NoiseType.PerlinFractal);
            SetInterp(Interp.Quintic);
            SetFractalType(FractalType.RigidMulti);
            SetFractalLacunarity(3);

            SetFrequency(0.1f);

            var r = GetNoise(xOff, yOff);
            return PMath.Map(r, -0.5f, 0.5f, Min, Max);
        }
    }
}