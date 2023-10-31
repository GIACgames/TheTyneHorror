using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionBehaviour : MonoBehaviour // Attach to the parent of a transition Image
{
    public Image fadeObjectImage;
    public float fadeSpeed; // Speed of the fade - set in the Inspector
    public bool fadeEnumFlag; // Mark when enumerator running
    public bool fullyFaded; // True when fully faded in
    Coroutine fadeCor;


    public bool testTrigger; // Testing

    // Start is called before the first frame update
    void Start()
    {
        fadeObjectImage.gameObject.SetActive(true);
        fadeEnumFlag = false; // Reset enum flag
        fadeObjectImage.color = new Color(fadeObjectImage.color.r, fadeObjectImage.color.g, fadeObjectImage.color.b, 1f); // Set opacity to 0
        fullyFaded = true;
        StartCoroutine(FadeToClear(1));

    }

    public void Update()
    {
        #region Test
        //if (fadeEnumFlag)
        //{
        //    return;
        //}
        //if (testTrigger)
        //{
        //    testTrigger = false;
        //    StartCoroutine(fadeEnum(2f));
        //}
        #endregion


    }
    public void FadeTransition(float interval , int fadeT = 0)
    {
        fullyFaded = false;
        if (fadeCor != null) {StopCoroutine(fadeCor);}
        if (fadeT == 0)
        {
            fadeCor = StartCoroutine(fadeEnum(interval));
        }
        else if (fadeT == 1)
        {
            fadeCor = StartCoroutine(FadeToClear(interval));
        }
        else
        {
            fadeCor = StartCoroutine(FadeToBlack(interval));
        }
    }

    public IEnumerator fadeEnum(float interval)
        /**
         * Usage:
         * check if flag is False
         * -- > if(!fadeEnumFlag)
         * then call the coroutine
         * -- > {StartCoroutine(fadeEnum(intervalAmount)}
         */
    {
        fadeEnumFlag = true;
        fadeObjectImage.gameObject.SetActive(true);


        // Fade In
        while (fadeObjectImage.color.a < 0.99f)
        {
            fadeObjectImage.color = new Color(fadeObjectImage.color.r, fadeObjectImage.color.g, fadeObjectImage.color.b, Mathf.Lerp(fadeObjectImage.color.a, 1f, fadeSpeed * Time.unscaledDeltaTime));
            yield return null;
        }


        // Mark when full 100% opacity
        fullyFaded = true;
        yield return new WaitForSecondsRealtime(interval);
        fullyFaded = false;

        // Fade out
        while (fadeObjectImage.color.a >= 0.01f)
        {
            fadeObjectImage.color = new Color(fadeObjectImage.color.r, fadeObjectImage.color.g, fadeObjectImage.color.b, Mathf.Lerp(fadeObjectImage.color.a, 0f, fadeSpeed * Time.unscaledDeltaTime));
            yield return null;
        }


        // End
        fadeEnumFlag = false;
        fadeObjectImage.gameObject.SetActive(false);
        yield return null;
        fadeCor = null;
    }
    public IEnumerator FadeToClear(float interval)
    {
        yield return new WaitForSecondsRealtime(interval);
        fullyFaded = false;
        // Fade out
        while (fadeObjectImage.color.a >= 0.01f)
        {
            fadeObjectImage.color = new Color(fadeObjectImage.color.r, fadeObjectImage.color.g, fadeObjectImage.color.b, Mathf.Lerp(fadeObjectImage.color.a, 0f, fadeSpeed * Time.unscaledDeltaTime));
            yield return null;
        }


        // End
        fadeEnumFlag = false;
        fadeObjectImage.gameObject.SetActive(false);
        yield return null;
        fadeCor = null;
    }
    public IEnumerator FadeToBlack(float interval)
    {
        yield return new WaitForSecondsRealtime(interval);
        fadeObjectImage.gameObject.SetActive(true);
        fullyFaded = false;
        // Fade out
        while (fadeObjectImage.color.a < 0.99f)
        {
            fadeObjectImage.color = new Color(fadeObjectImage.color.r, fadeObjectImage.color.g, fadeObjectImage.color.b, Mathf.Lerp(fadeObjectImage.color.a, 1f, fadeSpeed * Time.unscaledDeltaTime));
            yield return null;
        }
        fadeObjectImage.color = new Color(fadeObjectImage.color.r, fadeObjectImage.color.g, fadeObjectImage.color.b, 1f);
        fullyFaded = true;
        // End
        fadeEnumFlag = false;
        yield return null;
        fadeCor = null;
    }

}
