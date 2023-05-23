using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LotterySpin : MonoBehaviour
{
    private bool spinning; // if lottery is spinning rn
    private float spinPositionStart; // start position of spin
    private float spinPositionEnd; // end position of spin
    private float t; // time for lerp

    [Header("Result")]
    private float result; // the frame with prize
    private int droppedItemId; // id of prize
    [SerializeField] private Image resultSprite; // sprite of result item
    [SerializeField] private TMP_Text resultText; // text of result amount

    [Header("Lottery settings")]
    [SerializeField] private int[] numberForItem; // chances (int numbers) to get item from lottery
    [SerializeField] private int amountOfCells; // amount of items in spin session
    [SerializeField] private AnimationCurve timeCurve; // curve of roulette interpolation

    [Header("Item settings")]
    [SerializeField] private Image slot; // prefab of new item in roulette
    [SerializeField] private List<Sprite> items; // all items' sprites
    [SerializeField] private List<int> itemsCrystallAmount; // amount of crystalls for player

    [Header("Lottery ui")]
    [SerializeField] private RectTransform roulette; // holder of content of lottery
    [SerializeField] private GameObject startPanel; // lottery's start screen
    [SerializeField] private GameObject resultPanel; // lottery's end screen

    private void OnEnable()
    {
        RestartRoulette();
    }

    private void Update()
    {
        if (spinning)
        {
            t += Time.deltaTime * 0.2f;
            roulette.anchoredPosition = new Vector2(roulette.anchoredPosition.x, Mathf.Lerp(spinPositionStart, spinPositionEnd, timeCurve.Evaluate(t)));
            if (t > 1f)
                EndOfSpin();
        }
    }

    /// <summary>
    /// Restarts the lottery for new spin.
    /// </summary>
    public void RestartRoulette()
    {
        t = 0f;
        result = Random.Range(amountOfCells - 5f, amountOfCells);
        roulette.anchoredPosition = new Vector2(roulette.anchoredPosition.x, -75f);
        resultPanel.SetActive(false);
        startPanel.SetActive(true);
        GenerateItems();
    }

    /// <summary>
    /// Starts the spin of lottery  and calculates end position.
    /// </summary>
    public void StartSpin()
    {
        startPanel.SetActive(false);
        spinPositionStart = roulette.anchoredPosition.y;
        spinPositionEnd = result * 150f + spinPositionStart - 300f;
        spinning = true;
        GivePrize();
    }

    /// <summary>
    /// Ends the spin and displays results.
    /// </summary>
    private void EndOfSpin()
    {
        spinning = false;
        resultPanel.SetActive(true);
        resultSprite.sprite = items[droppedItemId];
        if(droppedItemId > 2) // if player got money, not skin
        {
            FindObjectOfType<MenuMoneyManager>().updateMoney(FindObjectOfType<LevelManager>().saveData.crystalls);
            resultText.text = itemsCrystallAmount[droppedItemId] + " <sprite anim=0,5,8>";
        }
        else
        {
            resultText.text = "";
        }
    }

    /// <summary>
    /// Gives prize to player.
    /// </summary>
    private void GivePrize()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (droppedItemId < 3)
        {
            switch (droppedItemId)
            {
                case 0:
                    if (levelManager.saveData.boughtSkins.Contains(13))
                        levelManager.saveData.crystalls += 20000;
                    else
                        levelManager.saveData.boughtSkins.Add(13);
                    break;
                case 1:
                    if (levelManager.saveData.boughtSkins.Contains(12))
                        levelManager.saveData.crystalls += 10000;
                    else
                        levelManager.saveData.boughtSkins.Add(12);
                    break;
                case 2:
                    if (levelManager.saveData.boughtSkins.Contains(8))
                        levelManager.saveData.crystalls += 5000;
                    else
                        levelManager.saveData.boughtSkins.Add(8);
                    break;
            }
        }
        else
        {
            levelManager.saveData.crystalls += itemsCrystallAmount[droppedItemId];
        }
        levelManager.SaveDataToFile();
    }

    /// <summary>
    /// Generates items in lottery slots.
    /// </summary>
    private void GenerateItems()
    {
        int droppedItem = Mathf.FloorToInt(result);
        // destroy previous items
        foreach (Transform child in roulette)
        {
            Destroy(child.gameObject);
        }

        // create new items
        for (int i = 0; i < droppedItem + 5; i++)
        {
            Image newSlot = Instantiate(slot, roulette);
            int generatedNumber;
            if (i == droppedItem)
            {
                generatedNumber = Random.Range(1, 1000);
                for(int j = 1; j < numberForItem.Length; j++)
                {
                    if(generatedNumber > (numberForItem[j-1]) && generatedNumber <= (numberForItem[j]))
                        generatedNumber = j-1;
                }
                droppedItemId = generatedNumber;
            }
            else
            {
                generatedNumber = Random.Range(0, items.Count);
            }
            newSlot.sprite = items[generatedNumber];
        }
    }

    /// <summary>
    /// Closes the lottery screen.
    /// </summary>
    public void CloseLottery()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
