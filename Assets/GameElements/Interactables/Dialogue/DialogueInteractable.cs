using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInteractable : Interactable
{
    public DialogueScriptableObject[] dialogueSOs;
    public int dialogueSOIndex; 
    public bool loopSOs;
    public override void Interact(Player pl)
    {
        base.Interact(pl);
        if (dialogueSOIndex < dialogueSOs.Length)
        {
            GameManager.gM.dialogueHandler.StartDialogue(dialogueSOs[dialogueSOIndex]); // Start the coroutine and play the dialogue at the current dialogue index
            dialogueSOIndex++; // Increment dialogue index so next time it is called the next dialogue plays
            if (loopSOs) {dialogueSOIndex = dialogueSOIndex % dialogueSOs.Length;}
        }
    }
}
