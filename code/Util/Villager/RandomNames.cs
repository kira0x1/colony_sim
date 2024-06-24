namespace Kira;

using System;

public class NamesData
{
    public List<string> FirstNames { get; set; }
    public List<string> LastNames { get; set; }
}

public class NamesDataRoot
{
    public NamesData Names { get; set; }
}

public static class RandomNames
{
    private static NamesDataRoot Data { get; set; }
    private static NamesData Names { get; set; }

    public static void Init()
    {
        try
        {
            Data = FileSystem.Mounted.ReadJson<NamesDataRoot>("Util/Villager/RandomNames.json");
            Names = Data.Names;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, $"Failed to initalize Names\n{ex.Message}");
        }
    }

    public static string RandomFirstName => Random.Shared.FromList(Names.FirstNames);
    public static string RandomLastName => Random.Shared.FromList(Names.LastNames);
}