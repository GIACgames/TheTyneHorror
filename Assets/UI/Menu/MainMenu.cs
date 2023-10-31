using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public bool returningPlayer;
    public static MainMenu mM;
    public SaveManager saveManager;
    public TextMeshProUGUI startContText;
    public GameObject newGameBut;
    public TransitionBehaviour tB;
    bool isLoading;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        mM = this;
        returningPlayer = saveManager.CheckSaveExists();
        print(returningPlayer);
        if (returningPlayer) {startContText.text = "Continue"; newGameBut.SetActive(true);}
        else {startContText.text = "Start"; newGameBut.SetActive(false); saveManager.ResetProgress("Save0");}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Button0()
    {
        if (!returningPlayer) {saveManager.ResetProgress("Save0");}
        BeginGame();
    }
    public void Button1()
    {
        print("New Game");
        saveManager.ResetProgress("Save0");
        BeginGame();
    }
    public void BeginGame()
    {
        if (!isLoading)
        {
        StartCoroutine(BeginGameIE());
        }
    }
    IEnumerator BeginGameIE()
    {
        isLoading = true;
        tB.FadeTransition(2);
        while (!tB.fullyFaded)
        {
            yield return null;
        }
        SceneManager.LoadScene(1);
    }
}
