namespace Kira;

using System;

public partial class Villager
{
    public Vector2 Destination { get; private set; }
    public TimeUntil TimeUntilNextRoam { get; private set; }

    public float Distance { get; set; }
    private float RandomWaitTime { get; set; }
    private readonly float roamRadius;
    public bool HasReachedDestination { get; set; }
    public TimeSince SinceReachedDestination = 0f;
    public Vector2 StartPoint { get; set; }

    public void Init()
    {
        StartPoint = RandomPoint();
        Destination = StartPoint;
    }

    private void MoveToDestination()
    {
        Vector2 pos = Vector2.Lerp(Pos, Destination, Speed * Time.Delta, true);
        Pos = pos;
    }

    public void UpdateMove()
    {
        // We're done waiting around, and time to pick a new destination and go to it 
        if (HasReachedDestination && SinceReachedDestination >= RandomWaitTime)
        {
            Destination = RandomPoint();
            HasReachedDestination = false;
            return;
        }

        Distance = Vector2.DistanceBetween(Destination, Pos);

        // We reached our destination
        if (Distance < 3f && !HasReachedDestination)
        {
            HasReachedDestination = true;
            // RandomWaitTime = Random.Shared.Float(0.1f, 1);
            SinceReachedDestination = 0f;
            return;
        }

        // Move Unit
        MoveToDestination();
    }

    private Vector2 RandomPoint()
    {
        return StartPoint + new Vector2(Random.Shared.Float(-roamRadius, roamRadius), Random.Shared.Float(-roamRadius, roamRadius));
    }
}