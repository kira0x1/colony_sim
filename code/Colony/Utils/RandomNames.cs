namespace Kira;

using System;

public record NamesData(List<string> FirstNames, List<string> LastNames);
public record NamesDataRoot(NamesData Names);

public static class RandomNames
{
    private static readonly NamesData Data = FileSystem.Mounted.ReadJson<NamesDataRoot>("Colony/Utils/RandomNames.json").Names;
    public static string RandomFirstName => Random.Shared.FromList(Data.FirstNames);
    public static string RandomLastName => Random.Shared.FromList(Data.LastNames);
}