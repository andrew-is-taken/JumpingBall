using UnityEngine;
using UnityEngine.UI;

public class DisplayLevelNumber : MonoBehaviour
{
    private LevelManager levelManager; // manager

    private int levelNumber; // this button number
    [SerializeField] private Image Lock; // loch image
    [SerializeField] private Image[] Stars; // difficulty stars
    [SerializeField] private Color[] StarsColors; // colors for difficulty stars

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();

        UpdateText();
        UpdateLockVisibility();

        GetComponent<Button>().onClick.AddListener(SelectLevel);
    }

    /// <summary>
    /// Selects the level number when user clicks on button.
    /// </summary>
    private void SelectLevel()
    {
        if(levelNumber != 1)
        {
            if (levelManager.saveData.levelsDone[levelNumber-2][0] 
                || levelManager.saveData.levelsDone[levelNumber-2][1] || levelManager.saveData.levelsDone[levelNumber-2][2])
            {
                FindObjectOfType<Menu>().SelectLevel(levelNumber);
            }
        }
        else
        {
            FindObjectOfType<Menu>().SelectLevel(levelNumber);
        }
    }

    /// <summary>
    /// Sets the button's text to level number.
    /// </summary>
    private void UpdateText()
    {
        GetComponentInChildren<Text>().text = name.Remove(0, 2);
        levelNumber = int.Parse(GetComponentInChildren<Text>().text);
    }

    /// <summary>
    /// Sets the visibility of the lock and stars for button.
    /// </summary>
    private void UpdateLockVisibility()
    {
        if (levelNumber != 1)
        {
            if (levelManager.saveData.levelsDone[levelNumber - 2][0]
                || levelManager.saveData.levelsDone[levelNumber - 2][1] || levelManager.saveData.levelsDone[levelNumber - 2][2])
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

    /// <summary>
    /// Gives color to the stars.
    /// </summary>
    private void PaintStars()
    {
        for(int i = 0; i < 3; i++)
        {
            if (levelManager.saveData.levelsDone[levelNumber-1][i])
                Stars[i].color = StarsColors[i];
        }
    }
}
