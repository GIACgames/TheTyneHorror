using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatExitInter : Interactable
{

    public int exitId;
    public Transform exitPos;
    public Transform boatDockPos;
    public override void Interact(Player pl)
    {
        base.Interact(pl);
        if (pl.inBoat)
        {
            player.ExitBoat(exitPos.position, exitPos.rotation, boatDockPos, exitId);
            
        }
    }
}
