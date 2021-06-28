using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;

namespace BannerNet.Core
{

    class CustomSkillList : SkillList
    {

        internal CustomSkillList()
        {
        }

        public override IEnumerable<SkillObject> GetSkillList()
        {
            yield return DefaultSkills.OneHanded;
            yield return DefaultSkills.TwoHanded;
            yield return DefaultSkills.Polearm;
            yield return DefaultSkills.Bow;
            yield return DefaultSkills.Crossbow;
            yield return DefaultSkills.Throwing;
            yield return DefaultSkills.Riding;
            yield return DefaultSkills.Athletics;
            yield return DefaultSkills.Tactics;
            yield return DefaultSkills.Scouting;
            yield return DefaultSkills.Roguery;
            yield return DefaultSkills.Crafting;
            yield return DefaultSkills.Charm;
            yield return DefaultSkills.Trade;
            yield return DefaultSkills.Leadership;
            yield return DefaultSkills.Steward;
            yield return DefaultSkills.Medicine;
            yield return DefaultSkills.Engineering;
            yield break;
        }
    }

    public class CustomGameType : GameType
    {
        public override bool IsCoreOnlyGameMode
        {
            get
            {
                return true;
            }
        }

        public CustomGameType()
        {
        }

        protected override void OnInitialize()
        {
            Game currentGame = base.CurrentGame;
            currentGame.FirstInitialize(false);
            
            GameTextManager gameTextManager = currentGame.GameTextManager;
            this.InitializeGameTexts(gameTextManager);

            IGameStarter gameStarter = new TaleWorlds.MountAndBlade.BasicGameStarter();
            this.InitializeGameModels(gameStarter);
            base.GameManager.OnGameStart(base.CurrentGame, gameStarter);

            MBObjectManager objectManager = currentGame.ObjectManager;
            currentGame.SecondInitialize(gameStarter.Models);
            currentGame.CreateGameManager();
            base.GameManager.BeginGameStart(base.CurrentGame);

            base.CurrentGame.ThirdInitialize();

            currentGame.CreateObjects();
            currentGame.InitializeDefaultGameObjects();
            this.LoadXMLFiles();

            objectManager.ClearEmptyObjects();
            currentGame.SetDefaultEquipments(new Dictionary<string, Equipment>());
            currentGame.CreateLists();

            objectManager.ClearEmptyObjects();
            base.GameManager.OnCampaignStart(base.CurrentGame, null);
            base.GameManager.OnAfterCampaignStart(base.CurrentGame);
            base.GameManager.OnGameInitializationFinished(base.CurrentGame);
        }

        private void InitializeGameTexts(GameTextManager gameTextManager)
        {
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/multiplayer_strings.xml");
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/global_strings.xml");
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/module_strings.xml");
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/native_strings.xml");
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("StoryMode") + "ModuleData/module_strings.xml");
            //gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("XYZTrainer") + "ModuleData/module_strings.xml");
        }

        private void InitializeGameModels(IGameStarter basicGameStarter)
        {
            basicGameStarter.AddModel(new TaleWorlds.MountAndBlade.MultiplayerAgentDecideKilledOrUnconsciousModel());
            basicGameStarter.AddModel(new TaleWorlds.MountAndBlade.CustomBattleAgentStatCalculateModel());
            basicGameStarter.AddModel(new TaleWorlds.MountAndBlade.CustomBattleApplyWeatherEffectsModel());
            //basicGameStarter.AddModel(new TaleWorlds.MountAndBlade.CustomBattle.CustomBattleAutoBlockModel());
            basicGameStarter.AddModel(new TaleWorlds.MountAndBlade.MultiplayerAgentApplyDamageModel());
            basicGameStarter.AddModel(new DefaultRidingModel());
            basicGameStarter.AddModel(new DefaultStrikeMagnitudeModel());
            basicGameStarter.AddModel(new CustomSkillList());
            basicGameStarter.AddModel(new TaleWorlds.MountAndBlade.CustomBattleMoraleModel());
        }
        private void LoadXML(string id)
        {
            bool isDevelopment = false;
            string gameType = "Campaign";
            base.ObjectManager.LoadXML(id, isDevelopment, gameType);
        }
        private void LoadXMLFiles()
        {
            foreach (MbObjectXmlInformation mbObjectXmlInformation in XmlResource.XmlInformationList)
            {
                MBDebug.Print("XYZ: All found XMLs:");
                MbObjectXmlInformation info = mbObjectXmlInformation;
                MBDebug.Print(String.Format("|{0} | {1} | {2} | {3} |",
                    info.Id, info.Name, info.ModuleName, String.Join(",", info.GameTypesIncluded)));
            }

            MBDebug.Print("XYZ: Loading Monsters, SkeletonScales, ItemModifiers, ItemModifierGroups");
            this.LoadXML("Monsters");
            this.LoadXML("SkeletonScales");
            this.LoadXML("ItemModifiers");
            this.LoadXML("ItemModifierGroups");
            MBDebug.Print("XYZ: Loading CraftingPieces");
            this.LoadXML("CraftingPieces");
            MBDebug.Print("XYZ: Loading CraftingTemplates");
            this.LoadXML("CraftingTemplates");
            MBDebug.Print("XYZ: Loading BodyProperties");
            this.LoadXML("BodyProperties");
            MBDebug.Print("XYZ: Loading SkillSets");
            this.LoadXML("SkillSets");
            MBDebug.Print("XYZ: Loading Items");
            this.LoadXML("Items");
            MBDebug.Print("XYZ: Loading EquipmentRosters");
            this.LoadXML("EquipmentRosters");
            MBDebug.Print("XYZ: Loading NPCCharacters");
            this.LoadXML("NPCCharacters");
            MBDebug.Print("XYZ: Loading SPCultures");
            this.LoadXML("SPCultures");
        }

        protected override void BeforeRegisterTypes(MBObjectManager objectManager)
        {
        }

        protected override void OnRegisterTypes(MBObjectManager objectManager)
        {
            objectManager.RegisterType<BasicCharacterObject>("NPCCharacter", "NPCCharacters", 43U, true);
            objectManager.RegisterType<BasicCultureObject>("Culture", "SPCultures", 17U, true);
        }

        protected override void DoLoadingForGameType(GameTypeLoadingStates gameTypeLoadingState, out GameTypeLoadingStates nextState)
        {
            nextState = GameTypeLoadingStates.None;
            switch (gameTypeLoadingState)
            {
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

        public override void OnDestroy()
        {
        }

        public override void OnStateChanged(GameState oldState)
        {
        }

    }

}
