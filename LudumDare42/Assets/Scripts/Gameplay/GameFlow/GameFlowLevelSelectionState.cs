public class GameFlowLevelSelectionState : HSMState
{
    public override void OnEnter ()
    {
        LevelManagerProxy.Get ().LoadScene ("Scenes/LevelSelection");
        this.RegisterAsListener ("Game", typeof (GameFlowEvent));
    }

    public void OnGameEvent (GameFlowEvent flowEvent)
    {
        if (flowEvent.GetAction () == EGameFlowAction.Start)
        {
            ChangeNextTransition (HSMTransition.EType.Clear, typeof (GameFlowLevelState));
        }
    }

    public override void OnExit ()
    {
        this.UnregisterAsListener ("Game");
    }
}