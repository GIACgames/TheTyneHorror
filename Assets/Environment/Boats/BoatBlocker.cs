using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatBlocker : MonoBehaviour
{
    public bool isBlocking;
    public float moveSpeed;
    public Vector3 blockPos;
    public Vector3 nonblockPos;
    public Transform boat;
    // Start is called before the first frame update
    void Start()
    {
        if (isBlocking)
        {
            boat.localPosition = blockPos;
        }
        else
        {
            boat.localPosition = nonblockPos;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isBlocking)
        {
            boat.localPosition = Vector3.Lerp(boat.localPosition, blockPos, moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            boat.localPosition = Vector3.Lerp(boat.localPosition, nonblockPos, moveSpeed * Time.fixedDeltaTime);
        }

    }
}
