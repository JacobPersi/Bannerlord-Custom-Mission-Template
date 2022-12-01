using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;

namespace BannerLord_CustomMission {
    public class CustomGame  {

        public class GameLoader : MBGameManager {

            protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep) {
                nextStep = GameManagerLoadingSteps.None;
                switch (gameManagerLoadingStep) {

                    case GameManagerLoadingSteps.PreInitializeZerothStep:
                        MBGameManager.LoadModuleData(false);
                        MBGlobals.InitializeReferences();
                        /* Load our custom game type! */
                        Game.CreateGame(new GameType(), this).DoLoading();
                        nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
                        return;

                    case GameManagerLoadingSteps.FirstInitializeFirstStep:
                        bool loadingComplete = true;
                        foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
                            loadingComplete &= mbsubModuleBase.DoLoading(Game.Current);
                        nextStep = (loadingComplete ? GameManagerLoadingSteps.WaitSecondStep : GameManagerLoadingSteps.FirstInitializeFirstStep);
                        return;

                    case GameManagerLoadingSteps.WaitSecondStep:
                        MBGameManager.StartNewGame();
                        nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
                        return;

                    case GameManagerLoadingSteps.SecondInitializeThirdState:
                        nextStep = (Game.Current.DoLoading() ? GameManagerLoadingSteps.PostInitializeFourthState : GameManagerLoadingSteps.SecondInitializeThirdState);
                        return;

                    case GameManagerLoadingSteps.PostInitializeFourthState:
                        nextStep = GameManagerLoadingSteps.FinishLoadingFifthStep;
                        return;

                    case GameManagerLoadingSteps.FinishLoadingFifthStep:
                        nextStep = GameManagerLoadingSteps.None;
                        return;

                    default:
                        return;
                }
            }

            public override void OnLoadFinished() {
                base.OnLoadFinished();
                /* Push our GameState! */
                GameStateManager.Current.CleanAndPushState(GameStateManager.Current.CreateState<GameState>());
            }
        }

        public class GameType : TaleWorlds.Core.GameType {

            public override bool IsCoreOnlyGameMode => true;
            public override bool IsDevelopment => false;
            public string TypeString => "Campaign";

            public GameType() { }

            protected override void OnInitialize() {
                IGameStarter gameStarter = new BasicGameStarter();
                this.RegisterGameModels(gameStarter);
                this.LoadGameText(CurrentGame.GameTextManager);
                GameManager.InitializeGameStarter(CurrentGame, gameStarter);
                GameManager.OnGameStart(CurrentGame, gameStarter);
                CurrentGame.SetBasicModels(gameStarter.Models);
                CurrentGame.CreateGameManager();
                GameManager.BeginGameStart(CurrentGame);
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
                basicGameStarter.AddModel(new DefaultAgentApplyDamageModel());
                basicGameStarter.AddModel(new DefaultDamageParticleModel());
                basicGameStarter.AddModel(new DefaultMissionDifficultyModel());
                basicGameStarter.AddModel(new DefaultRidingModel());
                basicGameStarter.AddModel(new DefaultStrikeMagnitudeModel());
                basicGameStarter.AddModel(new SandBox.GameComponents.SandboxAutoBlockModel());
                basicGameStarter.AddModel(new CustomBattleAgentStatCalculateModel());
                basicGameStarter.AddModel(new CustomBattleApplyWeatherEffectsModel());
                basicGameStarter.AddModel(new CustomBattleInitializationModel());
                basicGameStarter.AddModel(new CustomBattleMoraleModel());
            }

            private void LoadXML(string id) {
                ObjectManager.LoadXML(id, IsDevelopment, TypeString);
            }

            private void LoadGameText(GameTextManager gameTextManager) {
                gameTextManager.LoadGameTexts();
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

            public override void OnStateChanged(TaleWorlds.Core.GameState oldState) { }

            protected override void BeforeRegisterTypes(MBObjectManager objectManager) { }

            public override void OnDestroy() { }
        }

        public class GameState : TaleWorlds.Core.GameState {

            private Color _debugGreen;
            public Color DebugGreen {
                get {
                    if (_debugGreen == null) {
                        _debugGreen = Color.ConvertStringToColor("#527A2D");
                    }
                    return this._debugGreen;
                }
            }
            protected override void OnTick(float dt) {
                if (Input.IsKeyReleased(InputKey.F1)) {
                    CustomMission.MissionManager.OpenSceneEditor();
                }
            }
        }

        [GameStateScreen(typeof(BannerLord_CustomMission.CustomGame.GameState))]
        public class CustomGameScene : ScreenBase, IGameStateListener {

            private TaleWorlds.Core.GameState State;
            private Scene Scene;
            private Camera Camera;
            private SceneLayer SceneLayer;

            public CustomGameScene(TaleWorlds.Core.GameState state) {
                this.State = state;
            }

            protected override void OnInitialize() {
                base.OnInitialize();
                this.SceneLayer = new SceneLayer("SceneLayer");
                base.AddLayer(this.SceneLayer);
                this.SceneLayer.SceneView.SetResolutionScaling(true);
                this.Camera = Camera.CreateCamera();
                Common.MemoryCleanupGC();
            }

            protected override void OnFinalize() {
                base.OnFinalize();
            }

            protected override void OnActivate() {
                this.Scene = Scene.CreateNewScene();
                this.Scene.SetPlaySoundEventsAfterReadyToRender(true);
                /* 
                 * Note: If you load a scene with some scripting objects, ensure the game is in proper state, otherwise you'll crash the engine.
                 * I.e., if you try to load a castle defense scene without having an active MissionState, kaboom! This is handled through 
                 * engine callbacks so, we can't do much to protect against this.
                */
                this.Scene.Read("empty_scene");
                for (int i = 0; i < 40; i++) {
                    this.Scene.Tick(0.1f);
                }

                Vec3 vec = default(Vec3);
                GameEntity cameraInstance = this.Scene.FindEntityWithTag("camera_instance");
                cameraInstance.GetCameraParamsFromCameraScript(this.Camera, ref vec);

                SoundManager.SetListenerFrame(this.Camera.Frame);
                if (this.SceneLayer != null) {
                    this.SceneLayer.SetScene(this.Scene);
                    this.SceneLayer.SceneView.SetEnable(true);
                    this.SceneLayer.SceneView.SetSceneUsesShadows(true);
                }
                LoadingWindow.DisableGlobalLoadingWindow();
                base.OnActivate();
            }

            protected override void OnFrameTick(float dt) {
                base.OnFrameTick(dt);
                if (this.SceneLayer != null && this.SceneLayer.SceneView.ReadyToRender()) {
                    this.SceneLayer.SetCamera(this.Camera);
                }
                this.Scene.Tick(dt);
            }

            protected override void OnDeactivate() {
                base.OnDeactivate();
            }

            void IGameStateListener.OnActivate() { }

            void IGameStateListener.OnDeactivate() { }

            void IGameStateListener.OnInitialize() { }

            void IGameStateListener.OnFinalize() { }
        }
    }
}

