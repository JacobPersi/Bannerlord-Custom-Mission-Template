using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

public class CustomGameManager : MBGameManager
{
    public override void OnLoadFinished()
    {
        base.OnLoadFinished();
        Game.Current.GameStateManager.CleanAndPushState(Game.Current.GameStateManager.CreateState<CustomGameState>(), 0);
    }
    
    protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
    {
        nextStep = GameManagerLoadingSteps.None;
        switch (gameManagerLoadingStep)
        {
            case GameManagerLoadingSteps.PreInitializeZerothStep:
                MBGameManager.LoadModuleData(false);
                MBGlobals.InitializeReferences();
                /* Load our custom game type! */
                Game.CreateGame(new CustomGameType(), this).DoLoading();
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
}
