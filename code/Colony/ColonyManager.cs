using System;
using System.Text.Json.Nodes;

namespace Kira;

public class ColonyManager : Component
{
    public HashSet<VillagerData> Villagers { get; private set; } = new HashSet<VillagerData>();
    public static ColonyManager Instance { get; set; }

    private const float WorldTickRate = 0.2f;
    private TimeUntil TimeTillWorldTick { get; set; } = 0;

    private delegate void OnWorldTickEvent();
    private OnWorldTickEvent OnWorldTick;

    public override int ComponentVersion => 1;

    [JsonUpgrader(typeof(ColonyManager), 1)]
    private static void WorldTickPropertyUpgrader(JsonObject json)
    {
        json.Remove("OnWorldTick", out var newNode);
        json["OnWorldTick"] = newNode;
    }

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
        VillagerData villager = new VillagerData();
        Villagers.Add(villager);
        OnWorldTick += villager.OnWorldTick;
        return villager;
    }
}