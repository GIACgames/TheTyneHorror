using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantHandsEatAttack : MonoBehaviour
{
    public DemonicEntity[] hands;
    public Animator anim;
    public Transform head;
    public int attackStage;
    public Transform[] handTargPoses;
    public Vector3[] handLockPoses; //Where the hands will pull toward
    public Rigidbody boatRb;
    public Boat boat;
    public Transform headTarg;
    Vector3 lockHeadPos;
    public float pullForce = 0.3f;
    public float pullSpeed = 3;
    Quaternion boatLockRot;
    Vector3 boatLockPos;
    bool bothHandsPulling;
    bool wasAttacking;
    bool hasLockedHead;
    float lastAttackEndTime;
    public Animation[] handAnims;
    // Start is called before the first frame update
    void Start()
    {
        lastAttackEndTime = -30;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackStage == 0) //Not Attacking
        {
            hasLockedHead = false;
            wasAttacking = false;
            if (Time.time - lastAttackEndTime > 40 && boat.lanternInter.dangerDarkMode)
            {
                attackStage = 1;
            }
            if (head.transform.position.y < -8)
            {
                head.gameObject.SetActive(false);
            }
        }
        else //Left Hand Grab
        {
            head.gameObject.SetActive(true);
            if (!wasAttacking) {wasAttacking = true; boatLockRot = boatRb.transform.rotation;}
            if (attackStage > 0) //Right Hand Grab
            {
                if (attackStage > 0) //Head Emerge
                {
                    if (head.transform.position.y > -1)
                    {
                        anim.SetBool("JawOpen", Vector3.Distance(boatRb.transform.position, head.position) < 11f);
                    }
                    head.transform.position = Vector3.MoveTowards(head.transform.position, lockHeadPos, 0.9f * Time.deltaTime);
                    if (attackStage == 3 && Vector3.Distance(head.transform.position, lockHeadPos) < 0.2f)
                    {
                        attackStage = 4;
                    }
                    if (attackStage > 3) //Pull into head and eat
                    {
                        
                        if (attackStage != 5 && Vector3.Distance(boatRb.transform.position, new Vector3(head.position.x,boatRb.transform.position.y,head.position.z)) < 1.5f)
                        {
                            attackStage = 5;
                        }
                        if (attackStage > 4) //CompletAttack
                        {
                            
                        }
                    }
                }
            }
            
        }
        bothHandsPulling = true;
        for (int i = 0; i < hands.Length; i++)
        {
            ManageHand(hands[i], i);
        }
        if (hasLockedHead)
        {
            if (Vector3.Distance(boatRb.transform.position, new Vector3(head.position.x,boatRb.transform.position.y,head.position.z)) > 19f)
            {   
                if (hands[0].transform.position.y < -1 && hands[1].transform.position.y < -1)
                {
                    attackStage = 0;
                    anim.SetBool("JawOpen", false);
                    lastAttackEndTime = Time.time;
                }
            }   
        }
        if (attackStage < 3 && !hasLockedHead) {head.transform.position = new Vector3(headTarg.position.x, -15, headTarg.position.z);}
        Vector3 headBoatDisp = head.position - boat.transform.position;
        headBoatDisp.y = 0;
        boatLockRot = Quaternion.LookRotation(headBoatDisp, Vector3.up);
        head.forward = Vector3.Lerp(head.forward, -headBoatDisp.normalized, 5 * Time.deltaTime);
    }
    void ManageHand(DemonicEntity hand, int i)
    {
        if (attackStage == 0) //Not Attacking
        {
            //hand.transform.position = new Vector3(0,-30,0);
            hand.begin = Time.time;
            hand.aStage = 0;
            if (hand.transform.position.y < -2)
            {
                hand.gameObject.SetActive(false);
            }
        }
        else //Left Hand Grab
        {
            hand.gameObject.SetActive(true);
            bool isPulling = false;
            if (i == 1 && attackStage == 1 && (Time.time - hands[0].begin < 5)) {hand.begin = Time.time;}
            if (Time.time - hand.begin > 2)
            {
                if (hand.aStage == 0) {hand.aStage = 1;  if (i == 1 && attackStage == 1) {attackStage = 2;}}
                if (hand.aStage == 1 || hand.aStage == 2)
                {
                    Vector3 targPos = handTargPoses[i].position + (handTargPoses[i].right * - 0.3f);
                    float newY = hand.transform.position.y;
                    float lerpSpeed = 3f;
                    if (newY < targPos.y + 0.4f && hand.aStage == 1)
                    {   
                        newY += 2f * Time.deltaTime;
                        targPos.y = newY;
                    }
                    else
                    {
                        hand.aStage = 2;
                        lerpSpeed = 5f;
                        if (Vector3.Distance(hand.transform.position, targPos) < 0.2f) {hand.aStage = 3;}
                    }
                    hand.transform.position =  new Vector3(hand.transform.position.x,newY,hand.transform.position.z);
                    //targPos.y = newY;
                    hand.transform.position = Vector3.MoveTowards(hand.transform.position, targPos, lerpSpeed * Time.deltaTime);
                }
                if (hand.aStage > 1)
                {
                    if (hand.aStage == 3) 
                    {
                        hand.transform.position = Vector3.MoveTowards(hand.transform.position, handTargPoses[i].position, 5 * Time.deltaTime);
                        hand.transform.rotation = Quaternion.Lerp(hand.transform.rotation,handTargPoses[i].rotation,10f * Time.deltaTime);
                        if (Vector3.Distance(hand.transform.position, handTargPoses[i].position) < 0.05f) {
                            handAnims[i].CrossFade("GiantHandGrab", 0.2f);
                            hand.aStage = 4; handLockPoses[i] = hand.transform.position; boatLockPos = boatRb.transform.position; if (i == 0 && !hasLockedHead) {hasLockedHead = true;lockHeadPos = headTarg.position; lockHeadPos.y = 1; } }
                    }
                    if (hand.aStage == 4) 
                    {
                        isPulling = true;
                        if (attackStage == 2 && i == 1) {attackStage = 3; boatLockRot = boatRb.transform.rotation;}
                        hand.transform.position = handTargPoses[i].position;
                        hand.transform.rotation = handTargPoses[i].rotation;
                        if (attackStage == 4)
                        {
                            boatLockPos = Vector3.MoveTowards(boatLockPos, new Vector3(head.position.x,boatLockPos.y,head.position.z), pullSpeed * Time.deltaTime * 0.5f);
                        }
                        if (hand.condemnLevel > 5) {hand.aStage = 5; handAnims[i].CrossFade("GiantHandCondemn", 0.2f);}
                    }
                    if (hand.aStage == 5) 
                    {
                        isPulling = false;
                        Vector3 nTarg = handTargPoses[i].position - (handTargPoses[i].right * 0.8f); nTarg.y = -10;
                        hand.transform.position -= handTargPoses[i].right * 5f * Time.deltaTime;
                        hand.transform.position = Vector3.MoveTowards(hand.transform.position, nTarg, 5 * Time.deltaTime);
                        if (hand.transform.position.y < -5) {hand.aStage = 0; hand.begin = Time.time + 2; handAnims[i].CrossFade("GiantHandSearching", 0.2f);}

                    }
                }
            }
            else
            {
                Vector3 startPos = new Vector3(handTargPoses[i].position.x,-5,handTargPoses[i].position.z) + (handTargPoses[i].right * - 3);
                hand.transform.position = startPos;
            }
            if (isPulling)
            {
                if (i == 1 && bothHandsPulling)
                {
                    boatRb.transform.rotation = Quaternion.RotateTowards(boatRb.transform.rotation, boatLockRot, 25f * Time.deltaTime);
                    boatRb.GetComponent<Boat>().lockedRot = true;
                }
                else {boatRb.GetComponent<Boat>().lockedRot = false;}
                //boatRb.AddForceAtPosition((handLockPoses[i] - hand.transform.position) * pullForce, hand.transform.position);
                boatRb.transform.position = Vector3.Lerp(boatRb.transform.position, new Vector3(boatLockPos.x, boatRb.transform.position.y, boatLockPos.z), pullForce);
                boatRb.velocity =  Vector3.Lerp(boatRb.velocity, Vector3.zero, 0.8f);
                hand.transform.position = handTargPoses[i].position;
                hand.transform.rotation = handTargPoses[i].rotation;

                
            }
            else
            {
                bothHandsPulling = false;
                boatRb.GetComponent<Boat>().lockedRot = false;
            }
        }
        
    }
}
