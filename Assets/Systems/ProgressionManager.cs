using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public int mainQLStage;
    int lastMQLStage = -1;
    public NPC oDJohn;
    public BoatBlocker bb;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        if (lastMQLStage != mainQLStage)
        {
            SetUpStage();
            lastMQLStage = mainQLStage;
        }

        if (mainQLStage == 0)
        {
        }
    }
    void SetUpStage()
    {
        if (mainQLStage == 0)
        {
            bb.isBlocking = true;
            oDJohn.stage = 0;
        }
        else if (mainQLStage == 1)
        {
            bb.isBlocking = false;
            oDJohn.stage = 1;
        }
    }
}