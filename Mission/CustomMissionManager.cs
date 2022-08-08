using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.View.Missions;

namespace CustomMission {

    [MissionManager]
    public class CustomMissionManager {

        [MissionManager]
        public static Mission OpenSceneEditor() {
            return MissionState.OpenNew("CustomMission", 
                new MissionInitializerRecord("empty_scene") {
                    PlayingInCampaignMode = false,
                    AtmosphereOnCampaign = null,
                }, 
                (Mission controller) => new MissionBehavior[] {
                    new CustomMissionLogic() // Right now this just creates a character for us to possess.
                });
        }
    }

    [ViewCreatorModule]
    public class MissionViewCreator {

        [ViewMethod("CustomMission")]
        public static MissionView[] OpenSceneEditor(Mission mission) {
            return new MissionView[] {
                new MissionMainAgentController() // This is the default player controller, it will attach to the character spawned above. 
            };
        }
    }
}
