using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public SaveData saveData;
    public int difficulty;
    public int Level;
    public int[] MoneyForLevel;

    public GameObject EndLevelScreen;
    public GameObject DeathScreen;
    public GameObject PauseScreen;
    public GameObject TapToStartPanel;
    public GameObject GameCanvas;

    public Slider AudioSlider;
    public Toggle MusicToggle;

    public EndLevelMoneyManager EndLevelMoney;

    private Movement Player;
    private Coroutine lastCoroutine;

    private void Awake()
    {
        GetComponent<FileSaver>().readFile();
        int amount = FindObjectsOfType<LevelManager>().Length;

        if (amount == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SyncronizeLevels();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void SyncronizeLevels()
    {
        //ClearLevelsDoneFromSaveData();
        //SaveDataToFile();
        //PrintLevelsFromSaveData();
        int a = SceneManager.sceneCountInBuildSettings - 2; // Minus Start and Menu scenes
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
            SyncronizeLevels();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);

        if(scene.name == "Menu")
        {
            GameCanvas.SetActive(false);
            FindObjectOfType<MenuMoneyManager>().updateMoney(saveData.crystalls);
        }
        else if(scene.name == "Start")
        {
            GoToMenu();
        }
        else
        {
            StartOfLevel();
        }

        SetAllAudiosToValue();
        GetComponent<AudioSource>().enabled = saveData.musikEnabled;
        EndLevelMoney.gameObject.SetActive(false);
    }

    private void StartOfLevel()
    {
        Time.timeScale = 0f;
        GameCanvas.SetActive(true);
        Player = FindObjectOfType<Movement>();
        EndLevelScreen.SetActive(false);
        PauseScreen.SetActive(false);
        TapToStartPanel.SetActive(true);

        if (lastCoroutine != null)
        {
            StopCoroutine(lastCoroutine);
        }
    }

    public void TouchScreenTap()
    {
        Player.TapOnScreen();
    }

    public void Pause()
    {
        AudioSlider.value = saveData.volume;
        MusicToggle.isOn = saveData.musikEnabled;
        Time.timeScale = 0f;
        PauseScreen.SetActive(true);
    }

    public void Continue()
    {
        saveData.volume = AudioSlider.value;
        saveData.musikEnabled = MusicToggle.isOn;
        SaveDataToFile();
        SetAllAudiosToValue();
        TapToStartPanel.SetActive(true);
        PauseScreen.SetActive(false);
    }

    private void SetAllAudiosToValue()
    {
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audios)
        {
            audio.volume = saveData.volume;
        }
    }

    public void getSaveData(SaveData loadedSaveData)
    {
        saveData = loadedSaveData;
        GetComponent<AudioSource>().enabled = saveData.musikEnabled;
        SetAllAudiosToValue();
    }

    public void StartFromTap()
    {
        Time.timeScale = 1.0f;
        TapToStartPanel.SetActive(false);
    }

    public void MusicToggleChanged()
    {
        GetComponent<AudioSource>().enabled = MusicToggle.isOn;
    }

    public void AudioSliderChanged()
    {
        GetComponent<AudioSource>().volume = AudioSlider.value;
    }

    public void Death()
    {
        DeathScreen.SetActive(true);
    }

    public void Finish(float multiplier)
    {
        int money = CalculateMoneyAfterFinish(multiplier);

        WriteLevelDataAfterFinish(money);
        SaveDataToFile();

        lastCoroutine = StartCoroutine(StopTimeOnLevelEnd(money));
    }

    private int CalculateMoneyAfterFinish(float multiplier)
    {
        return (int)(MoneyForLevel[Level] * (difficulty + 1) * 0.5f * multiplier);
    }

    private void WriteLevelDataAfterFinish(int money)
    {
        saveData.levelsDone[Level][difficulty] = true;
        saveData.crystalls += money;
        saveData.lastLevel = Level;
        saveData.lastLevelDifficulty = difficulty;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void WatchAdToRespawn()
    {
        print("Try to respawn");
    }

    public void StartNextLevel()
    {
        Level += 1;
        SceneManager.LoadScene("Level" + Level.ToString());
    }

    private void SaveDataToFile()
    {
        GetComponent<FileSaver>().SaveFile(saveData);
    }

    private void PrintLevelsFromSaveData()
    {
        for(int i = 0; i < saveData.levelsDone.Count; i++)
        {
            for (int j = 0; j < saveData.levelsDone[i].Count; j++)
            {
                print(saveData.levelsDone[i][j]);
            }
        }
    }

    private void ClearLevelsDoneFromSaveData()
    {
        for (int i = 0; i < saveData.levelsDone.Count; i++)
        {
            for (int j = 0; j < saveData.levelsDone[i].Count; j++)
            {
                saveData.levelsDone[i][j] = false;
            }
        }
    }

    IEnumerator StopTimeOnLevelEnd(int money)
    {
        EndLevelScreen.SetActive(true);
        EndLevelMoney.gotNewResult(money);
        yield return new WaitForSeconds(2.5f);
        //Time.timeScale = 0;
        Player.gameObject.SetActive(false);
    }
}
