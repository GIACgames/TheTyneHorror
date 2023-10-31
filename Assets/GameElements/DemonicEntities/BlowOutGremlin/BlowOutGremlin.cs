using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowOutGremlin : MonoBehaviour
{
    public Transform gremlinHolder;
    public Transform gremlinBody;
    public DemonicEntity demon;
    public Animation anim;
    public bool canBlowOut;
    public float timeToBlowOut = 5f;
    public float blowOutInterval = 10;
    public bool playerChecking;
    public bool onBoat;
    float lastCheckTime;
    float lastBlowOutTime;
    bool hasRetreated;
    public bool hasBlownOut;
    // Start is called before the first frame update
    void Start()
    {
        gremlinBody.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerChecking = Vector3.Distance(GameManager.gM.player.head.forward, (gremlinHolder.position - GameManager.gM.player.head.position).normalized) < 1;
        if (playerChecking) {lastCheckTime = Time.time;}
        else
        {
            if (!onBoat && canBlowOut && Time.time - lastBlowOutTime > blowOutInterval && Time.time - lastCheckTime > 2 && GameManager.gM.player.boat.lanternInter.level > 0 && GameManager.gM.player.inBoat)
            {
                if (GameManager.gM.progMan.mainQLStage < 2) {GameManager.gM.progMan.mainQLStage = 2;}
                JumpOnBoat();
            }
        }
        
        if (!onBoat)
        {
            hasRetreated = false;
        }
        else
        {
            if (demon.condemnLevel > 5)
            {
                if (!hasRetreated) { StartCoroutine(Retreat());}
            }
        }
    }
    public void JumpOnBoat()
    {
        onBoat = true;
        StartCoroutine(BlowOutIE());
    }
    IEnumerator Retreat()
    {
        hasRetreated = true;
        GameManager.gM.sfxManager.PlaySoundAtPoint("Monsters", "BlowOutG", gremlinBody.transform.position, 1f, 102, false, null, 2);
        anim.CrossFade("GremlinRetreat", 0.05f);
        yield return new WaitForSeconds(0.6f);
        gremlinBody.parent = transform;
        gremlinBody.gameObject.SetActive(false);
        onBoat = false;
        lastBlowOutTime = Time.time;
        yield return new WaitForSeconds(3f);
        hasRetreated = false;
        
    }
    IEnumerator BlowOutIE()
    {
        //PlayClimbAboardSFX
        hasBlownOut = true;
        GameManager.gM.sfxManager.PlaySoundAtPoint("Monsters", "BlowOutG", Camera.main.transform.position, 1f, 104, false, null, 0).source.transform.parent = Camera.main.transform;
        gremlinBody.gameObject.SetActive(true);
        gremlinBody.parent = gremlinHolder;
        gremlinBody.localPosition = Vector3.zero;
        gremlinBody.localRotation = Quaternion.identity;
        yield return new WaitForSeconds(timeToBlowOut);
        if (!hasRetreated)
        {
            //PlayBlowoutSFXandAnimation
            GameManager.gM.sfxManager.PlaySoundAtPoint("Monsters", "BlowOutG", gremlinBody.transform.position, 1f, 103, false, null, 1);
            anim.CrossFade("GremlinBlowOut", 0.1f);
            GameManager.gM.player.boat.lanternInter.level = 0;
            yield return new WaitForSeconds(1);
            while (!playerChecking)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.4f);
            if (!hasRetreated)
            {
                StartCoroutine(Retreat());
            }
        }
    }
}
