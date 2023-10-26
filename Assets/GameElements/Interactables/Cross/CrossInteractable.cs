using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossInteractable : Interactable
{
    public float cPower = 1;
    public Transform raycastPoint;
    public LayerMask lM;
    public float range = 5;
    public bool isPointing;
    bool lClick;
    bool rClick;
    public Transform defCrossDrop;
    public bool hasBeenPickedUp;
    void Start()
    {
        if (!hasBeenPickedUp) {dropTrans = defCrossDrop;}
        transform.parent = dropTrans;
        
    }
    void Update()
    {
        bool wasPointing = isPointing;
        isPointing = (lClick || rClick);
        if (wasPointing != isPointing) {UpdatePlayerAnim();}

        if (isPointing)
        {
            if (Physics.Raycast(raycastPoint.position, raycastPoint.forward, out RaycastHit hit, range, lM, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.GetComponent<DemonicEntity>())
                {
                    hit.collider.GetComponent<DemonicEntity>().Condemn(cPower * Time.deltaTime);
                    //print("Burnt demon " + hit.collider.gameObject.name);
                }
            }
        }
        if (hasBeenPickedUp)
        {
        if (GameManager.gM.player != null && GameManager.gM.player.inBoat)
        {
            dropTrans = GameManager.gM.player.boat.boatCrossDrop;
        }
        else
        {
            dropTrans = GameManager.gM.player.bodCrossDrop;
        }
        }
        if (transform.parent != dropTrans){transform.parent = dropTrans;}
        if (!pickedUp)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
    void UpdatePlayerAnim()
    {
        if (player != null) {player.handAnim.SetBool("isPointing", isPointing);}
    }
    public override void PIClickDown()
    {
        base.PIClickDown();
        lClick = true;
    }
    public override void PIClickUp()
    {
        base.PIClickUp();
        lClick = false;
    }
    public override void PIRClickDown()
    {
        base.PIClickDown();
        rClick = true;
    }
    public override void PIRClickUp()
    {
        base.PIClickUp();
        rClick = false;
    }
    public override void PickUp()
    {
        isPointing = false;
        if (GameManager.gM.progMan.mainQLStage < 3)
        {
            GameManager.gM.progMan.mainQLStage = 3;
        }
        hasBeenPickedUp = true;
        UpdatePlayerAnim();
        base.PickUp();
        lClick = false;
        rClick = false;
    }
    public override void LetGo()
    {
        isPointing = false;
        UpdatePlayerAnim();
        base.LetGo();
        lClick = false;
        rClick = false;
    }
}
