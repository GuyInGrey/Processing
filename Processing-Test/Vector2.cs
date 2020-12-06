using System;
using System.Runtime.CompilerServices;
using Processing;

namespace Processing_Test
{
    public struct Vector2
    {
        /// <summary>
        /// Represents a Vector2 object with coordinates (0,0).
        /// </summary>
        public static readonly Vector2 Zero = new Vector2(0f, 0f);

        /// <summary>
        /// Represents a Vector2 object with coordinates (1,1).
        /// </summary>
        public static readonly Vector2 One = new Vector2(1f, 1f);

        /// <summary>
        /// Represents a Vector2 object with coordinates (0,1).
        /// </summary>
        public static readonly Vector2 Up = new Vector2(0f, 1f);

        /// <summary>
        /// Represents a Vector2 object with coordinates (0,1).
        /// </summary>
        public static readonly Vector2 Down = new Vector2(0f, -1f);

        /// <summary>
        /// Represents a Vector2 object with coordinates (0,1).
        /// </summary>
        public static readonly Vector2 Left = new Vector2(-1f, 0f);

        /// <summary>
        /// Represents a Vector2 object with coordinates (0,1).
        /// </summary>
        public static readonly Vector2 Right = new Vector2(1f, 0f);

        /// <summary>
        /// The coordinate on the X axis.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// The coordinate on the Y axis.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Access the x, y components using [0], [1] respectively. Null will return 0.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Nullable<float> this[int index]
        {
            get
            {
                switch (index)
                {
                    case (0):
                        return X;
                    case (1):
                        return Y;
                    default:
                        return null;
                }
            }

            set
            {
                switch (index)
                {
                    case (0):
                        X = value.GetValueOrDefault();
                        break;
                    case (1):
                        Y = value.GetValueOrDefault();
                        break;
                    default:
                        break;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2 vec)
            {
                return Equals(vec);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)(X * Y * SquareMagnitude) + base.GetHashCode();
        }

        public bool Equals(Vector2 obj) => (obj.X == X && obj.Y == Y);

        /// <summary>
        /// Returns the squared length of this vector.
        /// </summary>
        public float SquareMagnitude => (X * X) + (Y * Y);

        /// <summary>
        /// Returns the length of this vector. It is reccomended you use <see cref="SquareMagnitude"/>. <see cref="Math.Sqrt(double)"/> is resource intensive.
        /// </summary>
        public float Magnitude => (float)Math.Sqrt(SquareMagnitude);

        /// <summary>
        /// Returns the normalized vector.
        /// </summary>
        public Vector2 Normalized => Magnitude == 0 ? Vector2.Zero : this * (1f / Magnitude);

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        public void Normalize() => this = Normalized;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(float both) : this(both, both) { }

        public Vector2(Point p) : this(p.X, p.Y) { }

        public static Vector2 Add(params Vector2[] vecs)
        {
            var toReturn = Zero;
            foreach (var v in vecs)
            {
                toReturn += v;
            }

            return toReturn;
        }

        /// <summary>
        /// Add the specified vectors.
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result of addition</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Add(Vector2 a, Vector2 b)
            => a + b;

        /// <summary>
        /// Subtracts the specified vectors.
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result of subtraction</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Subtract(Vector2 a, Vector2 b)
            => a - b;

        /// <summary>
        /// Multiply a vector by a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <returns>Result of the multiplication</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Multiply(Vector2 a, float f)
            => a * f;

        /// <summary>
        /// Divide a vector by a scalar
        /// </summary>
        /// <param name="a">Vector operand</param>
        /// <param name="f">Scalar operand</param>
        /// <returns>Result of the division</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Divide(Vector2 a, float f)
            => a / f;

        /// <summary>
        /// Calculate the dot (scalar) product of two vectors
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>The dot product of the two inputs</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Vector2 a, Vector2 b)
            => (a.X * b.X) + (a.Y * b.Y);

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <returns>a when blend=0, b when blend=1, and a linear combination otherwise</returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float blend)
            => new Vector2(blend * (b.X - a.X) + a.X, blend * (b.Y - a.Y) + a.Y);

        /// <summary>
        /// Interpolate 3 Vectors using Barycentric coordinates
        /// </summary>
        /// <param name="a">First input Vector</param>
        /// <param name="b">Second input Vector</param>
        /// <param name="c">Third input Vector</param>
        /// <param name="u">First Barycentric Coordinate</param>
        /// <param name="v">Second Barycentric Coordinate</param>
        /// <returns>a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 BaryCentric(Vector2 a, Vector2 b, Vector2 c, float u, float v)
            => a + u * (b - a) + v * (c - a);

        /// <summary>
        /// Scales the Vector2 to unit length.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Normalize(Vector2 a)
            => a.Normalized;

