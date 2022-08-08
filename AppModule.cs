using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CustomMission {
    public class AppModule : MBSubModuleBase {

        public override void OnInitialState() {
            MBGameManager.StartNewGame((MBGameManager)new CustomGameManager());
        }
    }
}
