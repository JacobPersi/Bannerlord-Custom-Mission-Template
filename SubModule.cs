using TaleWorlds.MountAndBlade;

namespace BannerLord_CustomMission {
    public class SubModule : MBSubModuleBase {

        public override void OnInitialState() {
            if (MBGameManager.Current != null) {
                MBGameManager.EndGame();
            }
            MBGameManager.StartNewGame(new CustomGame.GameLoader());
        }
    }
}