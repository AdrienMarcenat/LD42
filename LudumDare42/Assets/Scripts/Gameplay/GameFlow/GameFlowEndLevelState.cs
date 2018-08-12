using UnityEngine;

public class GameFlowEndLevelState : HSMState
{
    public override void OnEnter ()
    {
        UpdaterProxy.Get ().SetPause (true);
        LevelManagerProxy.Get ().OnLevelEnd ();
        this.RegisterAsListener ("Game", typeof (GameFlowEvent));
    }

    public void OnGameEvent (GameFlowEvent flowEvent)
    {
        switch (flowEvent.GetAction ())
        {
            case EGameFlowAction.Menu:
                ChangeNextTransition (HSMTransition.EType.Clear, typeof (GameFlowMenuState));
                break;
            case EGameFlowAction.Retry:
                ChangeNextTransition (HSMTransition.EType.Clear, typeof (GameFlowLevelState));
                break;
            case EGameFlowAction.NextLevel:
                LevelManagerProxy.Get ().NextLevel ();
                ChangeNextTransition (HSMTransition.EType.Clear, typeof (GameFlowLevelState));
                break;
        }
    }

    public override void OnExit ()
    {
        this.UnregisterAsListener ("Game");
        UpdaterProxy.Get ().SetPause (false);
    }
}