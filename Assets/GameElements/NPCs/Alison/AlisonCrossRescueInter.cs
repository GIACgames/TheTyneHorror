using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlisonCrossRescueInter : Interactable
{
    public AlisonCrucifixSE acse;
    public override void Interact(Player pl = null)
    {
        base.Interact(pl);
        acse.StartCoroutine(acse.FreeAlison());
        selectable = false;
        //gameObject.SetActive(false);
    }
}
