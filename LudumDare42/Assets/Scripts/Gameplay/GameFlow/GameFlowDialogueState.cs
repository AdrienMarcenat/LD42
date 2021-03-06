﻿using UnityEngine;

public class GameFlowDialogueState : HSMState
{
    public override void OnEnter ()
    {
        this.RegisterAsListener ("Game", typeof (GameFlowEvent));
    }

    public void OnGameEvent (GameFlowEvent flowEvent)
    {
        switch (flowEvent.GetAction ())
        {
            case EGameFlowAction.EndDialogue:
                ChangeNextTransition (HSMTransition.EType.Exit);
                break;
        }
    }

    public override void OnExit ()
    {
        this.UnregisterAsListener ("Game");
    }
}