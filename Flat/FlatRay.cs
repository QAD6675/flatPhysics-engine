using System;
using Microsoft.Xna.Framework;

namespace Flat
{
    public readonly struct FlatRay
    {
        public readonly Vector2 Position;
        public readonly Vector2 Direction;

        public FlatRay(Vector2 position, Vector2 direction)
        {
            this.Position = position;
            this.Direction = direction;
        
        }

        public bool Intersects(in FlatCircle circle, out float distance)
        {
            distance = 0f;

            // TODO: what to do if ray starts inside the circle?
            if(circle.Intersects(this.Position))
            {
                return false;
            }

            // Ensure the ray is pointing "towards" the circle.
            if(FlatMath.Dot(this.Direction, circle.Center - this.Position) < 0)
            {
                return false;
            }

            // "a", "b", and "c" are 3 sides of a triangle.
            //  "c": is the hypotonus and extends from the ray.position to the circle.center.
            //  "b": is the projection of the hypotonus on the ray direction.
            //  "a": is the opposite side of the angle formed by "c" and "b".
            float c = FlatMath.Distance(this.Position, circle.Center);
            float b = FlatMath.Dot(circle.Center - this.Position, this.Direction);
            float a = MathF.Sqrt(c * c - b * b);

            // If "a" is bigger than the radius then no intersection.  Ray will pass off to the side of the circle.
            if(a >= circle.Radius)
            {
                return false;
            }

            // Now calculate the final side of the triangle adjacent to the triangle above. This will allow us to find the distance to intersection.
            float d = MathF.Sqrt(circle.Radius * circle.Radius - a * a);
            distance = b - d;

            return true;
        }



    }
}
