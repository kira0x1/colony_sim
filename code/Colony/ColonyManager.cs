using System;

namespace Kira;

public class ColonyManager : Component
{
    [NonSerialized]
    public List<VillagerData> Villagers = new List<VillagerData>();
    public static ColonyManager Instance { get; set; }

    private const float WorldTickRate = 0.5f;
    private TimeUntil TimeTillWorldTick { get; set; } = 1;

    private delegate void OnWorldTickEvent();
    private event OnWorldTickEvent OnWorldTick;


    protected override void OnAwake()
    {
        base.OnAwake();

        RandomNames.Init();
        Villagers = new List<VillagerData>();
        Instance = this;

        for (int i = 0; i < 9; i++)
        {
            CreateVillagerData();
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (TimeTillWorldTick)
        {
            TimeTillWorldTick = WorldTickRate;
            OnWorldTick?.Invoke();
        }
    }


    public VillagerData CreateVillagerData()
    {
        VillagerData villager = new VillagerData(RandomNames.RandomFirstName, RandomNames.RandomLastName);
        Villagers.Add(villager);
        OnWorldTick += villager.OnWorldTick;
        return villager;
    }
}