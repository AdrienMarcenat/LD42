
using System;
using UnityEngine;

public class Wall : TileObject
{
    public override void Init (ETileObjectType type, int x, int y, string[] args)
    {
        base.Init (type, x, y, args);
        EFacingDirection wallOrientation = (EFacingDirection)Enum.Parse (typeof (EFacingDirection), (String)args.GetValue (0), true);
        GetComponent<SpriteRenderer> ().sprite = RessourceManager.LoadSprite (args[1], 0);
        transform.rotation = Quaternion.Euler (new Vector3 (0, 0, PlayerController.ms_FacingAngles[wallOrientation]));
    }

    public override bool IsObstacle ()
    {
        return true;
    }

    public override bool CanBeGrabed ()
    {
        return false;
    }
}
