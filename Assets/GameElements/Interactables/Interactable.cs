using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Transform interTrans;
    public string interactKey = "e";
    public int interId;
    public int interType;
    public bool isSelected;
    public bool selectable = true;
    public bool grabbed;
    public bool grabbable;
    public bool pickupable;
    public bool isUsing;
    public bool pickedUp;
    public Transform dropTrans;
    public Transform interPoint;
    public Transform[] handPoses;
    public Player player;
    public int[] pickedUpInterTypes;
    public float interViewDist = 0.3f;
    public float interDist = 5;
    public Renderer[] mRenderers;
    

    public virtual void Grab()
    {
        grabbed = true;
    }
    public virtual void PickUp()
    {
        pickedUp = true;
    }
    public virtual void Interact(Player pl = null)
    {
        player = pl;
    }
    public virtual void LetGo()
    {
        grabbed = false;
        if (pickupable)
        {
            Drop();
        }
    }
    public virtual void Drop()
    {
        pickedUp = false;
        if (dropTrans != null)
        {
            interTrans.parent = dropTrans;
            interTrans.localPosition = Vector3.zero;
            interTrans.localRotation = Quaternion.identity;
        }
        else
        {
            interTrans.parent = null;
        }
    }
    public virtual void Select(Player pl, Color col)
    {
        if (!isSelected)
        {
            isSelected = true;
            foreach (Renderer mR in mRenderers)
            {
                //mR.sharedMaterial = pl.outlineMat;
                /*Material[] mats = new Material[mR.sharedMaterials.Length + 1];
                for (int m = 0; m < mats.Length - 1; m++) {mats[m] = mR.sharedMaterials[m];}
                //mats[0] = mR.sharedMaterial;
                mats[mats.Length - 1] = pl.outlineMat;
                mR.sharedMaterials = mats;*/
                if (mR.GetComponent<Outline>() == null)
                {
                    mR.gameObject.AddComponent<Outline>();
                }
                mR.GetComponent<Outline>().enabled = true;
                mR.GetComponent<Outline>().OutlineColor = col;
            }
        }
    }
    public virtual void DeSelect()
    {
        if (isSelected)
        {
            isSelected = false;
            foreach (Renderer mR in mRenderers)
            {
                /*Material[] mats = new Material[mR.sharedMaterials.Length - 1];
                if (mats.Length > 0)
                {
                    for (int m = 0; m < mats.Length; m++) {mats[m] = mR.sharedMaterials[m];}
                    mR.sharedMaterials = mats;
                }*/
                if (mR.GetComponent<Outline>() != null)
                {
                    //Destroy(mR.GetComponent<Outline>());
                    mR.GetComponent<Outline>().enabled = false;
                }
                
            }
        }
    }
    public virtual void PIClickDown()
    {

    }
    public virtual void PIClickUp()
    {

    }
    public virtual void PIRClickDown()
    {

    }
    public virtual void PIRClickUp()
    {

    }
    public virtual void BeginUseItem(Interactable item)
    {

    }
    public virtual void StopUseItem(Interactable item)
    {

    }
}
