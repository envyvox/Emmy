using System;

namespace Emmy.Data.Enums
{
    public enum CubeType : byte
    {
        D6 = 1,
        D8 = 2,
        D12 = 3
    }

    public static class CubeHelper
    {
        private static readonly Random Random = new();

        private static int MaxDrop(this CubeType cube) => cube switch
        {
            CubeType.D6 => 6,
            CubeType.D8 => 8,
            CubeType.D12 => 12,
            _ => throw new ArgumentOutOfRangeException(nameof(cube), cube, null)
        };

        public static string EmoteName(this CubeType cube, uint number)
        {
            if (number > cube.MaxDrop()) throw new ArgumentOutOfRangeException(nameof(cube), cube, null);

            return "Cube" + cube + number;
        }

        public static uint DropCube(this CubeType cube)
        {
            return (uint) Random.Next(1, cube.MaxDrop() + 1);
        }
    }
}