using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public Rigidbody rb;
    public float boyancy = 30;
    public bool tempFloater;
    public bool keepUpDirection;
    public float reorientSpeed;
    public Transform[] normalRayPoints;
    public bool inWater;
    bool wasInWater;
    float origDrag;
    float origAngDrag;

    void Start()
    {
        origDrag = rb.drag;
        origAngDrag = rb.angularDrag;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (tempFloater) {inWater = transform.position.y < 1;}
        if (inWater)
        {
            if (!wasInWater)
            {
                EnterWater();
            }
            float surfaceYlevel = GameManager.gM.waterManager.GetWaveHeight(transform.position);
            //transform.position = new Vector3(transform.position.x, surfaceYlevel, transform.position.z);
            float yDif = surfaceYlevel - transform.position.y;
            if (yDif > 0 || rb == null)
            {
                float targYVeloc = yDif * Time.fixedDeltaTime;
                //rb.velocity += Vector3.up * (yDif * yDif * Time.deltaTime * boyancy);
                if (rb != null) {rb.velocity = new Vector3(rb.velocity.x, Mathf.Lerp(rb.velocity.y, targYVeloc * 200 * yDif, Time.deltaTime * boyancy / yDif), rb.velocity.z);}
                else {transform.position = new Vector3(transform.position.x, surfaceYlevel, transform.position.z);}
                

                if (keepUpDirection)
                {
                    Vector3 surfaceNormal;
                    if (normalRayPoints.Length == 0)
                    {
                        surfaceNormal = GameManager.gM.waterManager.GetWaveNormal(transform.position, 0.4f);
                    }
                    else
                    {
                        surfaceNormal = GameManager.gM.waterManager.GetWaveNormalMP(normalRayPoints);
                    }
                    if (rb != null) {transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation, reorientSpeed * Time.deltaTime);}
                    else {transform.rotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;}
                }
            }
        }
        else
        {
            if (wasInWater)
            {
                ExitWater();
            }
        }
        wasInWater = inWater;
    }
    public void ExitWater()
    {
        rb.angularDrag = origAngDrag;
        rb.drag = origDrag;
        inWater = false;
    }
    public void EnterWater()
    {
        rb.angularDrag = origAngDrag * 3;
        rb.drag = origDrag * 3;
        inWater = true;
    }
}
