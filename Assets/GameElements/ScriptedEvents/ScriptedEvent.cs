using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedEvent : MonoBehaviour
{
    public int stage = 0; //SaveValue
    public bool playerInTrigger;
    public Transform[] triggerLocations;
    public virtual void EnterTrigger()
    {

    }
    public virtual void ExitTrigger()
    {

    }

    public void OnTriggerStay(Collider col)
    {
        if (col.gameObject.layer == 10)
        {
            if (!playerInTrigger)
            {
                EnterTrigger();
            }
            playerInTrigger = true;
        }
    }
    public void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer == 10)
        {
            if (playerInTrigger)
            {
                ExitTrigger();
            }
            playerInTrigger = false;
        }
    }

}
