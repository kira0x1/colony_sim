namespace Kira;

public sealed class VillagerPawn : Component
{
    private NavMeshAgent agent;
    private PawnController pawnController;
    private Villager villager;

    protected override void OnAwake()
    {
        base.OnAwake();
        agent = Components.Get<NavMeshAgent>();
    }

    protected override void OnStart()
    {
        base.OnStart();

        agent.Separation = 0.2f;
        agent.UpdateRotation = true;
        agent.Acceleration = 90f;
        agent.MaxSpeed = 80f;
        pawnController = new PawnController(agent, roamRadius: 150f);

        villager = ColonyManager.Instance.Villagers[0];

        villager.PosX = agent.AgentPosition.x;
        villager.PosY = agent.AgentPosition.y;
    }

    protected override void OnUpdate()
    {
        pawnController.Tick();

        villager.PosX = agent.AgentPosition.x;
        villager.PosY = agent.AgentPosition.y;
    }
}