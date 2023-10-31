using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public bool testSaveTrigger;
    public bool testLoadTrigger;
    public bool testReadTrigger;
    public bool testWriteTrigger;
    public bool testResetTrigger;
    public Vector3 boatPos;
    public Vector3 playerPos;
    public Vector3 playerRot;
    public Vector3 boatRot;
    public int mainQLStage;
    public float lanternLevel;
    public int oilCanCount;
    public bool inBoat;
    public float lowestX;
    public bool mainSaveMan;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.gM != null) {if (mainSaveMan) {Destroy(GameManager.gM.saveManager.gameObject);
        GameManager.gM.saveManager = this; LoadProgress();}}
        else {DontDestroyOnLoad(this.gameObject); mainSaveMan = true; ReadSaveFile("Save0");}
    }
    void OnLevelWasLoaded()
    {
        if (GameManager.gM != null) {if (mainSaveMan && GameManager.gM.saveManager != null) {Destroy(GameManager.gM.saveManager.gameObject);
        GameManager.gM.saveManager = this; LoadProgress(); print("LOADED PROGRESS");}}
        else {if (mainSaveMan) {Destroy(this.gameObject);} else {DontDestroyOnLoad(this.gameObject); mainSaveMan = true; ReadSaveFile("Save0");}}
    }

    // Update is called once per frame
    void Update()
    {
        if (testSaveTrigger)
        {
            testSaveTrigger = false;
            SaveProgress();
        }
        if (testLoadTrigger)
        {
            testLoadTrigger = false;
            LoadProgress();
        }
        if (testReadTrigger)
        {
            testReadTrigger = false;
            ReadSaveFile("Save0");
        }
        if (testWriteTrigger)
        {
            testWriteTrigger = false;
            WriteSaveFile("Save0");
        }
        if (testResetTrigger)
        {
            testResetTrigger = false;
            ResetProgress("Save0");
        }
        
    }
    public bool CheckSaveExists()
    {
        string path = Application.persistentDataPath + "/" + "Save0" + ".txt";
        if (File.Exists(path)) {print("Found " + path);}
        return (File.Exists(path));
    }
    public void SaveProgress()
    {
        GameManager.gM.hintManager.ShowSaveText();

        playerPos = GameManager.gM.player.transform.position;
        boatPos = GameManager.gM.player.boat.transform.position;
        playerRot = GameManager.gM.player.transform.rotation.eulerAngles;
        boatRot = GameManager.gM.player.boat.transform.rotation.eulerAngles;
        inBoat = GameManager.gM.player.inBoat;
        lanternLevel = GameManager.gM.player.boat.lanternInter.level;
        oilCanCount =GameManager.gM.player.boat.GetOilCanCount();
        mainQLStage = GameManager.gM.progMan.mainQLStage;
        lowestX = GameManager.gM.progMan.lowestX;
        WriteSaveFile("Save0");
    }
    public void LoadProgress()
    {
        GameManager.gM.player.inBoat = inBoat;
        if (!inBoat) {GameManager.gM.player.transform.position = playerPos;}
        GameManager.gM.player.transform.rotation = Quaternion.Euler(playerRot);
        GameManager.gM.player.boat.transform.position = boatPos;
        GameManager.gM.player.boat.transform.rotation = Quaternion.Euler(boatRot);
        GameManager.gM.player.boat.lanternInter.level = lanternLevel;
        GameManager.gM.player.boat.SetOilCanCount(oilCanCount);

        GameManager.gM.progMan.mainQLStage = mainQLStage;
        GameManager.gM.progMan.lowestX = lowestX;
    }
    public Vector3 StringToVector(string str)
    {
        string[] segs = str.Split(",");
        Vector3 vect = new Vector3(float.Parse(segs[0]),float.Parse(segs[1]),float.Parse(segs[2]));
        return vect;
    }
    public string VectorToString(Vector3 vect)
    {
        string str = vect.x + "," + vect.y + "," + vect.z;
        return str;
    }
    public void ReadSaveFile(string fileName, string path = "")
    {
        if (path == "") {path = Application.persistentDataPath + "/" + fileName + ".txt";}
        else {path += fileName + ".txt";}
        if (File.Exists(path))
        {
            StreamReader reader = new StreamReader(path);
            string line = reader.ReadLine();
            while (line != null)
            {
                string[] varSegs = line.Split(":");
                //print(varSegs[0]);
                if (varSegs[0] == "boatPos") {boatPos = StringToVector(varSegs[1]);}
                if (varSegs[0] == "boatRot") {boatRot = StringToVector(varSegs[1]);}
                if (varSegs[0] == "playerPos") {playerPos = StringToVector(varSegs[1]);}
                if (varSegs[0] == "playerRot") {playerRot = StringToVector(varSegs[1]);}
                if (varSegs[0] == "inBoat") {inBoat = (varSegs[1] == "1");}
                if (varSegs[0] == "oilCCount") {oilCanCount = int.Parse(varSegs[1]);}
                if (varSegs[0] == "lantLevel") {lanternLevel = float.Parse(varSegs[1]);}
                if (varSegs[0] == "mQLStage") {mainQLStage = int.Parse(varSegs[1]);}
                if (varSegs[0] == "lowestX") {lowestX = float.Parse(varSegs[1]);}

                line = reader.ReadLine();
            }
            reader.Close();
        }
        else{
            print(path + " Does not exist");
        }
    }
    public void WriteSaveFile(string fileName)
    {
        List<String> lines = new List<string>();
        lines.Add("boatPos:" + VectorToString(boatPos));
        lines.Add("boatRot:" + VectorToString(boatRot));
        lines.Add("playerPos:" + VectorToString(playerPos));
        lines.Add("playerRot:" + VectorToString(playerRot));
        lines.Add("inBoat:" + (inBoat ? 1 : 0));
        lines.Add("oilCCount:" + oilCanCount);
        lines.Add("lantLevel:" + lanternLevel);
        lines.Add("mQLStage:" + mainQLStage);
        lines.Add("lowestX:" + lowestX);

        string path = Application.persistentDataPath + "/" + fileName + ".txt";
        //Write some text to the test.txt file
        if (File.Exists(path))
        {
            File.WriteAllText(path,"");
        }
        else
        {
            File.Create(path).Dispose();
        }
        StreamWriter writer = new StreamWriter(path, false);
        //writer.WriteLine("Test");
        
        //Re-import the file to update the reference in the editor
        //AssetDatabase.ImportAsset(path); 
        //TextAsset asset = Resources.Load("test");
        //Print the text from the file
        //Debug.Log(asset.text);
        writer.Write("");
        for (int l = 0; l < lines.Count; l++)
        {
            //lines[l];
            writer.WriteLine(lines[l]);
        }
        writer.Close();
    }
    public void ResetProgress(string fileName)
    {
        ReadSaveFile("DefSaveTemplate","Assets/Resources/");
        string path = Application.persistentDataPath + "/" + fileName + ".txt";
        if (File.Exists(path))
        {
            print("Deleted " + path);
            File.Delete(path);
        }
        else 
        {
            print("Could not delete " + path);
        }
    }
}
