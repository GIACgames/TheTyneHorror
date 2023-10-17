using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueHandler : MonoBehaviour
{
    public static DialogueHandler DialogueHandlerInstance { get; private set; } // Singleton

    #region Dialogue Components
    public TextMeshProUGUI characterNameObject;
    public TextMeshProUGUI dialogueObject;
    public bool dialgoueEnumFlag;
    public string currentCharacterSpeaking;
    public string currentDialogue;
    #endregion

    #region Dialogue Typing Effect
    public bool textTypingEffectFlag;
    #endregion

    private void Awake()
    {
        #region Singleton Setup
        // Delete this if dialogue handler instance already exists

        if (DialogueHandlerInstance != null && DialogueHandlerInstance != this)
        {
            Destroy(this);
        }
        else
        {
            DialogueHandlerInstance = this;
        }
    #endregion
        dialgoueEnumFlag = false;
    }

    public void Update()
    {
        // Change the text being displayed
        characterNameObject.text = currentCharacterSpeaking;
        dialogueObject.text = currentDialogue;

    }
    public void StartDialogue(DialogueScriptableObject dialogueSO)
    {
        StartCoroutine(startDialogueEnum(dialogueSO));
    }
    public IEnumerator startDialogueEnum(DialogueScriptableObject dialogue)
        // Enum for displaying and cycling dialogue strings
    {
        dialgoueEnumFlag = true; // Ensures multiple Enums dont run simultaneously
        showDialogueCanvas();
        Debug.Log("Coroutine Begins");
        Queue<string> dialogueStrings = new Queue<string>(); // Creates a queue to hold dialogue strings
        dialogueStrings.Clear(); // Ensures queue is empty
        string newDialogue;
        
        if (dialogue.dialogueStrings == null) // Check if there are no Dialogue strings
        {
            Debug.Log("No Dialogue Strings");
            yield return null;
        }

        foreach (string dialogueString in dialogue.dialogueStrings) // Load every dialogue string in the array to the queue structure
        {
            dialogueStrings.Enqueue(dialogueString);
            Debug.Log("Enqueued New Dialogue String");
        }

        
        currentCharacterSpeaking = dialogue.characterName; // Changes the character name to who is currently speaking
        newDialogue = dialogueStrings.Dequeue(); // Gets the new dialogue from the queue
        currentDialogue = ""; // Reset Current Dialogue string
        for (int i = 0; i < newDialogue.Length; i++)
        // Iterate through string and add each letter to the current dialogue after 0.1 seconds
        {
            currentDialogue += newDialogue[i];
            yield return new WaitForSeconds(0.1f);
        }

        bool exitloop = false; // While loop exit bool 
        while (!exitloop) // For cycling through dialogue strings when Space is pressed
        {
       
            if(dialogueStrings.Count == 0 && Input.GetKeyDown("space"))
            {
                exitloop = true;
            }

            if (Input.GetKeyDown("space"))
            {
                if (dialogueStrings.Count >= 1) 
                { 
                    newDialogue = dialogueStrings.Dequeue(); // Gets the new dialogue from the queue
                    currentDialogue = ""; // Reset Current Dialogue string
                    for (int i = 0; i < newDialogue.Length; i++)
                    // Iterate through string and add each letter to the current dialogue after 0.1 seconds
                    {
                        currentDialogue += newDialogue[i];
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                else
                {
                    exitloop = true;
                }
            }
            
            yield return null;
        }

        Debug.Log("OUT OF STRINGS");


        // Reset vars and end Enum
        currentDialogue = "";
        hideDialogueCanvas();
        dialgoueEnumFlag = false;
        yield return null;
    }



    #region Show & Hide Canvas Methods
    public void showDialogueCanvas()
    {
        characterNameObject.gameObject.SetActive(true);
        dialogueObject.gameObject.SetActive(true);
    }
    public void hideDialogueCanvas()
    {
        characterNameObject.gameObject.SetActive(false);
        dialogueObject.gameObject.SetActive(false);
    }
    #endregion

}
