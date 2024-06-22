namespace Kira.Utils;

public partial class VillagerUtils
{
	public enum AgeGroup
	{
		Adult,
		Teen,
		Child
	}

	private static WeightedData ChildHeightRanges = new WeightedData(new[]
	{
		(110, 120, 1350), (120, 140, 8100), (140, 160, 13500), (160, 180, 3200), (180, 190, 28), (190, 190, 0)
	});

	private static WeightedData TeenHeightRanges = new WeightedData(new[]
	{
		(110, 120, 800), (120, 140, 5100), (140, 160, 13500), (160, 180, 9200), (180, 190, 5000), (190, 190, 0)
	});
	private static WeightedData AdultHeightRanges = new WeightedData(new[]
	{
		(140, 147, 3200), (147, 155, 135000), (155, 163, 2100000), (163, 170, 13600000), (170, 178, 34000000), (178, 185, 34000000), (185, 193, 13600000), (193, 200, 2100000),
		(200, 208, 135000), (208, 216, 3200), (216, 224, 28), (224, 231, 0)
	});

	private static Dictionary<AgeGroup, WeightedData> HeightRanges = new()
	{
		{ AgeGroup.Child, ChildHeightRanges }, { AgeGroup.Teen, TeenHeightRanges }, { AgeGroup.Adult, AdultHeightRanges }
	};

	private static int GenerateHeight( int age )
	{
		AgeGroup ageGroup = AgeGroup.Child;
		if (age > 12) ageGroup = AgeGroup.Teen;
		if (age > 18) ageGroup = AgeGroup.Adult;

		var height = GenerateHeight(ageGroup);
		return height;
	}

	private static int GenerateHeight( AgeGroup ageGroup = AgeGroup.Adult )
	{
		return HeightRanges[ageGroup].GenerateRandom();
	}
}
