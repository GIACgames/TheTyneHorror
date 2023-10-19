using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public Player player;
    public PlayerHand oppositeHand;
    public Interactable heldInter;
    public bool isGrabbing;
    public Transform modelHand;
    public int handIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (heldInter != null && heldInter.grabbed)
        {
            if (!modelHand.gameObject.active) {modelHand.gameObject.SetActive(true);}
            modelHand.position = heldInter.handPoses[handIndex].position;
            modelHand.rotation = heldInter.handPoses[handIndex].rotation;
        }
        else
        {
            if (modelHand.gameObject.active) {modelHand.gameObject.SetActive(false);}
        }
    }
}
