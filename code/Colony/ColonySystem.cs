namespace Kira;

using UI;

public class ColonySystem : GameObjectSystem
{
    public ColonySystem(Scene scene) : base(scene)
    {
        Listen(Stage.FinishUpdate, 10, UpdatePawns, "UpdatingPawns");
    }

    private bool MapInitalized { get; set; }
    private bool ColonyInitalized { get; set; }

    private void UpdatePawns()
    {
        ColonyManager cm = ColonyManager.Instance;
        UnitMap umap = Scene.Components.GetAll<UnitMap>().FirstOrDefault();

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

        if (!umap.IsValid())
        {
            if (!MapInitalized)
            {
                Log.Warning("UnitMap is not valid");
                MapInitalized = true;
            }

            return;
        }

        MapInitalized = false;
    }
}