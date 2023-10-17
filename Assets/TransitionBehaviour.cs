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


    public bool testTrigger; // Testing

    // Start is called before the first frame update
    void Start()
    {
        fadeObjectImage.gameObject.SetActive(false);
        fadeEnumFlag = false; // Reset enum flag
        fadeObjectImage.color = new Color(fadeObjectImage.color.r, fadeObjectImage.color.g, fadeObjectImage.color.b, 0f); // Set opacity to 0

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
            fadeObjectImage.color = new Color(fadeObjectImage.color.r, fadeObjectImage.color.g, fadeObjectImage.color.b, Mathf.Lerp(fadeObjectImage.color.a, 1f, fadeSpeed * Time.deltaTime));
            yield return null;
        }


        // Mark when full 100% opacity
        fullyFaded = true;
        yield return new WaitForSeconds(interval);
        fullyFaded = false;

        // Fade out
        while (fadeObjectImage.color.a >= 0.01f)
        {
            fadeObjectImage.color = new Color(fadeObjectImage.color.r, fadeObjectImage.color.g, fadeObjectImage.color.b, Mathf.Lerp(fadeObjectImage.color.a, 0f, fadeSpeed * Time.deltaTime));
            yield return null;
        }


        // End
        fadeEnumFlag = false;
        fadeObjectImage.gameObject.SetActive(false);
        yield return null;
    }

}
