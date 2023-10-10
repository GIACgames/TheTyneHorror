using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Asset", menuName = "New Dialogue SO")]
public class DialogueScriptableObject : ScriptableObject
{


    [TextArea]
    public string characterName;

    [TextArea]
    public string[] dialogueStrings;
}
