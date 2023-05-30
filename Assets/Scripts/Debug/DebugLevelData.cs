using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugLevelData : MonoBehaviour
{
    private DataManager dataManager;
    public bool unlockAllLevels;
    public bool printLevelsData;
    public bool clearLevelsData;
    public bool clearSkinsData;

#if UNITY_EDITOR
    private void Awake()
    {
        dataManager = GetComponent<DataManager>();
    }
#endif

    /// <summary>
    /// Called from level manager.
    /// </summary>
    public void SyncLevels()
    {
        if (unlockAllLevels) UnlockAllLevelsInSaveData();
        if (printLevelsData) PrintLevelsFromSaveData();
        if (clearLevelsData) ClearLevelsDoneFromSaveData();
        if (clearSkinsData) ClearBoughtSkinsFromSaveData();
    }

    /// <summary>
    /// <para>Debug. Called from SyncLevels().</para>
    /// Prints saved levels and bool if they are finished.
    /// </summary>
    private void PrintLevelsFromSaveData()
    {
        for (int i = 0; i < dataManager.saveData.levelsDone.Count; i++)
        {
            for (int j = 0; j < dataManager.saveData.levelsDone[i].Count; j++)
            {
                print("Level " + i + " with difficulty " + j + " has value " + dataManager.saveData.levelsDone[i][j]);
            }
        }
    }

    /// <summary>
    /// <para>Debug. Called from SyncronizeLevels().</para>
    /// Removes saved levels from memory and sets default values.
    /// </summary>
    private void ClearLevelsDoneFromSaveData()
    {
        dataManager.saveData.lastLevel = 1;
        dataManager.saveData.lastLevelDifficulty = -1;
        for (int i = 0; i < dataManager.saveData.levelsDone.Count; i++)
        {
            for (int j = 0; j < dataManager.saveData.levelsDone[i].Count; j++)
            {
                dataManager.saveData.levelsDone[i][j] = false;
            }
        }
    }

    /// <summary>
    /// Unlocks all levels for testing.
    /// </summary>
    private void UnlockAllLevelsInSaveData()
    {
        dataManager.saveData.lastLevel = 1;
        dataManager.saveData.lastLevelDifficulty = -1;
        for (int i = 0; i < dataManager.saveData.levelsDone.Count; i++)
        {
            for (int j = 0; j < dataManager.saveData.levelsDone[i].Count; j++)
            {
                dataManager.saveData.levelsDone[i][j] = true;
            }
        }
        dataManager.SaveDataToFile();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// <para>Debug. Called from SyncronizeLevels().</para>
    /// Removes bought skins from memory.
    /// </summary>
    private void ClearBoughtSkinsFromSaveData()
    {
        dataManager.saveData.ClearBoughtSkins();
    }
}
