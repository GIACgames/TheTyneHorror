using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OarInteractable : Interactable
{
    public Boat boat;
    public void FixedUpdate()
    {
        selectable = !boat.isHoldingOars;
    }
    public override void Grab()
    {
        base.Grab();
        boat.GrabOars();
    }
    public override void LetGo()
    {
        base.LetGo();
        boat.ReleaseOars();
    }
}