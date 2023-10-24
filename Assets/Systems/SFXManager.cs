using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioSource source;
    public float volume;
    public bool active;
    public int priority;
    public float timeActivated;
    public float lifeSpan;
    public bool autoDis;
    public bool isFadingOut;
    public Sound(AudioSource aS, float v,int p,float tA,float l, bool aD)
    {
        source = aS;
        volume = v;
        priority = p;
        timeActivated = tA;
        lifeSpan = l;
        autoDis = aD;
        isFadingOut = false;
    }
}
[System.Serializable]
public class ClipsList
{
    public string soundsName;
    public int lastClipUsed;
    public AudioClip[] clips;
}
[System.Serializable]
public class SoundCategory
{
    public string categoryName;
    public ClipsList[] soundlists;
    public Dictionary<string, ClipsList> clipsLists;
}
public class SFXManager : MonoBehaviour
{
    public SoundCategory[] sCategories;
    public Dictionary<string, SoundCategory> categories;
    public List<Sound> sounds;
    public GameObject sourcePrefab;
    public int sourceInstances;
    public float waterVolumeMulti = 1;
    // Start is called before the first frame update
    void Start()
    {
        categories = new Dictionary<string, SoundCategory>();
        foreach (SoundCategory sCat in sCategories)
        {
            categories[sCat.categoryName] = sCat;
            sCat.clipsLists = new Dictionary<string, ClipsList>();
            foreach (ClipsList clipList in sCat.soundlists)
            {
                sCat.clipsLists[clipList.soundsName] = clipList;
            }
        }
        sounds = new List<Sound>();
        for (int s = 0; s < sourceInstances; s++)
        {
            AudioSource aS = Instantiate(sourcePrefab,Vector3.zero, Quaternion.identity, transform).GetComponent<AudioSource>();
            aS.gameObject.SetActive(false);
            sounds.Add(new Sound(aS,1,0,0,0,true));
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Sound snd in sounds)
        {
            ManageSound(snd);
        }
    }
    public Sound PlaySoundAtPoint(string categoryName, string sndName, Vector3 pos, float volume, int priority, bool loop = false, Sound snd = null, int sndIndex = -1)
    {
        if (snd == null)
        {
            foreach (Sound s in sounds)
            {
                if (!s.active || ((snd != null && snd.priority > s.priority) && priority >= s.priority))
                {
                    snd = s;
                }
            }
        }
        if (snd != null)
        {
            snd.priority = priority;
            snd.source.transform.parent = null;
            snd.source.transform.position = pos;
            snd.timeActivated = Time.time;
            snd.active = true;
            snd.volume = volume;
            snd.source.volume = volume;
            snd.source.gameObject.SetActive(true);

            ClipsList cL = categories[categoryName].clipsLists[sndName];
            if (sndIndex == -1) {sndIndex = (cL.lastClipUsed + 1) % cL.clips.Length;}
            cL.lastClipUsed = sndIndex;
            snd.source.clip = cL.clips[sndIndex];
            snd.lifeSpan = snd.source.clip.length;
            snd.source.loop = loop;
            snd.source.Play();
            snd.isFadingOut = false;
        }
        return snd;
    }
    public void ManageSound(Sound snd)
    {
        if (Time.time - snd.timeActivated > snd.lifeSpan)
        {
            snd.source.volume = snd.volume;
            if (snd.autoDis)
            {
                DeactivateSound(snd);
            }
        }
    }
    public void DeactivateSound(Sound snd, bool fadeOut = false, float fadeSpeed = 1)
    {
        if (!fadeOut)
        {
            snd.source.transform.parent = transform;
            snd.source.gameObject.SetActive(false);
            snd.active = false;
        }
        else if (!snd.isFadingOut)
        {
            StartCoroutine(FadeOutSound(snd, fadeSpeed));
        }
    }
    IEnumerator FadeOutSound(Sound snd, float fadeSpeed)
    {
        snd.isFadingOut = true;
        while (snd.volume > 0.02f)
        {
            snd.volume -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
        snd.volume = 0;
        snd.isFadingOut = false;
        DeactivateSound(snd);
    }
}
