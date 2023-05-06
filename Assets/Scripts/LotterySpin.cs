using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LotterySpin : MonoBehaviour
{
    private bool spinning;
    private float spinPositionStart;
    private float spinPositionEnd;
    private float t;

    public float result;
    public int droppedItemId;

    public int[] numberForItem; // chance to get rarest drop from lottery
    public int amountOfCells; // amount of items in spin session

    public RectTransform roulette; // holder of content of lottery
    public AnimationCurve timeCurve; // curve of roulette interpolation

    public Image Slot; // prefab of new item in roulette
    public List<Sprite> Items; // all items' sprites
    public List<int> ItemsCrystallAmount; // amount of crystalls for player

    public GameObject startPanel;
    public GameObject resultPanel;
    public Image resultSprite;
    public TMP_Text resultText;

    private void OnEnable()
    {
        result = Random.Range(amountOfCells - 5f, amountOfCells);
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

    public void RestartRoulette()
    {
        t = 0f;
        roulette.anchoredPosition = new Vector2(roulette.anchoredPosition.x, -75f);
        resultPanel.SetActive(false);
        startPanel.SetActive(true);
        GenerateItems();
    }

    public void StartSpin()
    {
        startPanel.SetActive(false);
        spinPositionStart = roulette.anchoredPosition.y;
        spinPositionEnd = result * 150f + spinPositionStart - 300f;
        spinning = true;
        GivePrize();
    }

    private void EndOfSpin()
    {
        spinning = false;
        resultPanel.SetActive(true);
        resultSprite.sprite = Items[droppedItemId];
        if(droppedItemId > 2)
        {
            FindObjectOfType<MenuMoneyManager>().updateMoney(FindObjectOfType<LevelManager>().saveData.crystalls);
            resultText.text = ItemsCrystallAmount[droppedItemId] + " <sprite anim=0,5,8>";
        }
        else
        {
            resultText.text = "";
        }
    }

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
            levelManager.saveData.crystalls += ItemsCrystallAmount[droppedItemId];
        }
        levelManager.SaveDataToFile();
    }

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
            Image newSlot = Instantiate(Slot, roulette);
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
                generatedNumber = Random.Range(0, Items.Count);
            }
            newSlot.sprite = Items[generatedNumber];
        }
    }

    public void CloseLottery()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