        /// <summary>
        /// Clamp a vector to the given minimum and maximum vectors
        /// </summary>
        /// <param name="vec">Input vector</param>
        /// <param name="min">Minimum vector</param>
        /// <param name="max">Maximum vector</param>
        /// <returns>The clamped vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Clamp(Vector2 vec, Vector2 min, Vector2 max)
        {
            vec.X = vec.X < min.X ? min.X :
                vec.X > max.X ? max.X : vec.X;
            vec.Y = vec.Y < min.Y ? min.Y :
                vec.Y > max.Y ? max.Y : vec.Y;
            return vec;
        }

        /// <summary>
        /// Rotates the vector around a center of rotation.
        /// </summary>
        /// <param name="centerPoint">Center of rotation.</param>
        /// <param name="angleInRadians">Angle to rorate in radians.</param>
        /// <returns></returns>
        public Vector2 Rotate(Vector2 centerPoint, float angleInRadians)
        {
            var cosTheta = Math.Cos(angleInRadians);
            var sinTheta = Math.Sin(angleInRadians);

            return new Vector2()
            {
                X =
                    (float)
                    (cosTheta * (X - centerPoint.X) -
                    sinTheta * (Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (float)
                    (sinTheta * (X - centerPoint.X) +
                    cosTheta * (Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        /// <summary>
        /// Move in direction for distance.
        /// </summary>
        /// <param name="angle">Direction in degrees.</param>
        /// <param name="distance">Distance to move.</param>
        /// <returns></returns>
        public Vector2 Move(double angle, double distance)
        {
            angle += 270;
            var toReturn = Zero;
            toReturn.X = X + (float)(Math.Cos(angle * Math.PI / 180.0) * distance);
            toReturn.Y = Y + (float)(Math.Sin(angle * Math.PI / 180.0) * distance);

            return toReturn;
        }

        /// <summary>
        /// Returns the distance from this vector to another vector.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public float DistanceFrom(Vector2 v)
        => (float)Math.Sqrt(Math.Pow(v.X - X, 2) + Math.Pow(v.Y - Y, 2));

        public float AngleTo(Vector2 v)
        {
            var xDiff = v.X - X;
            var yDiff = v.Y - Y;
            return (float)(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
        }

        public float AngleToRadians(Vector2 v) => AngleTo(v) * (PMath.PI / 180f);

        /// <summary>
        /// Returns a random number.
        /// </summary>
        public static Random VectorRandom { get; set; } = new Random();

        /// <summary>
        /// Returns a random vector within the range given.
        /// </summary>
        /// <param name="minX">The minimum X coordinate of the vector.</param>
        /// <param name="minY">The minimum Y coordinate of the vector.</param>
        /// <param name="maxX">The maximum X coordinate of the vector.</param>
        /// <param name="maxY">The maximum Y coordinate of the vector.</param>
        /// <returns></returns>
        public static Vector2 Random(float minX, float minY, float maxX, float maxY)
        => new Vector2(Extensions.RandomNumberBetween(minX, maxX), Extensions.RandomNumberBetween(minY, maxY));

        #region Addition
        public static Vector2 operator +(Vector2 left, float right)
            => new Vector2(left.X + right, left.Y + right);

        public static Vector2 operator +(float right, Vector2 left)
            => new Vector2(left.X + right, left.Y + right);

        public static Vector2 operator +(Vector2 left, Vector2 right)
            => new Vector2(left.X + right.X, left.Y + right.Y);

        public static Vector2 operator +(Vector2 left, Vector2? right)
            => (right == null) ? left : left + right.Value;
        #endregion

        #region Subtraction
        public static Vector2 operator -(Vector2 left, float right)
            => new Vector2(left.X - right, left.Y - right);

        public static Vector2 operator -(float right, Vector2 left)
            => new Vector2(right - left.X, right - left.Y);

        public static Vector2 operator -(Vector2 left, Vector2 right)
            => new Vector2(left.X - right.X, left.Y - right.Y);

        public static Vector2 operator -(Vector2 vec)
            => new Vector2(-vec.X, -vec.Y);

        public static Vector2 operator -(Vector2 left, Vector2? right)
            => (right == null) ? left : left - right.Value;

        #endregion

        public static Vector2 operator *(Vector2 left, float right)
            => new Vector2(left.X * right, left.Y * right);

        public static Vector2 operator *(float left, Vector2 right)
            => new Vector2(left * right.X, left * right.Y);

        public static Vector2 operator *(Vector2 left, Vector2 right)
            => new Vector2(left.X * right.X, left.Y * right.Y);

        public static Vector2 operator /(Vector2 left, float right)
            => new Vector2(left.X / right, left.Y / right);

        public static Vector2 operator /(float right, Vector2 left)
            => new Vector2(right / left.X, right / left.Y);

        public static Vector2 operator /(Vector2 left, Vector2 right)
            => new Vector2(left.X / right.X, left.Y / right.Y);

        public override string ToString() => "(" + X + "," + Y + ")";
    }
}
