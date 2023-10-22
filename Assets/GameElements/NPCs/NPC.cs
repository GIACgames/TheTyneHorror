using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[System.Serializable]
public class StageProperties
{
    public Vector3 position;
    public  Quaternion rotation;
    public int animRestStage;
    public Transform parent;
    public bool inBoat;
    public DialogueScriptableObject[] dialogueSOs;
}
public class NPC : MonoBehaviour
{
    public Animator anim;
    public Transform body;
    public Transform head;
    public Transform lookAtTrans;
    public DialogueInteractable dialogueInter;
    public bool faceTarget;
    public float headTurnSpeed = 30f;
    Vector3 lookDirection;
    public int stage = 1;
    public StageProperties[] stages;
    int lastStage = -1;

    // Start is called before the first frame update
    void Start()
    {
        if (dialogueInter != null) {dialogueInter.npc = this;}
    }

    // Update is called once per frame
    void Update()
    {
        ManageStage();
        ManageHead();
    }
    public void ManageStage()
    {
        if (stage != -1)
        {
            if (lastStage != stage)
            {
                OnNewStage();
                lastStage = stage;
            }
        }
    }
    public void OnNewStage()
    {
        dialogueInter.dialogueSOs = stages[stage].dialogueSOs;
        dialogueInter.dialogueSOIndex = 0;
        anim.SetInteger("restType", stages[stage].animRestStage);
        body.parent = stages[stage].parent;
        body.localPosition = stages[stage].position;
        body.localRotation = stages[stage].rotation;
    }
    void ManageHead()
    {
        Vector3 lookTarg = head.position + (body.forward);
        if (faceTarget && lookAtTrans != null) {lookTarg = lookAtTrans.position;}
        lookDirection = Vector3.Lerp(lookDirection,lookTarg - head.position, headTurnSpeed * Time.deltaTime).normalized;
        head.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
    }
}
