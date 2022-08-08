using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CustomMission {
    public class CustomMissionLogic : MissionLogic {

        public override void AfterStart() {
            BasicCharacterObject @object = Game.Current.ObjectManager.GetObject<BasicCharacterObject>("aserai_tribal_horseman");
            GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("sp_play");
            MatrixFrame matrixFrame = (gameEntity != null) ? gameEntity.GetGlobalFrame() : MatrixFrame.Identity;
            AgentBuildData agentBuildData = new AgentBuildData(@object);
            AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(matrixFrame.origin);
            Vec2 vec = matrixFrame.rotation.f.AsVec2;
            vec = vec.Normalized();
            agentBuildData2.InitialDirection(vec).Controller(Agent.ControllerType.Player);
            Mission.SpawnAgent(agentBuildData, false, 0).WieldInitialWeapons(Agent.WeaponWieldActionType.InstantAfterPickUp);
        }

    }
}