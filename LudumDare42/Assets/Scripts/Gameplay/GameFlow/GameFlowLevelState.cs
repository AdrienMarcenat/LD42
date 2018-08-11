
public class GameFlowLevelState : HSMState
{
    public override void OnEnter ()
    {
        LevelManagerProxy.Get ().LoadLevel();
        this.RegisterAsListener ("Player", typeof (GameOverGameEvent), typeof (PlayerInputGameEvent));
    }

    public void OnGameEvent (GameOverGameEvent gameOver)
    {
        ChangeNextTransition (HSMTransition.EType.Siblings, typeof (GameFlowEndLevelState));
    }

    public void OnGameEvent (PlayerInputGameEvent inputEvent)
    {
        if (inputEvent.GetInput () == "Pause" && inputEvent.GetInputState() == EInputState.Down && !UpdaterProxy.Get().IsPaused())
        {
            ChangeNextTransition (HSMTransition.EType.Child, typeof (GameFlowPauseState));
        }
    }

    public override void OnExit ()
    {
        this.UnregisterAsListener ("Player");
    }
}