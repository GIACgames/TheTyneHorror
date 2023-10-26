using System;
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
    Sound splashAS;
    Sound screamAS;
    bool fallenIn;
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
        if (fallenIn && stage != 3)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 90, 3.5f * Time.deltaTime);
        }
        if (stage == 0)
        {
            alison.stage = 0;
            if (GameManager.gM.player.transform.position.x < 1505)
            {
                GameManager.gM.dialogueHandler.StartDialogue(stageSOs[0]);
                stage = 1;
            }
        }
        else if (stage == 1)
        {
            if (!hasFallen) {alison.stage = 1;}
            
            if (!hasFallen && GameManager.gM.player.transform.position.x < 1470)
            {
                StartCoroutine(ChildFallOutOfBoatIE());
                hasFallen = true;
            }
        }
        else if (stage == 2)
        {
            GameManager.gM.sfxManager.waterVolumeMulti = Mathf.Lerp(GameManager.gM.sfxManager.waterVolumeMulti, 1, Time.deltaTime * 1f);
            if (GameManager.gM.progMan.mainQLStage < 1)
            {
                GameManager.gM.progMan.mainQLStage = 1;
            }
            alisonGrabTrans.position = new Vector3(alisonGrabTrans.position.x, Mathf.Lerp(alisonGrabTrans.position.y, ypos, Time.deltaTime * 5), alisonGrabTrans.position.z);
            alisonGrabTrans.position -= Vector3.right * 3.5f * Time.deltaTime;
            ypos -= Time.deltaTime * 0.02f;
            Vector3 disp = GameManager.gM.player.transform.position - alisonGrabTrans.position;
            disp.y = 0;
            alisonGrabTrans.rotation = Quaternion.LookRotation(disp.normalized, Vector3.up);
            //alisonGrabTrans.forward = disp.normalized;
            alison.stage = 2;
            splashAS.source.volume = (1 - Mathf.Clamp(disp.magnitude / 25, 0, 1)) * 0.5f;
            if (disp.magnitude > 12 || alisonGrabTrans.position.y < -10)
            {
                //GameManager.gM.dialogueHandler.StartDialogue(stageSOs[2]);
                stage = 3;
            }
            
        }
        else if (stage == 3)
        {
            if (splashAS != null)
            {
            splashAS.active = false;
            splashAS = null;
            screamAS.active = false;
            screamAS = null;
            }
            GameManager.gM.sfxManager.waterVolumeMulti = Mathf.Lerp(GameManager.gM.sfxManager.waterVolumeMulti, 0.7f, Time.deltaTime * 0.7f);
            alison.stage = 3;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, 1 * Time.deltaTime);
        }
    }
    IEnumerator ChildFallOutOfBoatIE()
    {
        GameManager.gM.dialogueHandler.StartDialogue(stageSOs[1]);
        yield return new WaitForSeconds(0f);
        GameManager.gM.sfxManager.PlaySoundAtPoint("AmbientSpook", "ShortBuildingNote", Camera.main.transform.position, 2, 90, false, null, 0).source.transform.parent = Camera.main.transform;
        yield return new WaitForSeconds(14.6f);
        fallenIn = true;
        splashAS = GameManager.gM.sfxManager.PlaySoundAtPoint("NPCs", "Alison", Camera.main.transform.position, 3f, 99, true, null, 1);
        screamAS = GameManager.gM.sfxManager.PlaySoundAtPoint("NPCs", "Alison", Camera.main.transform.position, 0.5f, 100, true, null, 0);
        screamAS.source.transform.parent = Camera.main.transform;
        splashAS.source.transform.parent = Camera.main.transform;
        yield return new WaitForSeconds(0.1f);
        
        GameManager.gM.dialogueHandler.StartDialogue(stageSOs[2]);
        alison.dialogueInter.gameObject.SetActive(false);
        alison.anim.SetTrigger("YankOutOfBoat");
        yield return new WaitForSeconds(0.5f);
        GameManager.gM.hintManager.ShowHint("[LSHIFT] TO ROW FASTER",10f);
        alison.stage = 2;
        alison.transform.position = Vector3.down * 100;
        ypos = alisonGrabTrans.position.y;
        alisonGrabTrans.position = GameManager.gM.player.transform.position - (Vector3.right * 1);
        alisonGrabTrans.position = new Vector3(alisonGrabTrans.position.x, ypos - 5, alisonGrabTrans.position.z);
        yield return new WaitForSeconds(1f);
        alisonGrabTrans.position = GameManager.gM.player.transform.position - (Vector3.right * 2.5f);
        alisonGrabTrans.position = new Vector3(alisonGrabTrans.position.x, ypos - 5, alisonGrabTrans.position.z);
        stage = 2;
        screamAS.source.transform.parent = alisonGrabTrans;
        
    }
}
