using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gM;
    public Player player;
    public DialogueHandler dialogueHandler;
    public ProgressionManager progMan;
    public SFXManager sfxManager;
    public HintManager hintManager;
    public TransitionBehaviour transitionManager;
    public WaterManager waterManager;
    public SaveManager saveManager;
    // Start is called before the first frame update
    void Awake()
    {
        gM = this;
    }
    void OnLevelWasLoaded()
    {
        gM = this;
    }

}
