using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverManager : MonoBehaviour
{
    public FloatingObjectSpawner fOS;
    
    public BlowOutGremlin bOGremlin;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gM.progMan.mainQLStage >= 2 && GameManager.gM.player.transform.position.x < 1350)
        {
            float moveSpawnDist = 10;
            if (GameManager.gM.player.boat.lanternInter.level <= 20)
            {
                moveSpawnDist = 3;
            }
            if (Vector3.Distance(GameManager.gM.player.transform.position, fOS.lastSpawnPlayerPos) > moveSpawnDist)
            {
                fOS.spawnOilCan = true;
            }
        }
       
        bOGremlin.canBlowOut = GameManager.gM.player.transform.position.x < 1375;
        bOGremlin.blowOutInterval = (GameManager.gM.progMan.mainQLStage < 2 ? 10: 250);
    }
}
