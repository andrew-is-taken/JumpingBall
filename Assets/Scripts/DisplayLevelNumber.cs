using UnityEngine;
using UnityEngine.UI;

public class DisplayLevelNumber : MonoBehaviour
{
    private LevelManager levelManager;
    public int levelNumber;

    public Image Lock;
    public Image[] Stars;
    public Color[] StarsColors;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();

        UpdateText();
        UpdateLockVisibility();

        GetComponent<Button>().onClick.AddListener(SelectLevel);
    }

    private void SelectLevel()
    {
        if(levelNumber != 0)
        {
            if (levelManager.saveData.levelsDone[levelNumber][0]
            || levelManager.saveData.levelsDone[levelNumber][1] || levelManager.saveData.levelsDone[levelNumber][2])
            {
                FindObjectOfType<Menu>().SelectLevel(levelNumber + 1);
            }
        }
        else
        {
            FindObjectOfType<Menu>().SelectLevel(levelNumber + 1);
        }
    }

    private void UpdateText()
    {
        GetComponentInChildren<Text>().text = name.Remove(0, 2);
        levelNumber = int.Parse(GetComponentInChildren<Text>().text) - 1;
    }

    private void UpdateLockVisibility()
    {
        if (levelNumber != 0)
        {
            if (levelManager.saveData.levelsDone[levelNumber - 1][0]
                || levelManager.saveData.levelsDone[levelNumber - 1][1] || levelManager.saveData.levelsDone[levelNumber - 1][2])
            {
                Lock.gameObject.SetActive(false);
                PaintStars();
            }
            else
            {
                Lock.gameObject.SetActive(true);
                foreach (Image i in Stars)
                {
                    i.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            Lock.gameObject.SetActive(false);
            PaintStars();
        }
    }

    private void PaintStars()
    {
        for(int i = 0; i < 3; i++)
        {
            if (levelManager.saveData.levelsDone[levelNumber][i])
                Stars[i].color = StarsColors[i];
        }
    }
}
