namespace Kira;

public class ColonyManager : Component
{
    private static HashSet<VillagerData> Villagers = new HashSet<VillagerData>();
    public static ColonyManager Instance { get; set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        Instance = this;
    }

    public static VillagerData CreateVillagerData()
    {
        VillagerData villager = new VillagerData();
        Villagers.Add(villager);
        Log.Info(villager);
        return villager;
    }
}