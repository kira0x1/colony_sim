namespace Kira;

using System;

public partial class Villager
{
    private Vector2 destination;

    public Vector2 Destination
    {
        get => destination;
        private set => destination = value;
    }

    public TimeUntil TimeUntilNextRoam { get; private set; }

    public float Distance { get; set; }
    private float RandomWaitTime { get; set; }
    private const float RoamRadius = 10;
    public bool HasReachedDestination { get; set; }
    public TimeSince SinceReachedDestination = 0f;
    public Vector2 StartPoint { get; set; }
    public Vector2 Direction { get; set; }
    private Vector2 Velocity { get; set; }

    public void SetDestination(Vector2 destination)
    {
        this.Destination = destination;
    }

    public void Init()
    {
        StartPoint = Vector2.Zero;
        Destination = Vector2.One * 10;
    }

    private void MoveToDestination()
    {
        Direction = (Destination - Pos).Normal;
        Velocity = Direction * Speed * 5;
    }

    public void LogDir()
    {
        UpdateMove();

        Log.Info(" ");
        Log.Info($"Position: {Pos}");
        Log.Info($"Destination: {Destination}");
        Log.Info($"Velocity: {Velocity}");
        Log.Info($"Direction: {Direction}");
        Log.Info($"Distance: {Distance}");
        Log.Info($"HasReached: {HasReachedDestination}");
    }

    public void UpdateMove()
    {
        // We're done waiting around, and time to pick a new destination and go to it 
        if (HasReachedDestination && SinceReachedDestination >= RandomWaitTime)
        {
            // Destination = RandomPoint();
            HasReachedDestination = false;
            return;
        }

        Distance = Vector2.DistanceBetween(Destination, Pos);

        // We reached our destination
        if (Distance < 1f && !HasReachedDestination)
        {
            HasReachedDestination = true;
            // RandomWaitTime = Random.Shared.Float(0.1f, 1);
            SinceReachedDestination = 0f;
            return;
        }

        // Move Unit
        MoveToDestination();
    }

    // ReSharper disable once UnusedMember.Local
    private Vector2 RandomPoint()
    {
        return StartPoint + new Vector2(Random.Shared.Float(-RoamRadius, RoamRadius), Random.Shared.Float(-RoamRadius, RoamRadius));
    }
}