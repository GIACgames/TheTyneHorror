using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlisonCrucifixSE : ScriptedEvent
{
    public NPC alisonNPC;
    public Transform crossTrans;
    public Transform afterFreePos;
    public Transform headOverride;
    public DemonicEntity demonAlice;
    public CreditsSE credits;
    public DialogueScriptableObject[] dialogSOs;
    bool hasDied;
    public GiantHandsEatAttack giantAttack;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gM.progMan.mainQLStage > 2)
        {
            if (stage == 0)
            {
                alisonNPC.stage = 4;
                if (Vector3.Distance(crossTrans.position, GameManager.gM.player.transform.position) < 13)
                {
                    giantAttack.attackStage = 1;
                    stage = 1;
                }
                
            }
            else if (stage == 1)
            {
                crossTrans.position = Vector3.MoveTowards(crossTrans.position, new Vector3(GameManager.gM.player.transform.position.x - 10,crossTrans.position.y, crossTrans.position.z), Time.deltaTime * 2);
                if (giantAttack.attackStage == 0)
                {
                    stage = 9;
                }
            }
            else if (stage == 9)
            {
            }
            else if (stage == 10)
            {
                //Play animation
            }
            else if (stage == 11)
            {
                //Cut To credits;
            }

            if (stage < 9)
            {
                alisonNPC.faceTarget = Vector3.Distance(GameManager.gM.player.transform.position, alisonNPC.transform.position) < 5;
            }
            else
            {
                if (demonAlice.condemnLevel > 2)
                {
                    if (!hasDied) {StartCoroutine(KillAlison());}
                    alisonNPC.faceTarget = false;
                }
                else
                {
                     if (alisonNPC.overrideHeadRot) {
                alisonNPC.head.position = headOverride.position;
                alisonNPC.head.rotation = headOverride.rotation;
                     }}
            }
        }
    }
    public IEnumerator KillAlison()
    {
        hasDied = true;
        alisonNPC.overrideHeadRot = false;
        alisonNPC.anim.SetTrigger("Die");
        Sound screamAS = GameManager.gM.sfxManager.PlaySoundAtPoint("NPCs", "Alison", Camera.main.transform.position, 0.6f, 99, false, null, 0);
        while (screamAS.source.volume > 0 )
        {
            screamAS.source.volume -= 0.4f * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(2);
        credits.CutToCredits();
    }
    public IEnumerator FreeAlison()
    {
        GameManager.gM.transitionManager.FadeTransition(1f);
        while (!GameManager.gM.transitionManager.fullyFaded)
        {
            yield return null;
        }
        GameManager.gM.player.boat.transform.position = afterFreePos.position;
        GameManager.gM.player.boat.transform.rotation = afterFreePos.rotation;
        GameManager.gM.player.inBoat = true;
        stage = 9;
        alisonNPC.stage = 5;
        yield return new WaitForSeconds(2);
        GameManager.gM.dialogueHandler.StartDialogue(dialogSOs[0]);
        yield return new WaitForSeconds(7);
        demonAlice.gameObject.SetActive(true);
        if (!hasDied)
        {
        alisonNPC.anim.SetTrigger("GlitchOut");
        yield return new WaitForSeconds(11.25f);
        if (!hasDied)
        {
        Sound splashAS = GameManager.gM.sfxManager.PlaySoundAtPoint("NPCs", "Alison", Camera.main.transform.position, 3f, 99, false, null, 1);
        Sound screamAS = GameManager.gM.sfxManager.PlaySoundAtPoint("NPCs", "Alison", Camera.main.transform.position, 3f, 99, false, null, 0);
        
        yield return new WaitForSeconds(1.1f);
        credits.CutToCredits();
        yield return new WaitForSeconds(0.05f);
        
        while (splashAS.source.volume > 0 || screamAS.source.volume > 0 )
        {
            splashAS.source.volume -= 0.7f * Time.deltaTime;
            screamAS.source.volume -= 0.5f * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0f);
        
        }
        }
    }
}
