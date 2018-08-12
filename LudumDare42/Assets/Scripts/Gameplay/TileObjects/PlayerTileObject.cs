
using System;

public class PlayerTileObject : TileObject
{
    public override void Init (ETileObjectType type, int x, int y, string[] args)
    {
        base.Init (type, x, y, args);
        EFacingDirection playerFacingDirection = (EFacingDirection)Enum.Parse (typeof (EFacingDirection), (String)args.GetValue (0), true);
        GetComponent<PlayerController> ().SetFacingDirection (playerFacingDirection);
    }

    public override bool IsObstacle ()
    {
        return false;
    }

    public override bool CanBeGrabed ()
    {
        return false;
    }
}
