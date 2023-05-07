using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    public TMP_Text levelTextMain; // main next level number
    public TMP_Text difficultyText; // next level difficulty
    public TMP_Text levelTextLevelSelection; // next level number in level selection button

    public Color easyColor; // color of easy difficulty
    public Color mediumColor; // color of medium difficulty
    public Color hardColor; // color of hard difficulty

    public GameObject LotteryPanel; // screen with lottery

    private int nextLevel; // next level number
    private int nextDifficulty; // next level difficulty

    private int selectedLevel; // currently selected level

    private void Start()
    {
        CalculateNextLevel();
        DisplayNextLevel();
    }

    /// <summary>
    /// Starts the selected level.
    /// </summary>
    /// <param name="levelNumber"></param>
    public void StartLevel(int levelNumber)
    {
        SceneManager.LoadScene("Level " + levelNumber);
    }

    /// <summary>
    /// Starts the next level.
    /// </summary>
    public void StartNextLevel()
    {
        SetLevelDifficulty(nextDifficulty);
        StartLevel(nextLevel);
    }

    /// <summary>
    /// Sets the difficulty for the next level.
    /// </summary>
    /// <param name="difficulty"></param>
    public void SetLevelDifficulty(int difficulty)
    {
        FindObjectOfType<LevelManager>().difficulty = difficulty;
    }

    /// <summary>
    /// Opens or closes the level selection screen.
    /// </summary>
    /// <param name="isOn"></param>
    public void ChangeLevelSelectionMenuState(bool isOn)
    {
        GetComponent<Animator>().SetBool("OpenLevelSelection", isOn);
    }

    /// <summary>
    /// Calculates the next level parameters.
    /// </summary>
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

    /// <summary>
    /// Updates the next level texts in main menu.
    /// </summary>
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

    /// <summary>
    /// Opens lottery.
    /// </summary>
    public void OpenLottery()
    {
        LotteryPanel.SetActive(true);
    }

    /// <summary>
    /// Selects the level and opens difficulty selection screen.
    /// </summary>
    /// <param name="level"></param>
    public void SelectLevel(int level)
    {
        selectedLevel = level;
        GetComponent<Animator>().SetBool("OpenDifficultySelection", true);
    }

    /// <summary>
    /// Closes difficulty selection screen.
    /// </summary>
    public void CloseDifficultySelection()
    {
        GetComponent<Animator>().SetBool("OpenDifficultySelection", false);
    }

    /// <summary>
    /// Starts the level when user selected difficulty for that level.
    /// </summary>
    /// <param name="diff"></param>
    public void SelectDifficuultyAndStartLevel(int diff)
    {
        FindObjectOfType<LevelManager>().StartSelectedLevel(selectedLevel, diff);
    }
}
