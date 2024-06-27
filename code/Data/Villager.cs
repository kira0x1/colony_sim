namespace Kira;

using System;

public partial class Villager
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
    public int Age { get; set; }


    public Vector2 Pos { get; set; }
    public bool IsDead => Condition == VillagerCondition.Dead;

    // Stats
    public int Hunger { get; set; }
    public int Thirst { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Speed { get; set; }
    public int Strength { get; set; }

    public StatusEffectsFlags StatusEffects { get; set; } = StatusEffectsFlags.None;
    public VillagerCondition Condition { get; set; } = VillagerCondition.Normal;

    /// <summary>
    /// How many world ticks it takes for hunger to decrease
    /// </summary>
    private const int HungerTickRate = 6;

    /// <summary>
    /// How many world ticks it takes for Thirst to decrease
    /// </summary>
    private const int ThirstTickRate = 4;

    /// <summary>
    /// How many world ticks it takes for Health to decrease because of either hunger or thirst
    /// </summary>
    private const int HungerOrThirstDamageTickRate = 3;


    private int CurHungerTicks = 0;
    private int CurThirstTicks = 0;
    private int CurHungerOrThirstDamageTicks = 0;

    /// <summary>
    /// Height in cm
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Weight in kg
    /// </summary>
    public int Weight { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public float PosX
    {
        get => Pos.x;
        set => Pos = Pos.WithX(value);
    }

    public float PosY
    {
        get => Pos.y;
        set => Pos = Pos.WithY(value);
    }

    public Villager(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;

        Age = Random.Shared.Int(10, 60);
        Weight = Random.Shared.Int(50, 120);

        Height = Utils.VillagerUtils.RandomHeight(Age);

        // Hunger = Random.Shared.Int(0, 10);
        // Thirst = Random.Shared.Int(1, 5);
        Hunger = 100;
        Thirst = 100;

        MaxHealth = 100;
        Health = 100;

        Strength = Random.Shared.Int(10, 30);
        Speed = Random.Shared.Int(10, 30);
        Init();

        // Age = VillagerUtils.RandomAge;
    }

    public void OnWorldTick()
    {
        if (IsDead) return;

        UpdateMove();

        if (Hunger > 0 && CurHungerTicks >= HungerTickRate)
        {
            Hunger--;
            CurHungerTicks = 0;
        }
        else
        {
            CurHungerTicks++;
        }

        if (Thirst > 0 && CurThirstTicks >= ThirstTickRate)
        {
            Thirst--;
            CurThirstTicks = 0;
        }
        else
        {
            CurThirstTicks++;
        }


        if (Thirst > 0 && Hunger > 0)
        {
            CurHungerOrThirstDamageTicks = 0;
            return;
        }


        CurHungerOrThirstDamageTicks++;
        if (CurHungerOrThirstDamageTicks >= HungerOrThirstDamageTickRate)
        {
            int damage = 0;
            if (Thirst <= 0) damage++;
            if (Hunger <= 0) damage++;
            CurHungerOrThirstDamageTicks = 0;

            DealDamage(damage);
        }
    }

    public void DealDamage(int damage, string reason = "")
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Condition = VillagerCondition.Dead;
        }
    }

    public override string ToString()
    {
        return $"Name: {FirstName} {LastName}, Age: {Age}, Height: {Height}cm, Weight: {Weight}kg";
    }

    public int BuildHash() => System.HashCode.Combine(Hunger, Thirst, Health, Pos);
}