using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FileSaver : MonoBehaviour
{
    public SaveData saveData; // where data is stored
    private string path; // path for saving the file

    private FileStream dataStream; // stream to the file path
    private BinaryFormatter converter = new BinaryFormatter(); // formatter for encrypting

    private void Awake()
    {
        path = Application.persistentDataPath + "/data.xd";
    }

    /// <summary>
    /// Writes the data to file.
    /// </summary>
    /// <param name="newSaveData"></param>
    public void SaveFile(SaveData newSaveData)
    {
        saveData = newSaveData;
        dataStream = new FileStream(path, FileMode.Create);
        converter.Serialize(dataStream, saveData);
        dataStream.Close();
    }

    /// <summary>
    /// Reades the data from file.
    /// </summary>
    public void readFile()
    {
        path = Application.persistentDataPath + "/data.xd";
        if (File.Exists(path))
        {
            dataStream = new FileStream(path, FileMode.Open);
            saveData = converter.Deserialize(dataStream) as SaveData;
            dataStream.Close();
        }
        else
        {
            setDefaultParameters();
            SaveFile(saveData);
        }
        GetComponent<LevelManager>().setSaveData(saveData);
    }

    /// <summary>
    /// Restarts the data parameters to default.
    /// </summary>
    private void setDefaultParameters()
    {
        saveData.volume = 0.7f;
        saveData.lastLevel = 0;
        saveData.lastLevelDifficulty = -1;
        saveData.musikEnabled = true;
        saveData.AddFirstLineOfLevels();
        saveData.ClearBoughtSkins();
    }
}
