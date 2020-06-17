using System.Collections.Generic;

public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        var count = list.Count;

        while (count > 1)
        {
            count--;

            var randomValue = StaticRandom.Instance.Next(count + 1);

            var value = list[randomValue];
            list[randomValue] = list[count];
            list[count] = value;
        }
    }
}