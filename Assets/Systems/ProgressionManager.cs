using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public int mainQLStage;
    int lastMQLStage = -1;
    public NPC oDJohn;
    public BoatBlocker bb;
    public Boat boat;

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
            boat.progSpeedMulti = 0.7f;
        }
        else if (mainQLStage == 1) //Alison Fallen Out Of boat
        {
            bb.isBlocking = true;
            oDJohn.stage = 0;
            boat.progSpeedMulti = 1f;
        } 
        else if (mainQLStage == 2) //PickedUpCross
        {
            bb.isBlocking = false;
            oDJohn.stage = 1;
            boat.progSpeedMulti = 1f;
        }
    }
}