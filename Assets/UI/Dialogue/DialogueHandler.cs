using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueHandler : MonoBehaviour
{
    public bool dialogueInProgress;
    public static DialogueHandler DialogueHandlerInstance { get; private set; } // Singleton

    #region Dialogue Components
    public TextMeshProUGUI characterNameObject;
    public TextMeshProUGUI dialogueObject;
    public AudioSource[] speechAudioSrcs;
    public float speechVolume = 0.4f;
    public bool dialgoueEnumFlag;
    public string currentCharacterSpeaking;
    public string currentDialogue;
    #endregion

    #region Dialogue Typing Effect
    public bool textTypingEffectFlag;
    bool skipToEnd;
    Coroutine currentDialogueIE;
    DialogueParagraph curDialoguePara;
    public float lastEndDialogue;
    #endregion
    int curSpchASIndex;
    float[] aSTrackers;

    private void Awake()
    {
        aSTrackers = new float[speechAudioSrcs.Length];
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
        if (dialogueInProgress && curDialoguePara.canSkip)
        {
            if (Input.GetKeyDown("e")) { skipToEnd = true;}
        }

        for (int a = 0; a < aSTrackers.Length; a++)
        {
            if (speechAudioSrcs[a].isPlaying && Time.time - aSTrackers[a] > 0.6f)
            {
                speechAudioSrcs[a].Stop();
            }
        }

    }

    void SayLetter(char c, float pitch, float volume)
    {
      //aS.Stop();
      //print(c);
      c = Char.ToUpper(c);
      int asciiVal = System.Convert.ToInt32(c);
      if (asciiVal >= 65 && asciiVal <= 91)
      {
        if (speechAudioSrcs[curSpchASIndex].isPlaying) {speechAudioSrcs[curSpchASIndex].Stop();}
        speechAudioSrcs[curSpchASIndex].pitch = pitch;
        speechAudioSrcs[curSpchASIndex].volume = speechVolume * volume;
        speechAudioSrcs[curSpchASIndex].time = asciiVal - 65;
        speechAudioSrcs[curSpchASIndex].Play();
        aSTrackers[curSpchASIndex] = Time.time;
        curSpchASIndex = (curSpchASIndex + 1) % speechAudioSrcs.Length;
        //StartCoroutine(StopLetterSpeak());
      }
        
    }

    public void StartDialogue(DialogueScriptableObject dialogueSO , NPC npc = null)
    {
        if (dialogueInProgress)
        {
            
        }
        if (currentDialogueIE != null) {StopCoroutine(currentDialogueIE);}
        currentDialogueIE = StartCoroutine(startDialogueEnum(dialogueSO, npc));
    }
    public IEnumerator startDialogueEnum(DialogueScriptableObject dialogue, NPC npc = null)
        // Enum for displaying and cycling dialogue strings
    {
        bool originallyFacingP = false;
        if (npc != null) {originallyFacingP = npc.faceTarget;}
        float curPitch = dialogue.characterPitch;
        float curVolume = dialogue.characterVolume;
        dialogueInProgress = true;
        dialgoueEnumFlag = true; // Ensures multiple Enums dont run simultaneously
        showDialogueCanvas();
        Debug.Log("Coroutine Begins");
        Queue<DialogueParagraph> dialogueParas = new Queue<DialogueParagraph>(); // Creates a queue to hold dialogue strings
        dialogueParas.Clear(); // Ensures queue is empty
        //curDialoguePara = null;
        
        if (dialogue.dialogueStrings == null) // Check if there are no Dialogue strings
        {
            //Debug.Log("No Dialogue Strings");
            yield return null;
        }

        foreach (DialogueParagraph dialoguePara in dialogue.dialogueParagraphs) // Load every dialogue string in the array to the queue structure
        {
            dialogueParas.Enqueue(dialoguePara);
            //Debug.Log("Enqueued New Dialogue String");
        }

        
        currentCharacterSpeaking = dialogue.characterName; // Changes the character name to who is currently speaking
        //curDialoguePara = dialogueParas.Dequeue(); // Gets the new dialogue from the queue
        currentDialogue = ""; // Reset Current Dialogue string
        skipToEnd = false;
        /*for (int i = 0; i < curDialoguePara.dialogueString.Length; i++)
        // Iterate through string and add each letter to the current dialogue after 0.1 seconds
        {
            currentDialogue += curDialoguePara.dialogueString[i];
            if (!skipToEnd) {yield return new WaitForSeconds(0.05f / curDialoguePara.readSpeedMultiplier);}
        }
        yield return new WaitForSeconds(curDialoguePara.endLingerTime);
        skipToEnd = false;*/

        bool exitloop = false; // While loop exit bool 
        float lastCharTime = Time.time;
        bool firstPara = true;
        while (!exitloop) // For cycling through dialogue strings when Space is pressed
        {
       
            if(dialogueParas.Count == 0 && (Input.GetKeyDown("e") || (Time.time - lastCharTime > curDialoguePara.timeToAutoSkip)))
            {
                exitloop = true;
            }
            if ((Input.GetKeyDown("e") || firstPara) || (Time.time - lastCharTime > curDialoguePara.timeToAutoSkip))
            {
                firstPara = false;
                skipToEnd = false;
                if (dialogueParas.Count >= 1) 
                { 
                    curDialoguePara = dialogueParas.Dequeue(); // Gets the new dialogue from the queue
                    if (npc != null) {npc.faceTarget = curDialoguePara.facePlayer;}
                    curPitch = curDialoguePara.pitchAdjustment + dialogue.characterPitch;
                    curVolume = dialogue.characterVolume + curDialoguePara.volumeMultiplier;
                    currentDialogue = ""; // Reset Current Dialogue string
                    for (int i = 0; i < curDialoguePara.dialogueString.Length; i++)
                    // Iterate through string and add each letter to the current dialogue after 0.1 seconds
                    {
                        currentDialogue += curDialoguePara.dialogueString[i];
                        SayLetter(curDialoguePara.dialogueString[i], curPitch, curVolume);
                        if (!skipToEnd) {yield return new WaitForSeconds(0.05f / curDialoguePara.readSpeedMultiplier);}
                        lastCharTime = Time.time;
                    }
                    yield return new WaitForSeconds(curDialoguePara.endLingerTime);
                }
                else
                {
                    lastCharTime = Time.time;
                    exitloop = true;
                }
            }
            
            yield return null;
        }

        //Debug.Log("OUT OF STRINGS");


        // Reset vars and end Enum
        currentDialogue = "";
        hideDialogueCanvas();
        dialgoueEnumFlag = false;
        yield return null;
        dialogueInProgress = false;
        currentDialogueIE = null;
        lastEndDialogue = Time.time;
        if (npc != null) {npc.faceTarget = originallyFacingP;}
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
