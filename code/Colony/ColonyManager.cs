namespace Kira;

[Category("Kira")]
public sealed class ColonyManager : Component
{
    public List<Villager> Villagers = new List<Villager>();

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

        CreateTestVillagers();

        // Initalize villager array, and colony data
        ColonyData = new ColonyData();
        ColonyData.Villagers = Villagers;
        ColonyData.Population = Villagers.Count;
    }

    private void CreateTestVillagers()
    {
        var v = CreateVillagerData();

        Color vColor = Color.Cyan.WithBlue(0.8f).WithGreen(0.6f);
        Color vColor1 = vColor.WithBlue(0.7f).WithGreen(0.5f).WithRed(0.9f);

        v.PosX = 0;
        v.PosY = 0;
        v.Color = vColor;

        var v1 = CreateVillagerData();
        v1.PosX = -1;
        v1.PosY = 0;
        v1.Color = vColor1;

        var v2 = CreateVillagerData();
        v2.PosX = 1;
        v2.PosY = 0;
        v2.Color = vColor1;

        v.SetDestination(Vector2.Down * 100);
        v1.SetDestination(v.Destination);
        v2.SetDestination(v.Destination);
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