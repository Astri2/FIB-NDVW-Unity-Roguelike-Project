using System.Collections.Generic;

public static class WeightedRandom
{
    public static T Choose<T>(List<(T item, float weight)> options)
    {
        float total = 0f;

        foreach (var o in options)
            total += o.weight;

        float r = UnityEngine.Random.value * total;

        foreach (var o in options)
        {
            if (r < o.weight)
                return o.item;
            r -= o.weight;
        }

        return default; // Should never happen if weights > 0
    }
}
