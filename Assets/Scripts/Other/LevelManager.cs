using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Data")]
    public SaveData saveData; // where data is stored
    private MovementManager player; // player

    [HideInInspector] public int equippedSkin; // currently active skin
    private PlayerSkin[] Skins; // all player skins

    [HideInInspector] public int difficulty; // level difficulty
    private int RealLevel; // level number in game
    private int LevelInList; // level number in lists
    [SerializeField] private int[] MoneyForLevel; // money for each level

    #region Canvas panels
    [Header("Canvas panels")]
    [SerializeField] private GameObject EndLevelScreen; // screen after finish
    [SerializeField] private GameObject DeathScreen; // screen after death
    [SerializeField] private GameObject PauseScreen; // pause screen
    [SerializeField] private GameObject TapToStartPanel; // start of level screen
    [SerializeField] private GameObject GameCanvas; // canvas for levels
    #endregion

    #region Settings
    [Header("Settings")]
    [SerializeField] private Slider AudioSlider; // audio slider in settings
    [SerializeField] private Toggle MusicToggle; // music toggle in settings
    #endregion

    [Header("Other")]
    [SerializeField] private EndLevelMoneyManager EndLevelMoney; // money manager

    private Coroutine lastCoroutine; // last saved coroutine

    private void Awake()
    {
        SetQuality();
        GetComponent<FileSaver>().readFile();

        int amount = FindObjectsOfType<LevelManager>().Length; // calculate all levelManagers in scene
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

    /// <summary>
    /// Sets the framerate and vsync.
    /// </summary>
    private void SetQuality()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    /// <summary>
    /// Check if there are new levels that weren't added to level list after update.
    /// </summary>
    private void SyncronizeLevels()
    {
        //ClearBoughtSkinsFromSaveData();
        //ClearLevelsDoneFromSaveData();
        //SaveDataToFile();
        //PrintLevelsFromSaveData();

        int a = SceneManager.sceneCountInBuildSettings - 2; // scene count minus Start and Menu scenes
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

    /// <summary>
    /// Syncs the current data with the data saved in memory.
    /// </summary>
    /// <param name="loadedSaveData"></param>
    public void setSaveData(SaveData loadedSaveData)
    {
        saveData = loadedSaveData;
        equippedSkin = saveData.equippedSkin;
        GetComponent<AudioSource>().enabled = saveData.musikEnabled;
        SetAllAudiosToValue();
    }

    /// <summary>
    /// Writes data to file.
    /// </summary>
    public void SaveDataToFile()
    {
        GetComponent<FileSaver>().SaveFile(saveData);
    }

    /// <summary>
    /// Event when scene is loaded.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
#if UNITY_EDITOR
        Debug.Log("OnSceneLoaded: " + scene.name);
#endif

        if (scene.name == "Menu")
        {
            GameCanvas.SetActive(false); // disable game canvas in menu
            FindObjectOfType<MenuMoneyManager>().updateMoney(saveData.crystalls); // update money text in menu
        }
        else if (scene.name == "Start")
        {
            GoToMenu();
        }
        else
        {
            StartOfLevel();
        }

        SetAllAudiosToValue();
        GetComponent<AudioSource>().enabled = saveData.musikEnabled; // enable music

        //EndLevelMoney.gameObject.SetActive(false);
    }

    /// <summary>
    /// Starts the level when user touches the screen.
    /// </summary>
    private void StartOfLevel()
    {
        GameCanvas.SetActive(true);
        player = MovementManager.instance;

        RealLevel = int.Parse(SceneManager.GetActiveScene().name.Remove(0, 5)); // get level number
        LevelInList = RealLevel - 1;

        SyncPlayerSkin();
        PrepareMainUI();

        if (lastCoroutine != null)
        {
            StopCoroutine(lastCoroutine);
        }
    }

    /// <summary>
    /// Disables unnecessary screens and enables readysteady screen to start the level.
    /// </summary>
    public void PrepareMainUI()
    {
        Time.timeScale = 0f;

        EndLevelScreen.SetActive(false);
        DeathScreen.SetActive(false);
        PauseScreen.SetActive(false);
        TapToStartPanel.SetActive(true);
    }

    /// <summary>
    /// Sets player's skin and other effects.
    /// </summary>
    private void SyncPlayerSkin()
    {
        player.GetComponent<SpriteRenderer>().sprite = Skins[equippedSkin].sprite;
        player.GetComponentInChildren<TrailRenderer>().colorGradient = Skins[equippedSkin].color;
        player.GetComponentInChildren<TrailRenderer>().time = Skins[equippedSkin].time;

        Color dieCol = Skins[equippedSkin].dieColor;

        var mainPs = player.DeathParticles.GetComponent<ParticleSystem>().main; // main death particle system
        mainPs.startColor = dieCol;

        var c = player.DeathParticles.GetComponentsInChildren<ParticleSystem>()[1].colorOverLifetime; // splash particles
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(dieCol, 0.0f), new GradientColorKey(dieCol, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.8f), new GradientAlphaKey(0.0f, 1.0f) });
        c.color = grad;
    }

    /// <summary>
    /// When user touches the screen.
    /// </summary>
    public void TouchScreenTap()
    {
        player.TapOnScreen();
    }

    /// <summary>
    /// Stops the game.
    /// </summary>
    public void Pause()
    {
        pausePlayerAudio();

        AudioSlider.value = saveData.volume;
        MusicToggle.isOn = saveData.musikEnabled;
        PauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Continues the game.
    /// </summary>
    public void Continue()
    {
        continuePlayerAudio();
        setDataToSettingsAndSave();

        TapToStartPanel.SetActive(true);
        PauseScreen.SetActive(false);
    }

    /// <summary>
    /// Restarts the level.
    /// </summary>
    public void RestartLevel(bool fromSettings)
    {
        if (fromSettings)
            setDataToSettingsAndSave();

        Time.timeScale = 1f;
        PrepareMainUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Saves the settings to file.
    /// </summary>
    private void setDataToSettingsAndSave()
    {
        saveData.volume = AudioSlider.value;
        saveData.musikEnabled = MusicToggle.isOn;
        SaveDataToFile();
        SetAllAudiosToValue();
    }

    /// <summary>
    /// Pauses player's movement audio.
    /// </summary>
    private void pausePlayerAudio()
    {
        player.GetComponentsInChildren<AudioSource>()[1].Pause();
    }

    /// <summary>
    /// Unpauses player's movement audio.
    /// </summary>
    private void continuePlayerAudio()
    {
        player.GetComponentsInChildren<AudioSource>()[1].Play();
    }

    /// <summary>
    /// Sets all audios' volume in the scene to the desired. 
    /// </summary>
    private void SetAllAudiosToValue()
    {
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audios)
        {
            audio.volume = saveData.volume;
        }
    }

    /// <summary>
    /// Starts the level when user touches the screen.
    /// </summary>
    public void StartFromTap()
    {
        Time.timeScale = 1f;
        //player = MovementManager.instance;
        TapToStartPanel.SetActive(false);
        player.StartFromTap();
    }

    /// <summary>
    /// Enables or disables the music when user touches the toggle.
    /// </summary>
    public void MusicToggleChanged()
    {
        GetComponent<AudioSource>().enabled = MusicToggle.isOn;
    }

    /// <summary>
    /// Sets the music volume to the sliders value.
    /// </summary>
    public void AudioSliderChanged()
    {
        GetComponent<AudioSource>().volume = AudioSlider.value;
    }

    /// <summary>
    /// player's death.
    /// </summary>
    public void Death()
    {
        DeathScreen.SetActive(true);
    }

    /// <summary>
    /// End of the level.
    /// </summary>
    /// <param name="multiplier"></param>
    public void Finish(float multiplier)
    {
        int money = CalculateMoneyAfterFinish(multiplier);

        WriteLevelDataAfterFinish(money);
        SaveDataToFile();

        lastCoroutine = StartCoroutine(StopTimeOnLevelEnd(money));
    }

    /// <summary>
    /// Calculates the money after the end of the level.
    /// </summary>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    private int CalculateMoneyAfterFinish(float multiplier)
    {
        return (int)(MoneyForLevel[LevelInList] * (difficulty + 1) * 0.5f * multiplier);
    }

    /// <summary>
    /// Writes values to saveData after level.
    /// </summary>
    /// <param name="money"></param>
    private void WriteLevelDataAfterFinish(int money)
    {
        saveData.levelsDone[LevelInList][difficulty] = true;
        saveData.crystalls += money;
        saveData.lastLevel = RealLevel;
        saveData.lastLevelDifficulty = difficulty;
    }

    /// <summary>
    /// Quits to menu.
    /// </summary>
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Calculates and starts next level.
    /// </summary>
    public void StartNextLevel()
    {
        if (difficulty == 2)
        {
            RealLevel += 1;
            difficulty = 0;
        }
        else
        {
            difficulty += 1;
        }
        SceneManager.LoadScene("Level " + RealLevel.ToString());
    }

    /// <summary>
    /// Starts the level selected by user.
    /// </summary>
    /// <param name="selectedLevel"></param>
    /// <param name="diff"></param>
    public void StartSelectedLevel(int selectedLevel, int diff)
    {
        RealLevel = selectedLevel;
        difficulty = diff;
        SceneManager.LoadScene("Level " + RealLevel.ToString());
    }

    /// <summary>
    /// Respawns player on last checkpoint.
    /// </summary>
    public void RespawnPlayer()
    {
        player.RespawnOnLastCheckpoint();
    }

    /// <summary>
    /// Opens Lottery after player has watched the video ad.
    /// </summary>
    public void OpenLottery()
    {
        FindObjectOfType<Menu>().OpenLottery();
    }

    /// <summary>
    /// <para>Debug. Called from SyncronizeLevels().</para>
    /// Prints saved levels and bool if they are finished.
    /// </summary>
    private void PrintLevelsFromSaveData()
    {
        for (int i = 0; i < saveData.levelsDone.Count; i++)
        {
            for (int j = 0; j < saveData.levelsDone[i].Count; j++)
            {
                print("Level " + i + " with difficulty " + j + " has value " + saveData.levelsDone[i][j]);
            }
        }
    }

    /// <summary>
    /// <para>Debug. Called from SyncronizeLevels().</para>
    /// Removes saved levels from memory and sets default values.
    /// </summary>
    private void ClearLevelsDoneFromSaveData()
    {
        saveData.lastLevel = 1;
        saveData.lastLevelDifficulty = -1;
        for (int i = 0; i < saveData.levelsDone.Count; i++)
        {
            for (int j = 0; j < saveData.levelsDone[i].Count; j++)
            {
                saveData.levelsDone[i][j] = false;
            }
        }
    }

    /// <summary>
    /// <para>Debug. Called from SyncronizeLevels().</para>
    /// Removes bought skins from memory.
    /// </summary>
    private void ClearBoughtSkinsFromSaveData()
    {
        saveData.ClearBoughtSkins();
    }

    /// <summary>
    /// Sets the timeScale to 0 after level end.
    /// </summary>
    /// <param name="money"></param>
    /// <returns></returns>
    IEnumerator StopTimeOnLevelEnd(int money)
    {
        EndLevelScreen.SetActive(true);
        EndLevelMoney.gotNewResult(money);
        yield return new WaitForSeconds(2.5f);
        player.gameObject.SetActive(false);
    }
}