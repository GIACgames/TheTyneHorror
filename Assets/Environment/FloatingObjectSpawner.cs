using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObjectSpawner : MonoBehaviour
{
    public bool spawnOilCan;
    public GameObject oilCanPrefab;
    GameObject curOilCan;
    OilInteractable curOilInter;
    public Vector3 lastSpawnPlayerPos;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnOilCan)
        {
            if (curOilInter == null)
            {
                lastSpawnPlayerPos = GameManager.gM.player.transform.position;
                curOilCan = Instantiate(oilCanPrefab, lastSpawnPlayerPos - (Vector3.right * 20) + (Vector3.forward * Random.Range(-1f,1f) * 6), Quaternion.identity);
                curOilInter = curOilCan.transform.GetChild(0).GetComponent<OilInteractable>();
            }
            if (curOilInter.addedToBoat || curOilInter.pickedUp)
            {
                spawnOilCan = false;
                curOilCan = null;
                curOilInter = null;
            }
            else
            {
                if (curOilCan.transform.position.x > GameManager.gM.player.transform.position.x + 15) {Destroy(curOilCan);}
                else if (curOilCan.transform.position.x < GameManager.gM.player.transform.position.x)
                {
                    curOilCan.GetComponent<FloatingObject>().targetPos = new Vector3(curOilCan.transform.position.x, curOilCan.transform.position.y, GameManager.gM.player.transform.position.z);
                    curOilCan.GetComponent<FloatingObject>().targetSpeed = 0.001f;
                }
                else
                {
                    curOilCan.GetComponent<FloatingObject>().targetSpeed = 0;
                }
            }
        }
    }
}