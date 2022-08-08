using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace CustomMission {

    public class CustomGameType : GameType {

        public override bool IsCoreOnlyGameMode => true;
        public override bool IsDevelopment { get { return false; } }

        public static CustomGameType Current => Game.Current.GameType as CustomGameType;
        public string GameType { get { return "Campaign"; } }

        public CustomGameType() { }

        protected override void OnInitialize() {
            IGameStarter gameStarter = new BasicGameStarter();
            this.RegisterGameModels(gameStarter);
            this.LoadGameText(CurrentGame.GameTextManager);
            GameManager.InitializeGameStarter(CurrentGame, gameStarter);
            GameManager.OnGameStart(CurrentGame, gameStarter);
            CurrentGame.SetBasicModels(gameStarter.Models);
            CurrentGame.CreateGameManager();
            GameManager.BeginGameStart(CurrentGame);
            CurrentGame.SetRandomGenerators();
            CurrentGame.InitializeDefaultGameObjects();
            this.LoadGameXml();
            CurrentGame.ObjectManager.UnregisterNonReadyObjects();
            CurrentGame.SetDefaultEquipments(new Dictionary<string, Equipment>());
            CurrentGame.ObjectManager.UnregisterNonReadyObjects();
            GameManager.OnNewCampaignStart(CurrentGame, null);
            GameManager.OnAfterCampaignStart(CurrentGame);
            GameManager.OnGameInitializationFinished(CurrentGame);
        }

        protected override void DoLoadingForGameType(GameTypeLoadingStates gameTypeLoadingState, out GameTypeLoadingStates nextState) {
            nextState = GameTypeLoadingStates.None;

            switch (gameTypeLoadingState) {

                case GameTypeLoadingStates.InitializeFirstStep:
                    base.CurrentGame.Initialize();
                    nextState = GameTypeLoadingStates.WaitSecondStep;
                    return;

                case GameTypeLoadingStates.WaitSecondStep:
                    nextState = GameTypeLoadingStates.LoadVisualsThirdState;
                    return;

                case GameTypeLoadingStates.LoadVisualsThirdState:
                    nextState = GameTypeLoadingStates.PostInitializeFourthState;
                    break;

                case GameTypeLoadingStates.PostInitializeFourthState:
                    break;

                default:
                    return;
            }
        }

        private void RegisterGameModels(IGameStarter basicGameStarter) {
            basicGameStarter.AddModel(new DefaultDamageParticleModel());
            basicGameStarter.AddModel(new DefaultMissionDifficultyModel());
            basicGameStarter.AddModel(new DefaultRidingModel());
            basicGameStarter.AddModel(new DefaultStrikeMagnitudeModel());

            basicGameStarter.AddModel(new TaleWorlds.MountAndBlade.CustomBattle.CustomBattleAutoBlockModel());
            basicGameStarter.AddModel(new CustomBattleAgentStatCalculateModel());
            basicGameStarter.AddModel(new CustomBattleApplyWeatherEffectsModel());
            basicGameStarter.AddModel(new CustomBattleInitializationModel());
            basicGameStarter.AddModel(new CustomBattleMoraleModel());

            basicGameStarter.AddModel(new MultiplayerAgentApplyDamageModel());
            basicGameStarter.AddModel(new MultiplayerAgentDecideKilledOrUnconsciousModel());
        }


        private void LoadXML(string id) {
            ObjectManager.LoadXML(id, IsDevelopment, GameType);
        }

        private void LoadGameText(GameTextManager gameTextManager) {
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/multiplayer_strings.xml");
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/global_strings.xml");
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/module_strings.xml");
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/native_strings.xml");
        }

        private void LoadGameXml() {
            // Native files:
            LoadXML("Monsters");
            LoadXML("SkeletonScales");
            LoadXML("ItemModifiers");
            LoadXML("ItemModifierGroups");
            LoadXML("CraftingPieces");
            LoadXML("WeaponDescriptions");
            LoadXML("CraftingTemplates");
            LoadXML("BodyProperties");
            LoadXML("SkillSets");
            // Sandbox files:
            LoadXML("Items");
            LoadXML("EquipmentRosters");
            LoadXML("NPCCharacters");
            LoadXML("SPCultures");
        }

        protected override void OnRegisterTypes(MBObjectManager objectManager) {
            objectManager.RegisterType<BasicCharacterObject>("NPCCharacter", "NPCCharacters", 43U, true, false);
            objectManager.RegisterType<BasicCultureObject>("Culture", "SPCultures", 17U, true, false);
        }

        protected override void BeforeRegisterTypes(MBObjectManager objectManager) { }

        public override void OnDestroy() { }

        public override void OnStateChanged(GameState oldState) { }

    }
}
