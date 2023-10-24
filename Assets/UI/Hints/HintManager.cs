using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public bool isShowingHint;
    public float hintFlashSpeed = 5;
    public float hintFlashPower = 0.2f;
    float hintFadeOpac = 0;
    Coroutine hintCor;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(4);
        ShowHint("[E] TO INTERACT", 10);
        yield return new WaitForSeconds(12);
        ShowHint("[WASD] TO MOVE WHILE ROWING", 10);

    }
    void Update()
    {
        hintText.color = new Color(hintText.color.r, hintText.color.g, hintText.color.b, hintFadeOpac * (0.8f + (Mathf.Sin(Time.time * hintFlashSpeed) * hintFlashPower)));
        hintFadeOpac = Mathf.Lerp(hintFadeOpac, isShowingHint ? 1: 0, Time.time * 0.02f);
        hintText.gameObject.SetActive(hintFadeOpac > 0.001f);
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

}
