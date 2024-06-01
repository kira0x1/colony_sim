namespace Kira;

using System;

public class PawnController
{
    public Vector3 StartPoint { get; set; }
    public Vector3 Destination { get; set; }

    public TimeSince SinceReachedDestination = 0f;

    public float Distance { get; set; }
    public bool HasReachedDestination { get; set; }

    private NavMeshAgent agent { get; set; }
    private float RandomWaitTime { get; set; }
    private float RoamRadius { get; set; }

    public PawnController(NavMeshAgent agent, Vector3 startPos = new Vector3(), float roamRadius = 30f)
    {
        if (!agent.IsValid())
        {
            Log.Warning("Agent is null");
            return;
        }

        this.agent = agent;
        this.RoamRadius = roamRadius;

        StartPoint = RandomPoint();
        Destination = StartPoint;
        agent.MoveTo(Destination);
    }

    private Vector3 RandomPoint()
    {
        return StartPoint + new Vector3(Random.Shared.Float(-RoamRadius, RoamRadius), Random.Shared.Float(-RoamRadius, RoamRadius), 0f);
    }

    public void Tick()
    {
        Gizmo.Draw.SolidSphere(agent.GameObject.Scene.Transform.World.PointToLocal(Destination), 6f);

        if (HasReachedDestination && SinceReachedDestination >= RandomWaitTime)
        {
            Destination = RandomPoint();
            agent.MoveTo(Destination);
            HasReachedDestination = false;
            return;
        }

        Distance = Vector3.DistanceBetween(Destination, agent.AgentPosition);

        if (Distance < 3f && !HasReachedDestination)
        {
            HasReachedDestination = true;
            RandomWaitTime = Random.Shared.Float(8) + 1f;
            SinceReachedDestination = 0f;

            // Log.Info($"Wait Time: {RandomWaitTime}");
        }
    }
}