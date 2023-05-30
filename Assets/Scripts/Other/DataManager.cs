using UnityEngine;

public class DataManager : MonoBehaviour
{
    [Header("Data")]
    public SaveData saveData; // where data is stored
    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
    }

    /// <summary>
    /// Syncs the current data with the data saved in memory.
    /// </summary>
    /// <param name="loadedSaveData"></param>
    public void RestoreSaveData(SaveData loadedSaveData)
    {
        saveData = loadedSaveData;
        GetComponent<LevelManager>().equippedSkin = saveData.equippedSkin;
        GetComponent<AudioSource>().enabled = saveData.musikEnabled;
        GetComponent<LevelManager>().SetAllAudiosToSavedValue();
    }

    /// <summary>
    /// Writes data to file.
    /// </summary>
    public void SaveDataToFile()
    {
        GetComponent<FileSaver>().SaveFile(saveData);
    }

    /// <summary>
    /// Checks if there are any new levels to sync with progress.
    /// </summary>
    /// <param name="a"></param>
    /// <returns>True if there are new levels, otherwise false.</returns>
    public bool CheckForNewLevels(int a)
    {
        if (saveData.levelsDone != null)
        {
            if (saveData.levelsDone.Count < a)
            {
                saveData.AddNewLevelsToList(a);
                SaveDataToFile();
            }
        }
        else
        {
            saveData.AddFirstLineOfLevels();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Enables or disables audio source based on saved settings.
    /// </summary>
    public void SwitchAudioSource()
    {
        GetComponent<AudioSource>().enabled = saveData.musikEnabled;
    }

    /// <summary>
    /// Saves the audio volume and music state to file.
    /// </summary>
    /// <param name="volume"></param>
    /// <param name="musikEnabled"></param>
    public void SaveDataFromSettings(float volume, bool musikEnabled)
    {
        saveData.volume = volume;
        saveData.musikEnabled = musikEnabled;
        SaveDataToFile();
    }

    /// <summary>
    /// Writes values to saveData after level.
    /// </summary>
    /// <param name="money"></param>
    public void WriteLevelDataAfterFinish(int money, int levelInList, int realLevel, int difficulty)
    {
        saveData.levelsDone[levelInList][difficulty] = true;
        saveData.crystalls += money;
        saveData.lastLevel = realLevel;
        saveData.lastLevelDifficulty = difficulty;
        SaveDataToFile();
    }

    /// <summary>
    /// Saves the new skin data.
    /// </summary>
    public void ConfirmSkinBuy(int price, int skinId)
    {
        saveData.crystalls -= price;
        saveData.boughtSkins.Add(skinId);
        saveData.equippedSkin = skinId;
        SaveDataToFile();
    }
}
