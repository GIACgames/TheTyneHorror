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
    public Boat boat;
    public Transform head;
    public Transform body;
    public Animator handAnim;
    public bool isGrabbing;

    public Color[] interCols;
    public PlayerHand[] playerHands;
    public Interactable grabbedObj;
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
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        for (int i = 0; i < playerHands.Length; i++)
        {
            playerHands[i].handIndex = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        ManageLook();
        if (grabbedObj != null)
        {
            
        }
        DetectInteractables();
        
        ManageGrabbedObject();

    }
    public void ManageLook()
    {
        bool noLookBehind = (boat != null && boat.isHoldingOars);
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
            if ((Input.GetKeyDown("e") && selectedInteractable == null) || Input.GetKeyDown("q"))
            {
            LetGo();}
            else
            {
                if (selectedInteractable != null)
                {
                    if (Input.GetKeyDown("e") || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                    {
                        selectedInteractable.BeginUseItem(grabbedObj);
                    }
                    else if (Input.GetKeyUp("e") || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
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
            if (Input.GetKeyDown("e"))
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
        itemPOV.position = Vector3.Lerp(itemPOV.position, head.position, 0.1f);
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
            if (inter != null && inter.selectable && inter != grabbedObj && (inter.interType == 0 || (grabbedObj != null &&  IntInArray(inter.interType, grabbedObj.pickedUpInterTypes))))
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
            if (!pInter.inter.selectable || Time.time - pInter.timeDet > 0.1f || pInter.inter == grabbedObj || !(pInter.inter.interType == 0 || (grabbedObj != null &&  IntInArray(pInter.inter.interType, grabbedObj.pickedUpInterTypes))))
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
                if (inter != null && inter.selectable && inter != grabbedObj && (inter.interType == 0 || (grabbedObj != null &&  IntInArray(inter.interType, grabbedObj.pickedUpInterTypes))))
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
}
