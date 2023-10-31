using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class crowBehaviour : MonoBehaviour
{
    public bool EventTrigger;

    public GameObject player;
    public int flySpeed;

    private bool flyOverPlayerEventEnumFlag;

    public void Start()
    {
        EventTrigger = false;
        flyOverPlayerEventEnumFlag = false;
    }
    public void Update()
    {

        if (EventTrigger)
        {
            eventTrigger();
        }
        EventTrigger = false;
    }


    public void eventTrigger() // Call this function from outside this script to start the enum event
    {
        if (!flyOverPlayerEventEnumFlag)
        {
            StartCoroutine(flyOverPlayerEventEnum());
        }
    }

    public IEnumerator flyOverPlayerEventEnum()
        // Enum behaviour for the crow to fly over the player at given speed 
    {
        flyOverPlayerEventEnumFlag = true; // Enun Flag

        // Get direction of the player from the crow
        Vector3 directionOfPlayer = Vector3.up * 10;
        transform.position = player.transform.position + (Vector3.right * 19) + (Vector3.up * 2f) + (Vector3.forward * 4);

        // Move toward player
        while (directionOfPlayer.magnitude >= 1f)
        {
            // Renew the direction of the player
            directionOfPlayer = player.transform.position - transform.position;
            directionOfPlayer.y = 0f;
            

            // Change facing direction of the crow toward player
            transform.rotation = Quaternion.LookRotation(directionOfPlayer);
            
            // Move toward the player and past the player to given position
            this.transform.position += directionOfPlayer.normalized * flySpeed * Time.deltaTime;
            
            yield return null;
        }
        GameManager.gM.sfxManager.PlaySoundAtPoint("AmbientSpook", "General", Camera.main.transform.position, 2, 90, false, null, 0);
        Debug.Log("Play SFX"); // REPLACE: with the SFX call

        // Move away from player
        while(Vector3.Distance(transform.position, player.transform.position) < 40f)
        {
            transform.position += flySpeed * Time.deltaTime * transform.forward; // Keep moving forward away from the player

            yield return null;
        }

        Destroy(this.gameObject);
        flyOverPlayerEventEnumFlag = false;
        yield return null;
    }
}
