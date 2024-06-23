namespace Kira;

using UI;

public class ColonySystem : GameObjectSystem
{
    public ColonySystem(Scene scene) : base(scene)
    {
        Listen(Stage.FinishUpdate, 10, UpdatePawns, "UpdatingPawns");
    }

    private bool ColonyInitalized { get; set; }

    private void UpdatePawns()
    {
        ColonyManager cm = ColonyManager.Instance;

        if (!cm.IsValid())
        {
            if (!ColonyInitalized)
            {
                Log.Warning("Colony Manager is not valid");
                ColonyInitalized = true;
            }

            return;
        }

        ColonyInitalized = false;
    }
}