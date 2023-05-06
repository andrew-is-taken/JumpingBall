using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    public TMP_Text levelTextMain;
    public TMP_Text difficultyText;
    public TMP_Text levelTextLevelSelection;

    public Color easyColor;
    public Color mediumColor;
    public Color hardColor;

    public GameObject LotteryPanel;
    public GameObject DifficultyPanel;

    private int nextLevel;
    private int nextDifficulty;

    private int selectedLevel;

    private void Start()
    {
        CalculateNextLevel();
        DisplayNextLevel();
    }

    public void StartLevel(int levelNumber)
    {
        SceneManager.LoadScene("Level " + levelNumber);
    }

    public void SetLevelDifficulty(int difficulty)
    {
        FindObjectOfType<LevelManager>().difficulty = difficulty;
    }

    public void ChangeLevelSelectionMenuState(bool isOn)
    {
        GetComponent<Animator>().SetBool("OpenLevelSelection", isOn);
    }

    public void StartNextLevel()
    {
        SetLevelDifficulty(nextDifficulty);
        StartLevel(nextLevel);
    }

    private void CalculateNextLevel()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();

        nextLevel = levelManager.saveData.lastLevel;
        nextDifficulty = levelManager.saveData.lastLevelDifficulty;

        if(nextLevel != 0)
        {
            if (nextDifficulty == 2)
            {
                nextLevel += 1;
                nextDifficulty = 0;
            }
            else
            {
                nextDifficulty += 1;
            }
        }
        else
        {
            if (nextDifficulty == 2)
            {
                nextLevel += 2;
                nextDifficulty = 0;
            }
            else
            {
                nextLevel += 1;
                nextDifficulty += 1;
            }
        }
    }

    private void DisplayNextLevel()
    {
        levelTextMain.text = "Level " + nextLevel.ToString();
        levelTextLevelSelection.text = nextLevel.ToString();
        switch (nextDifficulty)
        {
            case 0:
                difficultyText.text = "Easy";
                difficultyText.color = easyColor;
                break;
            case 1:
                difficultyText.text = "Medium";
                difficultyText.color =  mediumColor;
                break;
            case 2:
                difficultyText.text = "Hard";
                difficultyText.color = hardColor;
                break;
        }
    }

    public void OpenLottery()
    {
        LotteryPanel.SetActive(true);
    }

    public void CloseDifficultySelection()
    {
        GetComponent<Animator>().SetBool("OpenDifficultySelection", false);
    }

    public void SelectLevel(int level)
    {
        selectedLevel = level;
        GetComponent<Animator>().SetBool("OpenDifficultySelection", true);
    }

    public void SelectDifficuultyAndStartLevel(int diff)
    {
        FindObjectOfType<LevelManager>().StartSelectedLevel(selectedLevel, diff);
    }
}
