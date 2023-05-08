using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int crystalls; // player's money
    public int equippedSkin; // equipped skin id

    public float volume; // volume of game sounds
    public bool musikEnabled; // if the music is enabled

    public int lastLevel = 1; // last finished level number
    public int lastLevelDifficulty = -1; // last finished level difficulty

    public List<int> boughtSkins = new List<int>{ 0 }; // list with ids of bought skins
    public List<List<bool>> levelsDone; // list with finished levels

    /// <summary>
    /// Adds new levels to the list if they were added in update.
    /// </summary>
    /// <param name="amount"></param>
    public void AddNewLevelsToList(int amount)
    {
        Debug.Log("Adding levels");
        for (int i = levelsDone.Count; i < amount; i++)
        {
            levelsDone.Add(new List<bool>() { false, false, false });
        }
    }

    /// <summary>
    /// Writes the first line of list.
    /// </summary>
    public void AddFirstLineOfLevels()
    {
        levelsDone = new List<List<bool>>() { new List<bool>() { false, false, false } };
    }

    /// <summary>
    /// Removes purchased skins from player's data.
    /// </summary>
    public void ClearBoughtSkins()
    {
        boughtSkins = new List<int> { 0 };
    }
}
