using UnityEngine;
using System.Collections;

public class PlayerHSM : HSM
{
    public PlayerHSM ()
        : base (new GameFlowLevelState ()
              , new GameFlowPauseState ()
        )
    {
        Start (typeof (GameFlowLevelState));
    }
}

