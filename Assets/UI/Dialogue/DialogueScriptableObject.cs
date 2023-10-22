using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogueParagraph
{
    [TextArea]
    public string dialogueString;
    public bool facePlayer;
    public float timeToAutoSkip;
    public float readSpeedMultiplier;
    public bool canSkip;
    public float endLingerTime;
    public float pitchAdjustment;
    public float volumeMultiplier;

    public DialogueParagraph(string dS, bool fP = false, float tTAS = 4f, float rSM = 1, bool cS = true, float eLT = 0, float pA = 0, float vM = 1)
    {
        dialogueString = dS;
        facePlayer = fP;
        timeToAutoSkip = tTAS;
        readSpeedMultiplier = rSM;
        canSkip = cS;
        endLingerTime = eLT;
        pitchAdjustment = pA;
        volumeMultiplier = vM;
    }
}

[CreateAssetMenu(fileName = "Dialogue Asset", menuName = "New Dialogue SO")]
public class DialogueScriptableObject : ScriptableObject
{

    [TextArea]
    public string characterName;
    public float characterPitch = 1;
    public float characterVolume = 1;
    public float timeSinceLastSOtoShow = 0; //The time before this can be read from dialogue inter since the last dialogueSO was read

    [TextArea]
    public string[] dialogueStrings;
    public DialogueParagraph[] dialogueParagraphs;
}
