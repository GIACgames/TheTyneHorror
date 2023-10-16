using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class WallGenerator : MonoBehaviour
{
    public Transform player;
    public Transform[] wallPoints;
    public GameObject wallPrefab;
    public bool debugLine;
    public bool setUpWallPoints;
    public Transform[] wallPool;
    public Transform wallPoolHolder;
    public int wallInstanceCount;
    public float wallLength;
    int curInstanceIndx = 0;
    int nearestPoint;
    int lastNearestPoint;
    public Color debugCol = Color.red;
    // Start is called before the first frame update
    void Start()
    {
        wallPool = new Transform[wallInstanceCount];
        for (int i = 0; i < wallInstanceCount; i++)
        {
            wallPool[i] = Instantiate(wallPrefab, Vector3.down * 100, Quaternion.identity, wallPoolHolder).transform;
        }
    }

    void OnDrawGizmos()
    {
        if (debugLine)
        {
            Gizmos.color = debugCol;
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
            }
        }
    }

    void Update()
    {
       
        if (setUpWallPoints)
        {
            setUpWallPoints = false;
            wallPoints = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                wallPoints[i] = transform.GetChild(i);
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        int newNearestPoint = -1;
        float nearestDist = 10000;
        for (int i = 0; i < wallPoints.Length; i++)
        {
            float dist = Vector3.Distance(wallPoints[i].position, player.position);
            if (dist < nearestDist)
            {
                newNearestPoint = i;
                nearestDist = dist;
            }
        }
        if (Application.isPlaying)
        {
        if (newNearestPoint != -1 && newNearestPoint != lastNearestPoint )
        {
            ResetWalls();
            if (newNearestPoint - 1 >= 0) {FillLine(newNearestPoint, newNearestPoint - 1);}
            if (newNearestPoint + 1 < wallPoints.Length) {FillLine(newNearestPoint, newNearestPoint + 1);}
            lastNearestPoint = newNearestPoint;
        }
        }
        
    }
    void ResetWalls()
    {
        for (int i = 0; i < wallInstanceCount; i++)
        {
            wallPool[i].position = Vector3.down * 100;
        }
    }
    void FillLine(int a, int b)
    {
        Vector3 disp = wallPoints[a].position - wallPoints[b].position;
        Vector3 dir = disp.normalized;
        float mag = disp.magnitude;
        int wallCount = Mathf.RoundToInt(mag / wallLength);
        float newWallLength = (mag / wallCount);
        float wallLengthMultiplier = newWallLength / wallLength;
        
        for (int i = 0; i < wallCount; i++)
        {
            wallPool[curInstanceIndx].position = wallPoints[b].position + (dir * (newWallLength) * (i + 0.5f));
            wallPool[curInstanceIndx].right = dir;
            //wallPool[curInstanceIndx].rotation = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(0, 90 ,0);
            bool flipZ = false;
            if (Vector3.Distance(wallPool[curInstanceIndx].forward, (wallPool[curInstanceIndx].position - player.position).normalized) > 1.5f) {flipZ = true;}
            wallPool[curInstanceIndx].localScale = new Vector3(newWallLength,wallPool[curInstanceIndx].localScale.y,Mathf.Abs(wallPool[curInstanceIndx].localScale.z) * (flipZ ? -1: 1));
            curInstanceIndx = (curInstanceIndx + 1) % wallInstanceCount;
        }
    }
}
