using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsSE : ScriptedEvent
{
    public Image blackBkg;
    public RectTransform creditsRoll;
    public bool isRolling;
    public float rollSpeed = 1;
    public float rollAccel = 1;
    public float yPosStopRoll;
    float accSpeed;
    bool finishedRolling;
    // Start is called before the first frame update
    void Start()
    {
        blackBkg.enabled = false;
        creditsRoll.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isRolling)
        {
            if (creditsRoll.position.y < yPosStopRoll){
            accSpeed = Mathf.Lerp(accSpeed, rollSpeed, rollAccel * Time.deltaTime);}
            else {accSpeed = Mathf.Lerp(accSpeed, 0, rollAccel * Time.deltaTime); finishedRolling = true;}
            creditsRoll.position += Vector3.up * Time.deltaTime * accSpeed;
        }
    }
    public void CutToCredits()
    {
        StartCoroutine(CutToCreditsIE());
    }
    IEnumerator CutToCreditsIE()
    {
        GameManager.gM.transitionManager.FadeTransition(1f, 0);
        while (!GameManager.gM.transitionManager.fullyFaded)
        {
            yield return null;
        }
        blackBkg.enabled = true;
        creditsRoll.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2);
        isRolling = true;
        GetComponent<AudioSource>().Play();
        while (!finishedRolling)
        {
            yield return null;
        }
        //yield return new WaitForSecondsRealtime(1 + Random.Range(0f,3f));
         yield return new WaitForSecondsRealtime(2f);
        GameManager.gM.transitionManager.FadeTransition(2);
        
        while (!GameManager.gM.transitionManager.fullyFaded)
        {
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(0);
    }
}
