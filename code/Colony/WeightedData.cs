using System;

namespace Kira.Utils;

public readonly struct WeightedData
{
    public readonly (int Min, int Max, int Weight)[] Ranges;
    public readonly int TotalWeight;

    public WeightedData((int Min, int Max, int Weight)[] ranges)
    {
        Ranges = ranges;
        TotalWeight = Ranges.Sum(x => x.Weight);
    }

    public int GenerateRandom()
    {
        var rnd = new Random();

        var randomNum = rnd.Next(TotalWeight);

        foreach ((int min, int max, int weight) in Ranges)
        {
            if (randomNum < weight)
                return rnd.Next(min, max);
            randomNum -= weight;
        }

        return 0;
    }
}