using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatExitInter : Interactable
{
    public int exitId;
    public Transform exitPos;
    public override void Interact(Player pl)
    {
        base.Interact(pl);
        player.ExitBoat(exitPos.position, exitPos.rotation, exitId);
    }
}
