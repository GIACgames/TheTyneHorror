using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public Rigidbody rb;
    public Player player;
    public Animator boatAnim;
    public Transform lanternHolder;
    public Transform boatCrossDrop;
    public LanternInteractable lanternInter;
    public Transform playerHolder;
    public bool playerInBoat = true;
    public bool isHoldingOars;
    public float maxRowVeloc;
    public float curRowVeloc;
    public float rowAccel;
    public float maxTurnVeloc;
    public float curTurnVeloc;
    public float turnAccel;
    float surfaceY;
    Vector3 surfaceNormal;
    Vector2 rowSpeed;
    public float rowPushAnimVal = 1;
    Vector3 lastLanternPos;
    public bool lockedRot;
    // Start is called before the first frame update
    void Start()
    {
        lastLanternPos = lanternHolder.position;
    }
    
    void FixedUpdate()
    {
        surfaceY = WaterManager.main.GetWaveHeight(transform.position);
        surfaceNormal = WaterManager.main.GetWaveNormal(transform.position, 0.8f);
        transform.rotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;

        Vector3 flatForw = transform.forward; flatForw.y = 0;
        playerHolder.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(flatForw, Vector3.up), 0.6f);
        //transform.up = surfaceNormal;
        boatAnim.SetFloat("RowX", rowSpeed.x);
        boatAnim.SetFloat("RowY", rowSpeed.y);
        ManageLantern();
    }
    void ManageLantern()
    {
        Vector3 disp = lanternHolder.position - lastLanternPos;
        if (lanternInter.oilCanRefiller == null)
        {
        lanternHolder.up = Vector3.Lerp(lanternHolder.up, Vector3.Lerp(Vector3.up, disp,0.9f), 0.4f);
        }
        else {lanternHolder.up = Vector3.Lerp(lanternHolder.up, transform.up, 0.1f);}
        lastLanternPos = lanternHolder.position;
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = Vector2.zero;
        if (isHoldingOars)
        {
            if (playerInBoat) {moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));}
        }
        Vector2 newRowSpeed = Vector2.zero;
        newRowSpeed.y += moveInput.y;
        newRowSpeed.x += moveInput.x;
        //if (Input.GetAxis("Vertical") != 0)
        {   
            float curSpeed = rb.velocity.magnitude;
            curRowVeloc = curSpeed;
            float accel = rowAccel;
            if (curSpeed < maxRowVeloc)
            {
            }
            rb.velocity = Vector3.MoveTowards(rb.velocity,transform.forward * moveInput.y * (newRowSpeed.y > 0 ? maxRowVeloc: maxRowVeloc * 0.75f),accel * Time.deltaTime * (newRowSpeed.y != 0 ? rowPushAnimVal: 0.1f));
            //rb.velocity += rowPushAnimVal * transform.forward * 0.02f;
        }

        curTurnVeloc += moveInput.x * turnAccel * Time.deltaTime  * (1f + (rowPushAnimVal / 2));
        if (moveInput.x == 0) {curTurnVeloc = Mathf.Lerp(curTurnVeloc, 0, Time.deltaTime * 2);}
        float absTurnVeloc = Mathf.Abs(curTurnVeloc);
        if (absTurnVeloc > maxTurnVeloc)
        {
            curTurnVeloc = (absTurnVeloc / curTurnVeloc) * maxTurnVeloc;
        }
        if (!lockedRot) {transform.Rotate(transform.up, curTurnVeloc * Time.deltaTime * Mathf.Abs(moveInput.x) * (1f + (rowPushAnimVal / 3)));}
        rowSpeed = Vector2.Lerp(rowSpeed, newRowSpeed, 0.01f);
    }
    public void GrabOars()
    {
        isHoldingOars = true;

    }
    public void ReleaseOars()
    {
        isHoldingOars = false;
    }
}
