using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gM;
    public Player player;
    public DialogueHandler dialogueHandler;
    // Start is called before the first frame update
    void Start()
    {
        gM = this;
    }

}
