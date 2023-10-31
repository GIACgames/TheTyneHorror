using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressionManager : MonoBehaviour
{
    public bool testDeathTrigger;
    public int mainQLStage;
    int lastMQLStage = -1;
    public NPC oDJohn;
    public BoatBlocker bb;
    public Boat boat;
    public float lowestX;
    float lastRevWarning;
    public crowBehaviour crow;
    bool hasCrowed;
    

    // Start is called before the first frame update
    void Start()
    {
        lowestX = GameManager.gM.player.transform.position.x;
    }
    void Update()
    {
        if (testDeathTrigger)
        {
            testDeathTrigger = false;
            PlayerDeath();
        }
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
        else if (mainQLStage == 2)
        {
            //if (!hasCrowed && crow != null && lowestX < 1300 && lowestX > 1280) {crow.eventTrigger();}
            if (!hasCrowed && crow != null && lowestX < 1200 && lowestX > 1180) {crow.eventTrigger();}
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
    public void PlayerDeath()
    {
        StartCoroutine(PlayerDeathIE());
    }
    public IEnumerator PlayerDeathIE()
    {
        GameManager.gM.player.isDead = true;
        GameManager.gM.transitionManager.FadeTransition(1);
        while (!GameManager.gM.transitionManager.fullyFaded)
        {
            yield return null;
        }
        SceneManager.LoadScene(1);
    }
    
}