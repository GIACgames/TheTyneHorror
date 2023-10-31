using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public Rigidbody rb;
    public Player player;
    public Animator boatAnim;
    public AudioSource boatMoveAS;
    public AudioSource waterIdleAS;
    public float maxMoveSpeedForMaxVol = 10;
    public Transform lanternHolder;
    public Transform boatCrossDrop;
    public LanternInteractable lanternInter;
    public Transform playerHolder;
    public Transform[] npcHolders;
    public OilInteractable[] oilCanisters;
    public GameObject oilCanPrefab;
    public Transform[] oilCanHolders;
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
    public float progSpeedMulti;
    public bool animRowStart;
    bool wasAnimRowStart;
    bool isFastRowing;
    public bool isDummyBoat;
    // Start is called before the first frame update
    void Start()
    {
        oilCanisters = new OilInteractable[oilCanHolders.Length];
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
        boatAnim.SetFloat("animSpeed", progSpeedMulti * (1.5f * (isFastRowing ? 1.3f: 1)));
        if (!isDummyBoat) {boatMoveAS.volume = Mathf.Clamp(rb.velocity.magnitude / maxMoveSpeedForMaxVol, 0, 1) * 0.5f * GameManager.gM.sfxManager.waterVolumeMulti;
        waterIdleAS.volume = GameManager.gM.sfxManager.waterVolumeMulti * 1f;}
        ManageLantern();
    }
    void NPCEnter(NPC npc)
    {
        Transform freeSpace = null;
        int i = 0;
        while (freeSpace == null && i < npcHolders.Length)
        {
            if (npcHolders[i].childCount == 0)
            {
                freeSpace = npcHolders[i];
            }
        }
        npc.transform.parent = freeSpace;
        npc.transform.localPosition = Vector3.zero;
        npc.transform.localRotation = Quaternion.identity;
        
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
        isFastRowing = false;
        Vector2 moveInput = Vector2.zero;
        if (isHoldingOars)
        {
            if (playerInBoat) {moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (Input.GetKey(KeyCode.LeftShift) && GameManager.gM.progMan.mainQLStage > 0) {isFastRowing = true;}}
        }
        Vector2 newRowSpeed = Vector2.zero;
        newRowSpeed.y += moveInput.y;
        newRowSpeed.x += moveInput.x;
        //if (Input.GetAxis("Vertical") != 0)
        {   
            float curSpeed = rb.velocity.magnitude;
            curRowVeloc = curSpeed;
            float accel = rowAccel * progSpeedMulti  * (isFastRowing ? 1.3f: 1);
            if (curSpeed < maxRowVeloc)
            {
            }
            rb.velocity = Vector3.MoveTowards(rb.velocity,transform.forward  * (isFastRowing ? 1.3f: 1) * moveInput.y * (newRowSpeed.y > 0 ? maxRowVeloc: maxRowVeloc * 0.75f),accel * Time.deltaTime * (newRowSpeed.y != 0 ? rowPushAnimVal: 0.1f));
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

        if (animRowStart != wasAnimRowStart && moveInput != Vector2.zero)
        {
            if (animRowStart)
            {
                AudioSource rowSound = GameManager.gM.sfxManager.PlaySoundAtPoint("Water", "Row", transform.position, GameManager.gM.sfxManager.waterVolumeMulti, 10, false).source;
                rowSound.pitch = (isFastRowing ? 0.9f : 1);
                rowSound.volume = (isFastRowing ? 4f : 1);
            }
            wasAnimRowStart = animRowStart;
        }
    }
    public void GrabOars()
    {
        isHoldingOars = true;

    }
    public void ReleaseOars()
    {
        isHoldingOars = false;
    }
    public void AddOilCan(OilInteractable oI)
    {
        bool freeSpace = false;
        int bestSpace = -1;
        oI.addedToBoat = true;
        for(int i = 0; i < oilCanisters.Length; i++)
        {
            if (!freeSpace)
            {
                if (oilCanisters[i] == null) {freeSpace = true; bestSpace = i;}
                else if (bestSpace == -1) {if (oI.level > oilCanisters[i].level){bestSpace = i;}}
                else {if (oilCanisters[bestSpace].level > oilCanisters[i].level){bestSpace = i;}}
            }
        }
        if (bestSpace != -1)
        {
            if (!freeSpace)
            {
                Destroy(oilCanisters[bestSpace]);
            }
            oilCanisters[bestSpace] = oI;
            oilCanisters[bestSpace].transform.parent = oilCanHolders[bestSpace];
            oilCanisters[bestSpace].dropTrans = oilCanHolders[bestSpace];
            oilCanisters[bestSpace].transform.localPosition = Vector3.zero;
            oilCanisters[bestSpace].transform.localRotation = Quaternion.identity;

        }
        else
        {
            Destroy(oI.gameObject);
        }

    }
    public void TakeOilCan(OilInteractable oI)
    {
        int oilIndex = -1;
        for(int i = 0; i < oilCanisters.Length; i++)
        {
            if (oilCanisters[i] == oI){oilIndex = i;}
        }
        if (oilIndex != -1)
        {
            oilCanisters[oilIndex] = null;
        }
    }
    public int GetOilCanCount()
    {
        int count = 0;
        for(int i = 0; i < oilCanisters.Length; i++)
        {
            if (oilCanisters[i] != null){count += 1;}
        }
        return count;
    }
    public void SetOilCanCount(int oIC)
    {
        for(int i = 0; i < oilCanisters.Length; i++)
        {
            OilInteractable oI = oilCanisters[i];
            if (oI != null) {TakeOilCan(oI);
            Destroy(oI.gameObject);}
        }
        for (int i =0; i < oIC; i++)
        {
            AddOilCan(Instantiate(oilCanPrefab).GetComponent<OilInteractable>());
        }
    }
}
