using System;

namespace Kira.Utils;

public partial class VillagerUtils
{
    public enum AgeGroup
    {
        Adult,
        Teen,
        Child
    }

    private static readonly List<(int Min, int Max, int Weight)> AdultHeightRanges = new List<(int Min, int Max, int Weight)>
    {
        (140, 147, 3200),
        (147, 155, 135000),
        (155, 163, 2100000),
        (163, 170, 13600000),
        (170, 178, 34000000),
        (178, 185, 34000000),
        (185, 193, 13600000),
        (193, 200, 2100000),
        (200, 208, 135000),
        (208, 216, 3200),
        (216, 224, 28),
        (224, 231, 0)
    };

    private static readonly List<(int Min, int Max, int Weight)> ChildHeightRanges = new List<(int Min, int Max, int Weight)>
    {
        (110, 120, 1350),
        (120, 140, 8100),
        (140, 160, 13500),
        (160, 180, 3200),
        (180, 190, 28),
        (190, 190, 0)
    };

    private static readonly List<(int Min, int Max, int Weight)> TeenHeightRanges = new List<(int Min, int Max, int Weight)>
    {
        (110, 120, 800),
        (120, 140, 5100),
        (140, 160, 13500),
        (160, 180, 9200),
        (180, 190, 5000),
        (190, 190, 0)
    };


    private static Dictionary<AgeGroup, List<(int Min, int Max, int Weight)>> HeightRanges = new()
    {
        { AgeGroup.Adult, AdultHeightRanges },
        { AgeGroup.Teen, TeenHeightRanges },
        { AgeGroup.Child, ChildHeightRanges }
    };

    private static Dictionary<AgeGroup, int> WeightRanges = new()
    {
        { AgeGroup.Adult, AdultHeightRanges.Sum(x => x.Weight) },
        { AgeGroup.Teen, TeenHeightRanges.Sum(x => x.Weight) },
        { AgeGroup.Child, ChildHeightRanges.Sum(x => x.Weight) }
    };

    private static int GenerateHeight(int age)
    {
        var ageGroup = AgeGroup.Child;
        if (age > 12) ageGroup = AgeGroup.Teen;
        if (age > 18) ageGroup = AgeGroup.Adult;

        var height = GenerateHeight(ageGroup);
        return height;
    }

    private static int GenerateHeight(AgeGroup ageGroup = AgeGroup.Adult)
    {
        var rnd = new Random();

        int totalWeight = WeightRanges[ageGroup];
        var ranges = HeightRanges[ageGroup];

        var randomNum = rnd.Next(totalWeight);
        foreach ((int min, int max, int weight) in ranges)
        {
            if (randomNum < weight)
                return rnd.Next(min, max);
            randomNum -= weight;
        }

        return 0;
    }
}