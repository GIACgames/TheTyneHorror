using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused;
    bool isLoading;
    public GameObject menu;
    public CreditsSE cred;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool wasPaused = isPaused;
        if ((Input.GetKeyDown("p") || Input.GetKeyDown(KeyCode.Escape)) && !isLoading && !cred.isRolling)
        {
            isPaused = !isPaused;
        }
        if (isPaused)
        {
            if (!wasPaused)
            {
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                menu.SetActive(true);
            }
        }
        else 
        {
            if (wasPaused)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
                menu.SetActive(false);
            }
        }
    }
    public void QuitToMenu()
    {
        if (!isLoading) {StartCoroutine(QuitToMenuIE());}
    }
    IEnumerator QuitToMenuIE()
    {
        isLoading = true;
        GameManager.gM.transitionManager.FadeTransition(2);
        
        while (!GameManager.gM.transitionManager.fullyFaded)
        {
            yield return null;
        }
        
        SceneManager.LoadScene(0);
    }
    public void ReturnToCheckpoint()
    {
        if (!isLoading) {StartCoroutine(ReturnToCheckpointIE());}
    }
    IEnumerator ReturnToCheckpointIE()
    {
        isLoading = true;
        GameManager.gM.transitionManager.FadeTransition(2);
        
        while (!GameManager.gM.transitionManager.fullyFaded)
        {
            yield return null;
        }
        
        SceneManager.LoadScene(1);
    }
    public void Resume()
    {
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        menu.SetActive(false);
    }
}
