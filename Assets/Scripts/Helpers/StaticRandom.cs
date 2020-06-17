using System;
using System.Threading;

public static class StaticRandom
{
    private static int Seed;

    private static readonly ThreadLocal<Random> ThreadLocal = new ThreadLocal<Random>
        (() => new Random(Interlocked.Increment(ref Seed)));

    static StaticRandom()
    {
        Seed = Environment.TickCount;
    }

    public static Random Instance => ThreadLocal.Value;
}