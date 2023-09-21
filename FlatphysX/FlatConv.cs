using Microsoft.Xna.Framework;
using Flat;
using FlatPhysics;

namespace FlatPhysX
{
    public static class FlatConv
    {
        public static Vector2 Flat2mono(FlatVector vec)
        {
            return new Vector2(vec.X,vec.Y);
        }
        public static FlatVector Mono2flat(Vector2 vec)
        {
            return new FlatVector(vec.X, vec.Y);
        }
    }
}
