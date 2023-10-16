using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatEvent : UnityEvent<float> {}
public class LowFPSAnimation : MonoBehaviour
{
    public Animator[] anims;
    public int fps;
    public bool scaledTime;
    public float lastFrameT;
    public FloatEvent[] onStepEvents;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        float frameDelta = Time.time - lastFrameT;
        if (frameDelta > (1f / fps))
        {
            foreach (Animator anim in anims)
            {
                if (anim.gameObject.active) {anim.Update(frameDelta);}
            }
            foreach (FloatEvent fE in onStepEvents)
            {
                fE.Invoke(frameDelta);
            }
            lastFrameT = Time.time;
        }
    }
}
