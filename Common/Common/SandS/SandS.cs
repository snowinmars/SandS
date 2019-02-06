using System;

namespace Common.SandS
{
    public static class SandS
    {
        public static Random Random { get; } = new Random();
        public static double SmallDouble { get; } = 0.000001d;
        public static float SmallFloat { get; } = 0.000001f;
        public static decimal SmallDecimal { get; } = 0.000001m;
    }
}
