using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildFallOutOfBoatSE : ScriptedEvent
{
    public NPC alison;
    public Transform alisonGrabTrans;
    public DialogueScriptableObject[] stageSOs;
    bool hasFallen;
    float ypos;
    // Start is called before the first frame update
    void Start()
    {
        //alison.stage = 1;
        //StartCoroutine(ChildFallOutOfBoatIE());
        //hasFallen = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (stage == 0)
        {
            alison.stage = 0;
            if (GameManager.gM.player.transform.position.x < 1480)
            {
                GameManager.gM.dialogueHandler.StartDialogue(stageSOs[0]);
                stage = 1;
            }
        }
        else if (stage == 1)
        {
            if (!hasFallen) {alison.stage = 1;}
            
            if (!hasFallen && GameManager.gM.player.transform.position.x < 1400)
            {
                StartCoroutine(ChildFallOutOfBoatIE());
                hasFallen = true;
            }
        }
        else if (stage == 2)
        {
            if (GameManager.gM.progMan.mainQLStage < 1)
            {
                GameManager.gM.progMan.mainQLStage = 1;
            }
            alisonGrabTrans.position = new Vector3(alisonGrabTrans.position.x, Mathf.Lerp(alisonGrabTrans.position.y, ypos, Time.deltaTime * 5), alisonGrabTrans.position.z);
            alisonGrabTrans.position -= Vector3.right * 2.5f * Time.deltaTime;
            ypos -= Time.deltaTime * 0.02f;
            Vector3 disp = GameManager.gM.player.transform.position - alisonGrabTrans.position;
            disp.y = 0;
            alisonGrabTrans.rotation = Quaternion.LookRotation(disp.normalized, Vector3.up);
            //alisonGrabTrans.forward = disp.normalized;
            alison.stage = 2;
            if (disp.magnitude > 20 || alisonGrabTrans.position.y < -10)
            {
                //GameManager.gM.dialogueHandler.StartDialogue(stageSOs[2]);
                stage = 3;
            }
        }
        else if (stage == 3)
        {
            alison.stage = 3;
        }
    }
    IEnumerator ChildFallOutOfBoatIE()
    {
        GameManager.gM.dialogueHandler.StartDialogue(stageSOs[1]);
        alison.dialogueInter.gameObject.SetActive(false);
        alison.anim.SetTrigger("YankOutOfBoat");
        yield return new WaitForSeconds(0.5f);
        alison.stage = 2;
        alison.transform.position = Vector3.down * 100;
        ypos = alisonGrabTrans.position.y;
        alisonGrabTrans.position = GameManager.gM.player.transform.position - (Vector3.right * 1);
        alisonGrabTrans.position = new Vector3(alisonGrabTrans.position.x, ypos - 5, alisonGrabTrans.position.z);
        yield return new WaitForSeconds(1f);
        alisonGrabTrans.position = GameManager.gM.player.transform.position - (Vector3.right * 2.5f);
        alisonGrabTrans.position = new Vector3(alisonGrabTrans.position.x, ypos - 5, alisonGrabTrans.position.z);
        stage = 2;

    }
}
