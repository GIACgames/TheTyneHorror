using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternInteractable : Interactable
{
    public Transform oilTarget;
    public float level;
    public float capacity;
    public float drainSpeed = 0.5f;
    public float refillSpeed = 2;
    public float minimumLightIntensity = 1;
    public Light lightSource;
    public OilInteractable oilCanRefiller;
    public MeshRenderer renderer;
    public bool dangerDarkMode;
    float lastOilBegin;
    float lastLightTime;
    public bool showLight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //selectable = level < capacity - 20;
        if (oilCanRefiller == null || oilCanRefiller.level <= 0 || (Time.time - lastOilBegin > 0.35f && (!oilCanRefiller.grabbed && oilCanRefiller.level <= 0 || level > capacity - 5)))
        {
            if (oilCanRefiller != null){oilCanRefiller.player.itemEventTarget = null;
            oilCanRefiller = null;}
            if (GameManager.gM.progMan.mainQLStage > 0 && GameManager.gM.player.inBoat) {level = Mathf.Clamp(level - (drainSpeed * Time.deltaTime), 0, capacity);}
        }
        else
        {
            level = Mathf.Clamp(level + (refillSpeed * Time.deltaTime), 0, capacity);
            if (level < capacity) {oilCanRefiller.level -= (refillSpeed * Time.deltaTime);}
        }
        
        renderer.sharedMaterial.SetFloat("_Intensity", 0f + (1 * (level / capacity)));
        if (showLight)
        {
            lightSource.intensity = Mathf.Lerp(lightSource.intensity,minimumLightIntensity + (1 * (level / capacity)), 1 * Time.deltaTime);
        }
        else
        {
            lightSource.intensity = Mathf.Lerp(lightSource.intensity,0, 1 * Time.deltaTime);
        }
        if (level > 5)
        {
            lastLightTime = Time.time;
            dangerDarkMode = false;
        }
        else
        {
            if (Time.time - lastLightTime > 5)
            {
                dangerDarkMode = true;
            }
        }
    }
    public override void BeginUseItem(Interactable item)
    {
        if (item.interId == 2)
        {
            lastOilBegin = Time.time;
            oilCanRefiller = item.GetComponent<OilInteractable>();
            item.player.itemEventTarget = oilTarget;
        }
    }
    public override void StopUseItem(Interactable item)
    {
        if (item.interId == 2)
        {
            if (oilCanRefiller != null) {oilCanRefiller.player.itemEventTarget = null;
            oilCanRefiller = null;}
            //print("used OIL " + Time.time);
        }
    }
}
