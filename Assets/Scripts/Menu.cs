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

    private int nextLevel;
    private int nextDifficulty;

    private void Start()
    {
        CalculateNextLevel();
        DisplayNextLevel();
    }

    public void StartLevel(int levelNumber)
    {
        SceneManager.LoadScene("Level" + levelNumber);
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

        nextLevel = levelManager.saveData.lastLevel + 1;
        nextDifficulty = levelManager.saveData.lastLevelDifficulty;

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
}
