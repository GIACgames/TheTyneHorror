using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilInteractable : Interactable
{
    public float capacity;
    public float level;
    public bool addedToBoat;
    public FloatingObject fO;
    public MeshRenderer meshRndrer;
    public Material liquidmat;
    public void FixedUpdate()
    {
        if (grabbed)
        {
            liquidmat.SetFloat("_FillAmount", Mathf.Lerp(0.65f, 0.38f, level / capacity));
        }
        if (!addedToBoat && !grabbed)
        {
        }
        else
        {
            if (fO != null)
            {
                Destroy(fO.gameObject);
            }
        }
    }
    public override void PickUp()
    {
        base.PickUp();
        if (addedToBoat) {GameManager.gM.player.boat.TakeOilCan(this);}
        meshRndrer.sharedMaterials = new Material[] {meshRndrer.sharedMaterials[0], liquidmat};
    }
    public override void LetGo()
    {
        base.LetGo();
        if (level > 0) {GameManager.gM.player.boat.AddOilCan(this);
        meshRndrer.sharedMaterials = new Material[] {meshRndrer.sharedMaterials[0]};}
        else {Destroy(this.gameObject);}
    }
}
