using System.Collections.Generic;

public static class EnemyLimiter
{
    private static readonly Dictionary<string, int> liveCounts = new();

    public static void Register(string typeId)
    {
        if (string.IsNullOrEmpty(typeId)) return;
        if (!liveCounts.ContainsKey(typeId)) liveCounts[typeId] = 0;
        liveCounts[typeId]++;
    }

    public static void Unregister(string typeId)
    {
        if (string.IsNullOrEmpty(typeId)) return;
        if (!liveCounts.ContainsKey(typeId)) return;
        liveCounts[typeId] = System.Math.Max(0, liveCounts[typeId] - 1);
    }

    public static int GetCount(string typeId)
    {
        if (string.IsNullOrEmpty(typeId)) return 0;
        return liveCounts.TryGetValue(typeId, out var v) ? v : 0;
    }

    public static bool UnderLimit(string typeId, int maxConcurrent)
        => GetCount(typeId) < maxConcurrent;
}
