using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FileSaver : MonoBehaviour
{
    public SaveData saveData;
    public string path; 

    private FileStream dataStream;
    private BinaryFormatter converter = new BinaryFormatter();

    private void Awake()
    {
        path = Application.persistentDataPath + "/data.xd";
    }

    public void SaveFile(SaveData newSaveData)
    {
        saveData = newSaveData;
        dataStream = new FileStream(path, FileMode.Create);
        converter.Serialize(dataStream, saveData);
        dataStream.Close();
    }

    public void readFile()
    {
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
        GetComponent<LevelManager>().getSaveData(saveData);
    }

    private void setDefaultParameters()
    {
        saveData.volume = 1;
    }
}
