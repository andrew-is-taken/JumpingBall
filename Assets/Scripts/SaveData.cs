using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int crystalls;
    public int equippedSkin;
    public List<int> boughtSkins = new List<int>{ 0 };

    public float volume;
    public bool musikEnabled;

    public List<List<bool>> levelsDone;

    public int lastLevel = 0;
    public int lastLevelDifficulty = -1;

    public void AddNewLevelsToList(int amount)
    {
        Debug.Log("Adding levels");
        for (int i = levelsDone.Count; i < amount; i++)
        {
            levelsDone.Add(new List<bool>() { false, false, false });
        }
    }

    public void AddFirstLineOfLevels()
    {
        levelsDone = new List<List<bool>>() { new List<bool>() { false, false, false } };
    }

    public void ClearBoughtSkins()
    {
        boughtSkins = new List<int> { 0 };
    }
}
