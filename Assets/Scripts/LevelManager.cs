using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public SaveData saveData;
    public int difficulty;

    public GameObject EndLevelScreen;
    public GameObject PauseScreen;
    public GameObject TapToStartPanel;
    public GameObject GameCanvas;

    public Slider AudioSlider;
    public Toggle MusicToggle;

    private Movement Player;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);

        if(scene.name == "Menu")
        {
            GameCanvas.SetActive(false);
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
    }

    private void StartOfLevel()
    {
        Time.timeScale = 0f;
        GameCanvas.SetActive(true);
        Player = FindObjectOfType<Movement>();
        EndLevelScreen.SetActive(false);
        PauseScreen.SetActive(false);
        TapToStartPanel.SetActive(true);
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
        GetComponent<FileSaver>().SaveFile(saveData);
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
        EndLevelScreen.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
