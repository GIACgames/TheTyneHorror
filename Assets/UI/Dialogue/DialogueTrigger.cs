using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
    // Attached to NPCs or any objects that can trigger the dialogue
{
    private DialogueHandler dialogueHandler;
    public DialogueScriptableObject[] dialogue; // Attach the dialogue SO for this NPC
    private int dialogueIndex; // The current dialogue index for this character -- Points to which dialogueSO to be using

    public void Awake()
    {
        dialogueHandler = GameObject.FindGameObjectWithTag("DialogueHandler").GetComponent<DialogueHandler>();
    }
    public void Start()
    {
        dialogueIndex = 0;
    }

    public void triggerDialogue() // Call from button, or when in range of NPC, or when interacted with NPC, etc.
    {
        if (!dialogueHandler.dialgoueEnumFlag) // Starts the IEnum when triggered 
        {
            dialogueHandler.StartCoroutine(dialogueHandler.startDialogueEnum(dialogue[dialogueIndex])); // Start the coroutine and play the dialogue at the current dialogue index
            dialogueIndex++; // Increment dialogue index so next time it is called the next dialogue plays
        }
    }
}
