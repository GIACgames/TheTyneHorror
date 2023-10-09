using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using System.Linq;

public class Ripple
{
    public Vector3 sourcePos;
    public float height;
    public float maxRadius;
    public float curRadius;

    public Ripple(float h, float mR, float cR)
    {
        height = h; maxRadius = mR; curRadius = cR;
    }
    public void UpdateRipple(float deltaTime)
    {
        curRadius += deltaTime;
        
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WaterManager : MonoBehaviour
{
    public static WaterManager main;
    private MeshFilter meshFilter;
    public float speed = 1f;
    public float offset = 0f;
    public Transform testObject;
    public bool testRipple;
    public List<Ripple> ripples;
    
    [SerializeField] private int widthSegments = 1;
    [SerializeField] private int lengthSegments = 1;
    [SerializeField] private float width = 1f;
    [SerializeField] private float lengthh = 1f;
    public float maxDistance = 10f;
    public float detailMultiplier = 0.5f;

    public float waveHeight = 0.5f;   // The height of the waves
    public float waveLength = 1f;     // The length of the waves
    public float waveSpeed = 1f;      // The speed of the waves
    public float waveSteepness = 0.3f;// The steepness of the waves
    public int numPhases = 4;          // The number of wave phases to use

    private float[] _phases;  

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        meshFilter = GetComponent<MeshFilter>();

        _phases = new float[numPhases];
        for (int i = 0; i < numPhases; i++)
        {
            _phases[i] = Random.Range(0f, Mathf.PI * 2f);
        }
         GenerateMesh();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        waveLength = 4 + ((waveSteepness - 1) / waveSteepness);
        float wS = width / widthSegments;
        float lS = lengthh / lengthSegments;
        transform.position = new Vector3(Mathf.Floor(testObject.position.x / wS) * wS, 0, Mathf.Floor(testObject.position.z / lS) * lS);
        offset += Time.deltaTime * speed;

        WaveMesh();

        testObject.position = new Vector3(testObject.position.x, GetWaveHeight(testObject.position), testObject.position.z);
        //testObject.up = GetWaveNormal(testObject.position, 0.8f);
        if (testRipple)
        {

        }
    }
    public void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        int numVertices = (widthSegments + 1) * (lengthSegments + 1);
        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uv = new Vector2[numVertices];
        int[] triangles = new int[widthSegments * lengthSegments * 6];

        float halfWidth = width * 0.5f;
        float halfLength = lengthh * 0.5f;

        float distanceStep = maxDistance / (widthSegments * 0.5f);

        for (int z = 0, i = 0; z <= lengthSegments; z++)
        {
            float zPos = (((float)z / lengthSegments) - 0.5f) * lengthh;
            for (int x = 0; x <= widthSegments; x++, i++)
            {
                float xPos = (((float)x / widthSegments) - 0.5f) * width;

                // Calculate distance from center
                float distance = Mathf.Sqrt(xPos * xPos + zPos * zPos);
                float detailLevel = 1f - Mathf.Clamp01(distance / maxDistance);
                detailLevel = Mathf.Pow(detailLevel, detailMultiplier);

                // Scale y value by detail level
                //float y = detailLevel * Mathf.PerlinNoise(xPos, zPos) * 2f;

                vertices[i] = new Vector3(xPos, 0, zPos);
                uv[i] = new Vector2((float)x / widthSegments, (float)z / lengthSegments);
            }
        }

        for (int ti = 0, vi = 0, z = 0; z < lengthSegments; z++, vi++)
        {
            for (int x = 0; x < widthSegments; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + widthSegments + 1;
                triangles[ti + 5] = vi + widthSegments + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        //mesh.triangles = mesh.triangles.Reverse().ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
    public void WaveMesh()
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = GetWaveHeight(transform.position + (vertices[i]));
        }
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
    }
    public Vector3 GetWaveNormal(Vector3 pos, float tresh)
    {
        Vector3 a = new Vector3(pos.x - tresh, GetWaveHeight(pos - (Vector3.right * tresh)), pos.z);
        Vector3 b = new Vector3(pos.x + tresh, GetWaveHeight(pos + (Vector3.right * tresh)), pos.z);
        Vector3 c = b - a;
        c = new Vector3(-c.y, c.x, c.z);
        a = new Vector3(pos.x, GetWaveHeight(pos - (Vector3.forward * tresh)), pos.z - tresh);
        b = new Vector3(pos.x, GetWaveHeight(pos + (Vector3.forward * tresh)), pos.z + tresh);
        Vector3 d = b - a;
        d = new Vector3(d.x, d.z, -d.y);
        return  new Vector3(c.x, (c.y + d.y) / 2, d.z);
    }
    public float GetWaveHeight(Vector3 pos)
    {
        /*float wave1 = amplitude * Mathf.Sin(pos.x / length + offset);
        float wave2 = amplitude * Mathf.Sin(pos.z / length + offset) * 0.8f;
        return (wave1 + wave2) / 2;*/
        // Calculate the horizontal distance from the wave's peak
        float dx = pos.x / waveLength;
        float dz = pos.z / waveLength;

        // Calculate the wave's height
        float height = 0f;
        for (int i = 0; i < numPhases; i++)
        {
            float angle = Mathf.PI * 2f * ((dx * ((i + 1) % 2)) + (dz * (i % 2)) + offset) + _phases[i];
            float phase = Mathf.Pow(waveSteepness / waveLength, i + 1);
            height += Mathf.Sin(angle) * waveHeight * phase;
        }

        return height;
    }
}
