using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LankyMonsterAI : MonoBehaviour
{
    public Transform body;
    public BuoyForestSE bF;
    public Animator anim;
    public DemonicEntity demonEnt;
    public float turnSpeed = 2;
    public float walkSpeed;
    bool isJumpScaring;
    public Transform grabPlayerPos;
    public Transform animGrabPlayerPos;
    bool grabbedPlayer;
    bool lockedGrab;
    bool hasDied;
    public AudioSource wadeAS;
    bool hasEaten;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (demonEnt.condemnLevel < 5 || grabbedPlayer)
        {
            Vector3 disp = GameManager.gM.player.transform.position - body.position;
            disp.y = 0;
            if (!isJumpScaring)
            {
                body.rotation = Quaternion.Lerp(body.rotation,Quaternion.LookRotation(disp.normalized, Vector3.up),turnSpeed * Time.deltaTime);
            }
            else
            {
                
                Vector3 targBodPos = GameManager.gM.player.transform.position + (-disp.normalized * 4);
                targBodPos.y = body.position.y;
                if (grabbedPlayer)
                {
                    GameManager.gM.player.transform.forward = Vector3.Lerp(GameManager.gM.player.transform.forward, disp,1 * Time.deltaTime);
                    if (lockedGrab)
                    {
                        grabPlayerPos.position = animGrabPlayerPos.position;
                        grabPlayerPos.rotation = animGrabPlayerPos.rotation;
                    }
                    else
                    {
                        grabPlayerPos.position = Vector3.Lerp(grabPlayerPos.position, animGrabPlayerPos.position,15 * Time.deltaTime);
                        grabPlayerPos.rotation = Quaternion.Lerp(grabPlayerPos.rotation, animGrabPlayerPos.rotation,10 * Time.deltaTime);
                    }
                }
                else{body.position = Vector3.MoveTowards(body.position, targBodPos, walkSpeed * Time.deltaTime * 2);body.rotation = Quaternion.Lerp(body.rotation,Quaternion.LookRotation(disp.normalized, Vector3.up),turnSpeed * 3 * Time.deltaTime);}
            }
            if (disp.magnitude < 4)
            {
                if (!isJumpScaring) {StartCoroutine(JumpScare());}
            }
            else
            {
                float moveMultiplier = 1;
                float lookDist = Vector3.Distance(GameManager.gM.player.head.forward, (body.position - GameManager.gM.player.head.position).normalized);
                if (lookDist  > 1.2f) 
                {
                    moveMultiplier = 5f;
                    if (disp.magnitude > 15)
                    {
                        Destroy(gameObject);
                    }
                }
                body.position += walkSpeed * body.forward * moveMultiplier;
            }
            if (!hasEaten) {wadeAS.volume = (1 - Mathf.Clamp(disp.magnitude/ 30, 0,1)) * 2;}
        }
        else
        {
            wadeAS.volume = 0;
            if (!hasDied) {StartCoroutine(DeathIE());}
            hasDied = true;
            
        }
        if (hasEaten) {wadeAS.volume = Mathf.Lerp(wadeAS.volume, 0, 10 * Time.deltaTime);}
    }
    IEnumerator DeathIE()
    {
        anim.SetTrigger("Die");
        GameManager.gM.sfxManager.PlaySoundAtPoint("Monsters", "LankyMonster", Camera.main.transform.position, 0.5f, 100, true, null, 1);
        yield return new WaitForSeconds(0.4f);
        if (bF != null) {bF.lastLMDeath = Time.time;}
        Destroy(gameObject);
    }
    IEnumerator JumpScare()
    {
        isJumpScaring = true;
        anim.SetTrigger("Jumpscare");
        yield return new WaitForSeconds(2.4f);
        if (demonEnt.condemnLevel < 5)
        {
            lockedGrab = false;
            grabbedPlayer = true;
            grabPlayerPos.position = GameManager.gM.player.transform.position;
            grabPlayerPos.rotation = GameManager.gM.player.transform.rotation;
            GameManager.gM.player.grabbedPoser = grabPlayerPos;
            yield return new WaitForSeconds(0.2f);
            lockedGrab = true;
            yield return new WaitForSeconds(0.4f);
            GameManager.gM.sfxManager.PlaySoundAtPoint("Monsters", "LankyMonster", Camera.main.transform.position, 5f, 100, true, null, 0);
            yield return new WaitForSeconds(1.20f);
            hasEaten = true;
            GameManager.gM.transitionManager.FadeTransition(1);
            yield return new WaitForSeconds(5);
            isJumpScaring = false;
            
        }
        else
        {

        }
    }
}
