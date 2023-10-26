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
    public float lowestX;
    float lastRevWarning;

    // Start is called before the first frame update
    void Start()
    {
        lowestX = GameManager.gM.player.transform.position.x;
    }
    void Update()
    {
        if (GameManager.gM.player.transform.position.x < lowestX)
        {
            lowestX = GameManager.gM.player.transform.position.x;
        }
        if (lastMQLStage != mainQLStage)
        {
            SetUpStage();
            lastMQLStage = mainQLStage;
        }

        if (mainQLStage == 0)
        {
            if (GameManager.gM.player.transform.position.x > lowestX + 4 && Time.time - lastRevWarning > 5)
            {
                GameManager.gM.hintManager.ShowHint("FOLLOW THE CURRENT", 4.2f);
                lastRevWarning = Time.time;
            }
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
        else if (mainQLStage == 2) //FirstGoblinBlowout
        {
            bb.isBlocking = true;
            oDJohn.stage = 0;
            boat.progSpeedMulti = 1f;
        } 
        else if (mainQLStage == 3) //PickedUpCross
        {
            bb.isBlocking = false;
            oDJohn.stage = 1;
            boat.progSpeedMulti = 1f;
        }
    }

    
}