namespace Kira.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

public sealed partial class VillagerUtils
{
    private static readonly List<(int Min, int Max, int Weight)> AgeRanges = new List<(int Min, int Max, int Weight)>
    {
        (0, 20, 10000),
        (20, 40, 50000),
        (40, 60, 10000),
        (60, 80, 5000),
        (80, 100, 1000)
    };

    private static readonly int TotalAgeWeight = AgeRanges.Sum(x => x.Weight);

    public static int GenerateAge()
    {
        var rnd = new Random();

        var randomNum = rnd.Next(TotalAgeWeight);
        foreach ((int min, int max, int weight) in AgeRanges)
        {
            if (randomNum < weight)
                return rnd.Next(min, max);
            randomNum -= weight;
        }

        return 0;
    }
}