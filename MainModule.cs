using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

public class MainModule : MBSubModuleBase
{
    protected override void OnSubModuleLoad()
    {
    }

    protected override void OnApplicationTick(float dt)
    {
        /* Hacky way of loading our scene */
        if (Input.IsKeyDown(InputKey.F1))
        {
            MBGameManager.StartNewGame((MBGameManager)new CustomGameManager());
        }
    }

}
