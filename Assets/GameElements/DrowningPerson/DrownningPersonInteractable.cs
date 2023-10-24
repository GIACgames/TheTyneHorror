using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrownningPersonInteractable : Interactable
{
    public NPC npc;
    public Transform drownerTrans;
    public Animator anim;
    public bool hasBeenRescued;
    public DialogueScriptableObject rescueDialogue;
    void Start()
    {
        Rejigger();
    }
    
    public void Rejigger()
    {
        selectable = !hasBeenRescued;
        grabbable = true;
        pickupable = false;

    }
    public override void Interact(Player pl)
    {
        base.Interact();
        if (npc != null)
        {
            StartCoroutine(RescueIE());
        }
    }
    IEnumerator RescueIE()
    {
        hasBeenRescued = true;
        anim.SetTrigger("Rescue");
        yield return new WaitForSeconds(1);
        
        //FadeToBlack
        yield return new WaitForSeconds(1);
        player.LetGo();
        npc.stage += 1;
        GameManager.gM.dialogueHandler.StartDialogue(rescueDialogue, npc);

        
        
    }
}
