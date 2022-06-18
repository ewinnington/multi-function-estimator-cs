// https://devblogs.microsoft.com/pfxteam/getting-random-numbers-in-a-thread-safe-way/

public static class MultiThreadRandom
{
    private static Random _global = new Random();
    
    #pragma warning disable CS8618 // Non-nullable field is uninitialized.
    [ThreadStatic]
    private static Random _local;

    public static double NextDouble()
    {
        Random inst = _local;
        if (inst == null)
        {
            int seed;
            lock (_global) seed = _global.Next();
            _local = inst = new Random(seed);
        }
        return inst.NextDouble();
    }
}