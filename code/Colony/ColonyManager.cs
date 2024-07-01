namespace Kira;

[Category("Kira")]
public sealed class ColonyManager : Component
{
    public List<Villager> Villagers = new List<Villager>();

    public bool HasInitalized { get; set; }
    public static ColonyManager Instance { get; set; }

    private const float WorldTickRate = 0.5f;
    private TimeUntil TimeTillWorldTick { get; set; } = 1;

    public delegate void OnWorldTickEvent();
    public event OnWorldTickEvent OnWorldTick;

    public ColonyData ColonyData { get; set; } = new ColonyData();

    protected override void OnAwake()
    {
        base.OnAwake();

        RandomNames.Init();
        Instance = this;

        for (int i = 0; i < 3; i++)
        {
            var v = CreateVillagerData();

            if (i == 0)
            {
                v.PosX = 0;
                v.PosY = 0;
                v.Color = Color.Cyan;
            }
            else if (i == 1)
            {
                v.PosX = 1f;
                v.PosY = -2f;
                v.Color = Color.Red;
            }
            else if (i == 2)
            {
                v.PosX = 1;
                v.PosY = 2;
                v.Color = Color.Green;
            }
        }

        ColonyData = new ColonyData();
        ColonyData.Villagers = Villagers;
        ColonyData.Population = Villagers.Count;
        HasInitalized = true;
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
        Villager villager = new Villager(RandomNames.RandomFirstName, RandomNames.RandomLastName, Villagers.Count);
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