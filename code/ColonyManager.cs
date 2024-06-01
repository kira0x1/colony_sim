namespace Kira;

public class ColonyManager : Component
{
    public HashSet<VillagerData> Villagers { get; private set; } = new HashSet<VillagerData>();
    public static ColonyManager Instance { get; set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        Villagers = new HashSet<VillagerData>();
        Instance = this;

        for (int i = 0; i < 9; i++)
        {
            CreateVillagerData();
        }
    }

    public VillagerData CreateVillagerData()
    {
        VillagerData villager = new VillagerData();
        Villagers.Add(villager);
        Log.Info(villager);
        return villager;
    }
}