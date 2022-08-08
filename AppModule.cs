using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CustomMission {
    public class AppModule : MBSubModuleBase {

        public override void OnInitialState() {
            // This project assumes you want to create your own game outside of the standard campaign environment. 
            // To do this, we create a custom MBGameManager called CustomGameManager...
            MBGameManager.StartNewGame((MBGameManager)new CustomGameManager());
        }
    }
}
