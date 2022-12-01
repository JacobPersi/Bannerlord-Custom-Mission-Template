using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace BannerLord_CustomMission {
    class CustomMission {

        public class PlayerSpawner : MissionLogic {
            public override void AfterStart() {
                GameEntity SpawnPoint = Mission.Current.Scene.FindEntityWithTag("sp_play");
                BasicCharacterObject CharacterObject = Game.Current.ObjectManager.GetObject<BasicCharacterObject>("aserai_tribal_horseman");
                MatrixFrame SpawnLocation = (SpawnPoint != null) ? SpawnPoint.GetGlobalFrame() : MatrixFrame.Identity;
                AgentBuildData agentBuildData = new AgentBuildData(CharacterObject).InitialPosition(SpawnLocation.origin);
                Vec2 vec = SpawnLocation.rotation.f.AsVec2;
                vec = vec.Normalized();
                agentBuildData.InitialDirection(vec).Controller(Agent.ControllerType.Player);
                Agent player = Mission.SpawnAgent(agentBuildData, false, 0);
                player.WieldInitialWeapons(Agent.WeaponWieldActionType.InstantAfterPickUp);
            }
        }

        [TaleWorlds.MountAndBlade.MissionManager]
        public class MissionManager {
            [MissionMethod]
            public static Mission OpenSceneEditor() {
                MissionInitializerRecord MissionInitializer = new MissionInitializerRecord("empty_scene") {
                    PlayingInCampaignMode = false,
                    AtmosphereOnCampaign = null,
                };
                InitializeMissionBehaviorsDelegate MissionBehaviorDelegate = (Mission controller) => new MissionBehavior[] {
                    new PlayerSpawner()
                };
                return MissionState.OpenNew("CustomMission", MissionInitializer, MissionBehaviorDelegate);
            }
        }

        [ViewCreatorModule]
        public class ViewCreator {
            [ViewMethod("CustomMission")]
            public static MissionView[] OpenSceneEditor(Mission mission) {
                return new MissionView[] {
                    new MissionMainAgentController() // This is the default player controller view, it will attach to the character spawned above. 
                };
            }
        }
    }
}
