using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    float lastTimeChecked;
    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 10 || col.gameObject.tag == "Boat")
        {
            if (Time.time - lastTimeChecked > 2)
            {
                lastTimeChecked = Time.time;
                print("CHECKPOINT");
                GameManager.gM.saveManager.SaveProgress();
            }
        }
    }
}
