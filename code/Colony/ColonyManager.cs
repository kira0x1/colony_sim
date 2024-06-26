using System;

namespace Kira;

[Category("Kira")]
public sealed class ColonyManager : Component
{
    [NonSerialized]
    public List<Villager> Villagers = new List<Villager>();
    public static ColonyManager Instance { get; set; }

    private const float WorldTickRate = 0.5f;
    private TimeUntil TimeTillWorldTick { get; set; } = 1;

    private delegate void OnWorldTickEvent();
    private event OnWorldTickEvent OnWorldTick;

    public ColonyData ColonyData { get; set; } = new ColonyData();

    protected override void OnAwake()
    {
        base.OnAwake();

        RandomNames.Init();
        Villagers = new List<Villager>();
        Instance = this;

        for (int i = 0; i < 9; i++)
        {
            var v = CreateVillagerData();
            if (i == 1)
            {
                v.PosX = -2.4f;
                v.PosY = -0.5f;
            }
            else if (i == 2)
            {
                v.PosX = 2.4f;
                v.PosY = 0.5f;
            }
        }

        ColonyData = new ColonyData();
        ColonyData.Villagers = Villagers;
        ColonyData.Population = Villagers.Count;
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

    public Villager CreateVillagerData()
    {
        Villager villager = new Villager(RandomNames.RandomFirstName, RandomNames.RandomLastName);
        Villagers.Add(villager);
        OnWorldTick += villager.OnWorldTick;
        return villager;
    }
}

public class ColonyData
{
    public string ColonyName { get; set; } = "A Colony";
    public int Scrap { get; set; } = 50;
    public int Population { get; set; }
    public List<Villager> Villagers { get; set; }
}