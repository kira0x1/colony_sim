namespace Kira;

public sealed class VillagerPawn : Component
{
    private NavMeshAgent agent;
    private PawnController pawnController;
    private VillagerData villagerData;

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

        villagerData = ColonyManager.Instance.CreateVillagerData();

        villagerData.PosX = agent.AgentPosition.x;
        villagerData.PosY = agent.AgentPosition.y;
    }

    protected override void OnUpdate()
    {
        pawnController.Tick();

        villagerData.PosX = agent.AgentPosition.x;
        villagerData.PosY = agent.AgentPosition.y;
    }
}