using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI savedText;
    public TextMeshProUGUI interPromptText;
    public bool isShowingHint;
    public float hintFlashSpeed = 5;
    public float hintFlashPower = 0.2f;
    float hintFadeOpac = 0;
    Coroutine hintCor;
    public bool showInterPrompt;
    public string interPrompt;
    void Start()
    {
        savedText.gameObject.SetActive(false);
        hintText.gameObject.SetActive(false);
        //yield return new WaitForSeconds(4);
       // ShowHint("[E] TO INTERACT", 10);
       // yield return new WaitForSeconds(12);
       // ShowHint("[WASD] TO MOVE WHILE ROWING", 10);

    }
    void Update()
    {
        hintText.color = new Color(hintText.color.r, hintText.color.g, hintText.color.b, hintFadeOpac * (0.8f + (Mathf.Sin(Time.time * hintFlashSpeed) * hintFlashPower)));
        hintFadeOpac = Mathf.Lerp(hintFadeOpac, isShowingHint ? 1: 0, Time.time * 0.02f);
        hintText.gameObject.SetActive(hintFadeOpac > 0.001f);
        
        interPromptText.gameObject.SetActive(showInterPrompt);
        interPromptText.text = interPrompt;
    }
    public void ShowHint(string hint, float timeToShow)
    {
        if (hintCor != null) {StopCoroutine(hintCor);}
        hintCor = StartCoroutine(ShowHintIE(hint, timeToShow));
    }
    IEnumerator ShowHintIE(string hint, float timeToShow)
    {
        isShowingHint = true;
        hintText.text = hint;
        //hintText.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeToShow);
        //hintText.gameObject.SetActive(false);
        isShowingHint = false;
    }
    public void ShowSaveText()
    {
        StartCoroutine(ShowSavedIE());
    }
    IEnumerator ShowSavedIE()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        savedText.color = new Color(savedText.color.r, savedText.color.g, savedText.color.b, 1);
        savedText.gameObject.SetActive(true);
        while (savedText.color.a > 0.01f)
        {
            savedText.color = new Color(savedText.color.r, savedText.color.g, savedText.color.b, Mathf.Lerp(savedText.color.a, 0, 1.6f * Time.unscaledDeltaTime));
            yield return null;
        }
        savedText.gameObject.SetActive(false);
    }

}
