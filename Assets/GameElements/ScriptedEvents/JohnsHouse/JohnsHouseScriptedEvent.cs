using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JohnsHouseScriptedEvent : ScriptedEvent
{
    public Transform frontRoomDoor;
    public Transform bathroomDoor;
    public Transform hallwayDoor;
    public Transform peepLookAt;
    public Transform playerRevertPos;
    public Animation cMAnim;
    public DemonicEntity cornerMonster;
    public float doorOpenSpeed = 5;
    public float doorCloseSpeed = 5;
    float startPeep = 0;
    bool isPeeping;
    bool cMEnumFlag;
    bool withinRange;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = triggerLocations[0].position;
        transform.rotation = triggerLocations[0].rotation;
        transform.localScale = triggerLocations[0].localScale;
        bathroomDoor.localRotation = Quaternion.Euler(0,16.37f,0);
        hallwayDoor.localRotation = Quaternion.Euler(0,-98,0);
        frontRoomDoor.localRotation = Quaternion.Euler(0,104,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.gM.player.inBoat && Vector3.Distance(GameManager.gM.player.transform.position, transform.position) < 10)
        {
            if (!withinRange)
            {
                withinRange = true; GameManager.gM.player.boat.lanternInter.showLight = false;
            }
        }
        else
        {
            if (withinRange)
            {
                withinRange = false; GameManager.gM.player.boat.lanternInter.showLight = true;
            }
        }
        if (stage == 0)
        {
            if (playerInTrigger && GameManager.gM.progMan.mainQLStage >= 3) //GetCross
            {
                stage = 1;
                transform.position = triggerLocations[1].position;
                transform.rotation = triggerLocations[1].rotation;
                //Slam door shut
                frontRoomDoor.localRotation = Quaternion.identity;
            }
        }
        else if (stage == 1)
        {
            hallwayDoor.localRotation = Quaternion.Euler(0,0,0);
            bathroomDoor.localRotation = Quaternion.Euler(0,95,0);
            frontRoomDoor.localRotation = Quaternion.Lerp(frontRoomDoor.localRotation, Quaternion.Euler(0,16.37f,0),doorOpenSpeed * Time.deltaTime);
            bool wasPeeping = isPeeping;
            isPeeping = (playerInTrigger && Vector3.Distance(GameManager.gM.player.head.forward, (peepLookAt.position - GameManager.gM.player.head.position).normalized)  < 0.3f); //Walk over to door
            if (isPeeping && !wasPeeping && startPeep == 0)
            {
                startPeep = Time.time;
            }
            if (isPeeping && Time.time - startPeep > 3)
            {
                stage = 2;
                transform.position = triggerLocations[2].position;
                transform.rotation = triggerLocations[2].rotation;
                transform.localScale = triggerLocations[2].localScale;
            }
        }
        else if (stage == 2)
        {
            hallwayDoor.localRotation = Quaternion.Lerp(hallwayDoor.localRotation, Quaternion.Euler(0,0,0),doorCloseSpeed * Time.deltaTime);
            bathroomDoor.localRotation = Quaternion.Euler(0,95,0);
            frontRoomDoor.localRotation = Quaternion.Lerp(frontRoomDoor.localRotation, Quaternion.Euler(0,95f,0),doorOpenSpeed * Time.deltaTime);
            if (cornerMonster.condemnLevel > 2 && !cMEnumFlag)
            {
                StartCoroutine(CornerMonsterCloseDoorIE());
                
            }
        }
        else if (stage == 3)
        {
            bathroomDoor.localRotation = Quaternion.Lerp(bathroomDoor.localRotation, Quaternion.Euler(0,0,0),doorCloseSpeed * Time.deltaTime);
            hallwayDoor.localRotation = Quaternion.Lerp(hallwayDoor.localRotation, Quaternion.Euler(0,-95f,0),doorOpenSpeed * Time.deltaTime);
            if (playerInTrigger)
            {
                GameManager.gM.player.transform.position = playerRevertPos.position;
            }
        }
    }
    public override void EnterTrigger()
    {
        base.EnterTrigger();
        
    }
    IEnumerator CornerMonsterCloseDoorIE()
    {
        cMEnumFlag = true;
        cMAnim.CrossFade("CornerMonsterCloseDoor",0.01f);
        yield return new WaitForSeconds(0.5f);
        //bathroomDoor.localRotation = Quaternion.Euler(0,0,0);
        if (playerInTrigger)
        {

        }
        stage = 3;
        cMEnumFlag = false;
    }
}
