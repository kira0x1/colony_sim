using System;

namespace Kira;

[Category("Kira")]
public sealed class ColonyManager : Component
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
            var v = CreateVillagerData();
            if (i == 1)
            {
                v.PosX = 150;
                v.PosY = -100;
            }
            else if (i == 2)
            {
                v.PosX = -150;
                v.PosY = -100;
            }
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