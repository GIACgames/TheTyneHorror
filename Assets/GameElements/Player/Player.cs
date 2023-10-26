using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PossibleInter
{
    public Interactable inter;
    public float timeDet;
    public PossibleInter(Interactable i, float td)
    {
        inter = i;
        timeDet = td;
    }
}
public class Player : MonoBehaviour
{
    public Rigidbody pRb;
    public Boat boat;
    public Transform head;
    public Transform body;
    public bool inBoat = true;
    public Animator handAnim;
    public bool isGrabbing;

    public Color[] interCols;
    public PlayerHand[] playerHands;
    public Interactable grabbedObj;
    public Transform bodCrossDrop;
    public Transform itemEventTarget;
    public Vector2 lookSensitivity;
    public Interactable selectedInteractable;
    public List<PossibleInter> possibleInters;
    public float interactDistance;
    public Transform itemHolder;
    public Transform itemPOV;
    public float interViewDistance;
    public LayerMask interLM;
    public Material outlineMat;
    Vector2 curSimVeloc;
    public float walkSpeed = 5;
    public float sprintMultiplier = 2;
    public float walkAccel = 1;
    public float headBobAmount;
    public float headBobSpeed;
    float headBobVal;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        for (int i = 0; i < playerHands.Length; i++)
        {
            playerHands[i].handIndex = i;
        }
    }
    void LateUpdate()
    {
        itemPOV.position = head.position;
    }
    // Update is called once per frame
    void Update()
    {
        
        ManageBody();
        ManageLook();
        if (grabbedObj != null)
        {
            
        }
        DetectInteractables();
        
        ManageGrabbedObject();

    }
    public void ManageBody()
    {
        if (inBoat)
        {
            boat.playerInBoat = true;
            if (pRb != null)
            {
                Destroy(pRb);
                head.localPosition = new Vector3(0,1.096f, 0);
            }
            if (body.parent != boat.playerHolder)
            {
                body.parent = boat.playerHolder;
                body.localPosition = Vector3.zero;
                body.localRotation = Quaternion.identity;
                
            }
        }
        else
        {
            if (boat != null)
            {
                boat.playerInBoat = false;
            }
            if (pRb == null)
            {
                pRb = body.gameObject.AddComponent<Rigidbody>();
                pRb.constraints = RigidbodyConstraints.FreezeRotation;
                head.localPosition = new Vector3(0,1.7f, 0);
            }
            if (body.parent == boat.playerHolder)
            {
                body.parent = null;
                
                //body.localPosition = Vector3.zero;
                //body.localRotation = Quaternion.identity;
            }
            curSimVeloc = Vector2.zero;
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            curSimVeloc.x += moveInput.x;
            curSimVeloc.y += moveInput.y;
            float moveSpeed = walkSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed *= sprintMultiplier;
            }
            
            Vector3 localVeloc = body.InverseTransformDirection(pRb.velocity);
            localVeloc = new Vector3(curSimVeloc.x * (moveSpeed * 0.75f), localVeloc.y, curSimVeloc.y * moveSpeed);
            pRb.velocity = Vector3.MoveTowards(pRb.velocity, body.TransformDirection(localVeloc), walkAccel * Time.deltaTime);
            if (curSimVeloc.y != 0)
            {
                headBobVal =  Mathf.Lerp(headBobVal, Mathf.Sin(Time.time * moveSpeed * headBobSpeed) * curSimVeloc.y * headBobAmount, 30f * Time.deltaTime);
            }
            else
            {
                headBobVal = Mathf.Lerp(headBobVal,0, 30f * Time.deltaTime);
            }
            head.localPosition = new Vector3(0,1.7f + headBobVal, 0);
        }
    }
    public void ManageLook()
    {
        bool noLookBehind = (inBoat && boat != null && boat.isHoldingOars);
        Vector2 lookInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
        float bodYSpeedMultiplier = 1;
        float bodY = body.localEulerAngles.y;
        if (noLookBehind)
        {
            if (bodY < 180)
            {
                if (lookInput.x > 0) {bodYSpeedMultiplier = 1 - Math.Clamp(bodY / 85, 0, 1);}
            }
            else
            {
                if (lookInput.x < 0) {bodYSpeedMultiplier = Math.Clamp((bodY - 275) / 85, 0, 1);}
            }
        }
        bodY = (body.localEulerAngles.y + (lookInput.x * Time.deltaTime * lookSensitivity.x * bodYSpeedMultiplier)) % 360;
        if (noLookBehind)
        {
            if (bodY < 180 && bodY > 85) {bodY = 85;}
            else if (bodY > 180 && bodY < 275) {bodY = 275;}
        }
        body.localRotation = Quaternion.Euler(0,bodY, 0);

        float headXSpeedMultiplier = 1;
        float headX = head.localEulerAngles.x;
        //print(headX);
        if (headX < 180)
        {
            if (lookInput.y > 0) {headXSpeedMultiplier = 1 - Math.Clamp(headX / 75, 0, 1);}
        }
        else
        {
            if (lookInput.y < 0) {headXSpeedMultiplier = Math.Clamp((headX - 275) / 85, 0, 1);}
        }
        headX = head.localEulerAngles.x + (lookInput.y * Time.deltaTime * lookSensitivity.y * headXSpeedMultiplier);
        if (headX < 180 && headX > 75) {headX = 75;}
        else if (headX > 180 && headX < 275) {headX = 275;}
        if (Time.time < 1) {headX = 0;}
        head.localRotation = Quaternion.Euler(headX, 0, 0);
    }
    public void ManageGrabbedObject()
    {
        if (grabbedObj != null)
        {
            if (((Input.GetKeyDown("e") && selectedInteractable == null) || Input.GetKeyDown("q")) && !GameManager.gM.transitionManager.fadeEnumFlag)
            {
            LetGo();}
            else
            {
                if (selectedInteractable != null)
                {
                    if (!GameManager.gM.transitionManager.fadeEnumFlag && Input.GetKeyDown("e") || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                    {
                        selectedInteractable.BeginUseItem(grabbedObj);
                    }
                    else if ((Input.GetKeyUp("e") || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
                    {
                        selectedInteractable.StopUseItem(grabbedObj);
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                    //print("Clicked");
                    grabbedObj.PIClickDown();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    grabbedObj.PIClickUp();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    grabbedObj.PIRClickDown();
                }
                if (Input.GetMouseButtonUp(1))
                {
                    grabbedObj.PIRClickUp();
                }
            }
        }
        if (selectedInteractable != null)
        {
            if ((Input.GetKeyDown("e") || (selectedInteractable.interId == 8 && Input.GetKeyDown("space"))) && !GameManager.gM.transitionManager.fadeEnumFlag)
            {
                AttemptInter(selectedInteractable);
            }
        }

        Vector3 disp = head.position - itemPOV.position;
        float maxIPOVDist = 0.1f; float dist = disp.magnitude;
        if (dist > maxIPOVDist)
        {
        // itemPOV.position += (disp.normalized * (dist - maxIPOVDist));
        }
        //Vector3.Lerp(itemPOV.position, head.position, 0.1f);
        itemPOV.rotation = Quaternion.Lerp(itemPOV.rotation, head.rotation, 0.1f);
        if (grabbedObj != null && grabbedObj.pickedUp)
        {
            if (itemEventTarget == null)
            {
                grabbedObj.interTrans.parent = itemHolder;
            }
            else
            {
                grabbedObj.interTrans.parent = itemEventTarget;
            }
            grabbedObj.interTrans.localPosition = Vector3.zero;
            grabbedObj.interTrans.localRotation = Quaternion.identity;
        }
    }
    bool IntInArray(int i, int[] array)
    {
        foreach (int element in array)
        {
            if (i == element) {return true;}
        }
        return false;
    }
    public void DetectInteractables()
    {
        Collider[] colliders = Physics.OverlapSphere(head.position, 5, interLM, QueryTriggerInteraction.Collide);
        foreach (Collider col in colliders)
        {
            
            Interactable inter = col.GetComponent<Interactable>();
            if (inter != null && inter.selectable && (!inter.boatOnly || inBoat) && inter != grabbedObj && (inter.interType == 0 || (grabbedObj != null &&  IntInArray(inter.interType, grabbedObj.pickedUpInterTypes))))
            {
                Vector3 disp = inter.interPoint.position - head.position;
                float dist = disp.magnitude; float viewDist = Vector3.Distance(disp.normalized,head.forward);
                //print("Detected " + col.gameObject.name + " viewDist: " + viewDist);
                if (dist < inter.interDist && viewDist < inter.interViewDist * 2)
                {
                    
                    bool alreadyAdded = false;
                    foreach (PossibleInter pInter in possibleInters)
                    {
                        if (pInter.inter == inter)
                        {
                            alreadyAdded = true;
                            pInter.timeDet = Time.time;
                        }
                    }
                    if (!alreadyAdded)
                    {
                        possibleInters.Add(new PossibleInter(inter,Time.time));
                    }
                }
            }
        }
        float smallestDist = interactDistance;
        float smallestViewDist = interViewDistance;
        Interactable lastSelectedInter = selectedInteractable;
        selectedInteractable = null;
        int pIToRemove = -1;
        for (int pI = 0; pI < possibleInters.Count; pI++)
        {
            PossibleInter pInter = possibleInters[pI];
            if (!pInter.inter.selectable || !(!pInter.inter.boatOnly || inBoat) || Time.time - pInter.timeDet > 0.1f || pInter.inter == grabbedObj || !(pInter.inter.interType == 0 || (grabbedObj != null &&  IntInArray(pInter.inter.interType, grabbedObj.pickedUpInterTypes))))
            {
                pIToRemove = pI;
            }
            else
            {
                Vector3 disp = pInter.inter.interPoint.position - head.position;
                float dist = disp.magnitude; float viewDist = Vector3.Distance(disp.normalized,head.forward);
                if (viewDist < smallestViewDist || (viewDist == smallestViewDist && dist < smallestDist))
                {
                    smallestDist = dist;
                    smallestViewDist = viewDist;
                    selectedInteractable = pInter.inter;
                }
            }
        }
        if (pIToRemove != -1)
        {
            possibleInters.Remove(possibleInters[pIToRemove]);
        }
        if (selectedInteractable == null)
        {
            if (Physics.Raycast(head.position, head.forward, out RaycastHit hit, interactDistance * 10, interLM, QueryTriggerInteraction.Ignore))
            {
                //print("HIT " + hit.collider.gameObject.name);
                Interactable inter = hit.collider.gameObject.GetComponent<Interactable>();
                if (inter != null && inter.selectable && (!inter.boatOnly || inBoat) && inter != grabbedObj && (inter.interType == 0 || (grabbedObj != null &&  IntInArray(inter.interType, grabbedObj.pickedUpInterTypes))))
                {
                    selectedInteractable = inter;
                }
            }
        }
        if (selectedInteractable != lastSelectedInter)
        {
            SwitchSelectedInteractable(lastSelectedInter);
        }
    }
    public void SwitchSelectedInteractable(Interactable lastSInter)
    {
        if (lastSInter != null){lastSInter.DeSelect();}
        if (selectedInteractable != null) {selectedInteractable.Select(this, interCols[selectedInteractable.interType]);}
    }
    public void AttemptInter(Interactable inter)
    {
        Interact(inter);
        if (inter.grabbable)
        {
            Grab(inter);
        }
    }
    public void Interact(Interactable inter)
    {
        if (inter != null)
        {
            inter.Interact(this);
        }
    }
    public void Grab(Interactable inter)
    {
        if (grabbedObj != null)
        {
            LetGo();
        }
        for (int h = 0; h < playerHands.Length; h++)
        {
            playerHands[h].isGrabbing = true;
            playerHands[h].heldInter = inter;
        }
        inter.player = this;
        inter.grabbed = true;
        inter.Grab();
        grabbedObj = inter;
        if (inter.pickupable)
        {
            inter.PickUp();
            inter.interTrans.parent = itemHolder;
            inter.interTrans.localPosition = Vector3.zero;
            inter.interTrans.localRotation = Quaternion.identity;
        }
        inter.DeSelect();
    }
    public void LetGo()
    {
        for (int h = 0; h < playerHands.Length; h++)
        {
            playerHands[h].isGrabbing = false;
            playerHands[h].heldInter = null;
        }
        if (grabbedObj != null)
        {
            grabbedObj.LetGo();
            grabbedObj = null;                                              
        }
        isGrabbing = false;
    }
    public void GrabOars()
    {
        if (!boat.isHoldingOars) {boat.GrabOars();}
        isGrabbing = true;
    }
    public void OnAnimUpdate(float frameDelta)
    {

    }
    public void ExitBoat(Vector3 exitPos,Quaternion exitRot, Transform dockPos = null, int exId = -1)
    {
        if (boat.isHoldingOars)
        {
            LetGo();
        }
        StartCoroutine(ExitBoatIE(exitPos,exitRot,dockPos,exId));
    }
    IEnumerator ExitBoatIE(Vector3 exitPos,Quaternion exitRot, Transform dockPos = null, int exId = -1)
    {
        GameManager.gM.transitionManager.FadeTransition(0.3f);
        while (!GameManager.gM.transitionManager.fullyFaded)
        {
            yield return null;
        }
        inBoat = false;
        body.parent = null;
        body.position = exitPos;
        body.rotation = exitRot;
        if (dockPos != null)
        {
            boat.transform.position = dockPos.position;
            boat.transform.rotation = dockPos.rotation;
            boat.rb.velocity = Vector3.zero;
        }
    }
    public void EnterBoat()
    {
        StartCoroutine(EnterBoatIE());
    }
    public IEnumerator EnterBoatIE()
    {
        GameManager.gM.transitionManager.FadeTransition(0.3f);
        while (!GameManager.gM.transitionManager.fullyFaded)
        {
            yield return null;
        }
        inBoat = true;
    }
}
