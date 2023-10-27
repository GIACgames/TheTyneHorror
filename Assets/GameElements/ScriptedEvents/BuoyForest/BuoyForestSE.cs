using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyForestSE : ScriptedEvent
{
    public GameObject BuoyPrefab;
    public GameObject lankyMonsterPrefab;
    public Transform[] lmS;
    public Transform buoyHolder;
    public Transform[] buoyInstances;
    public int buoyInstanceCount = 10;
    public Vector3 lastNBPos;
    public bool inBuoyForest;
    public int lmsCount;
    public float lastLMDeath;
    // Start is called before the first frame update
    void Start()
    {
        GenerateBuoyInstances();
    }

    // Update is called once per frame
    void Update()
    {
        inBuoyForest = playerInTrigger;
        if (Vector3.Distance(lastNBPos, GameManager.gM.player.transform.position) > 7)
        {
            NewBuoys();
        }

        if (inBuoyForest)
        {
            if (Time.time - lastLMDeath > 26 && GameManager.gM.player.transform.position.x < 1100)
            {
                for (int i = 0; i < lmS.Length; i++)
                {
                    if (i < lmsCount)
                    {
                        if (lmS[i] == null)
                        {
                            lmS[i] = Instantiate(lankyMonsterPrefab, Vector3.down * 100, Quaternion.identity).transform;
                            lmS[i].GetComponent<LankyMonsterAI>().bF = this;
                        }
                        if (Vector3.Distance(lmS[i].position, GameManager.gM.player.transform.position) > 22)
                        {
                            Vector3 pos = GameManager.gM.player.transform.position + (((GameManager.gM.player.transform.forward) + (GameManager.gM.player.transform.right * Random.Range(-1f, 1f))).normalized * 18);// + (new Vector3(Random.Range(-1f, -0.5f), 0, Random.Range(-0.1f, 0.1f)).normalized * 19);
                            pos.y = -1.48f;
                            lmS[i].position = pos;
                        }
                    }
                    else
                    {

                    }
                }
            }
        }
    }
    void NewBuoys()
    {
        Transform playerTrans = GameManager.gM.player.transform;
        Vector3 dir =  lastNBPos - playerTrans.position;
        List<Transform> newBuoys = new List<Transform>();
        for (int i = 0; i < buoyInstanceCount; i++)
        {
            if ((!buoyInstances[i].gameObject.activeSelf && inBuoyForest) || Vector3.Distance(buoyInstances[i].position, playerTrans.position) > 20)
            {
                newBuoys.Add(buoyInstances[i]);
                buoyInstances[i].gameObject.SetActive(false);
            }
        }
        int buoysToMove = 2;
        for (int i = 0; i < newBuoys.Count; i++)
        {
            if (i < buoysToMove)
            {
                if (inBuoyForest)
                {
                    newBuoys[i].position = playerTrans.position + (-dir.normalized * 18 * Random.Range(1f, 1.3f)) + (Vector3.Cross(dir.normalized, Vector3.up) * Random.Range(-1f, 1f) * 6);
                    newBuoys[i].gameObject.SetActive(true);
                }
                else
                {
                    newBuoys[i].gameObject.SetActive(false);
                }
            }
        }

        lastNBPos = playerTrans.position;
    }
    void GenerateBuoyInstances()
    {
        ClearBuoysInstances();
        buoyInstances = new Transform[buoyInstanceCount];
        for (int i = 0; i < buoyInstanceCount; i++)
        {
            buoyInstances[i] = Instantiate(BuoyPrefab, Vector3.zero, Quaternion.identity, buoyHolder).transform;
            buoyInstances[i].gameObject.SetActive(false);
        }

    }
    void ClearBuoysInstances()
    {
        for (int i = 0; i < buoyInstances.Length; i++)
        {
            Destroy(buoyInstances[i].gameObject);
        }
        buoyInstances = new Transform[0];
    }
}
