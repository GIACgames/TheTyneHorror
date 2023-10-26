using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowOutGremlin : MonoBehaviour
{
    public Transform gremlinHolder;
    public Transform gremlinBody;
    public bool canBlowOut;
    public float timeToBlowOut = 5f;
    public float blowOutInterval = 10;
    public bool playerChecking;
    public bool onBoat;
    float lastCheckTime;
    float lastBlowOutTime;
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
    }
    public void JumpOnBoat()
    {
        onBoat = true;
        StartCoroutine(BlowOutIE());
    }
    IEnumerator BlowOutIE()
    {
        //PlayClimbAboardSFX
        gremlinBody.gameObject.SetActive(true);
        gremlinBody.parent = gremlinHolder;
        gremlinBody.localPosition = Vector3.zero;
        gremlinBody.localRotation = Quaternion.identity;
        yield return new WaitForSeconds(timeToBlowOut);
        //PlayBlowoutSFXandAnimation
        GameManager.gM.player.boat.lanternInter.level = 0;
        yield return new WaitForSeconds(1);
        while (!playerChecking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        gremlinBody.parent = transform;
        gremlinBody.gameObject.SetActive(false);
        onBoat = false;
        lastBlowOutTime = Time.time;

    }
}
