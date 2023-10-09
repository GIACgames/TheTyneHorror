using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonicEntity : MonoBehaviour
{
    public float condemnLevel = 0;
    public float cLevelToBeat = 20;
    public float cLevelDrainSpeed = 1;
    public float begin = -100;
    public int aStage = 0;
    float lastCTime;
    public Material burnMat;
    public Renderer[] mRenderers;
    bool wasBurning;
    public float matScale = 12;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCTime > 0.3f)
        {
            condemnLevel = Mathf.Clamp(condemnLevel - (cLevelDrainSpeed * Time.deltaTime),0,cLevelToBeat + 100);
        }
        if (condemnLevel > 0)
        {
            if (!wasBurning)
            {
                foreach (Renderer mRend in mRenderers)
                {
                    Material[] mats = new Material[mRend.sharedMaterials.Length + 1];
                    mats[mats.Length-1] = burnMat;
                    for (int m = 0; m < mats.Length - 1; m++) {mats[m] = mRend.sharedMaterials[m];}
                    mRend.sharedMaterials = mats;
                }
            }
            burnMat.SetFloat("_Scale", matScale);
            burnMat.SetFloat("_Intensity", (condemnLevel / cLevelToBeat) * 0.5f);
            wasBurning = true;
        }
        else
        {
            if (wasBurning)
            {
                foreach (Renderer mRend in mRenderers)
                {
                    Material[] mats = new Material[mRend.sharedMaterials.Length - 1];
                    for (int m = 0; m < mats.Length; m++) {mats[m] = mRend.sharedMaterials[m];}
                    mRend.sharedMaterials = mats;
                }
            }
            wasBurning = false;
        }
    }
    public void Condemn(float cPower)
    {
        lastCTime = Time.time;
        condemnLevel += cPower;
    }
}
