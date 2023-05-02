using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotterySpin : MonoBehaviour
{
    private bool spinning;
    private float spinPositionStart;
    private float spinPositionEnd;

    private float result;
    private float t;

    public float percentForRareItem; // chance to get rarest drop from lottery
    public int amountOfCells; // amount of items in spin session

    public RectTransform roulette; // holder of content of lottery
    public AnimationCurve timeCurve; // curve of roulette interpolation

    public Image Slot; // prefab of new item in roulette
    public List<Sprite> Items; // all items from lottery

    private void Update()
    {
        if (spinning)
        {
            t += Time.deltaTime * 0.2f;
            roulette.anchoredPosition = new Vector2(roulette.anchoredPosition.x, Mathf.Lerp(spinPositionStart, spinPositionEnd, timeCurve.Evaluate(t)));
            if (t > 1f)
                spinning = false;
        }
    }

    public void RestartRoulette()
    {
        t = 0f;
        result = Random.Range(amountOfCells - 5f, amountOfCells);
        roulette.anchoredPosition = new Vector2(roulette.anchoredPosition.x, -75f);
    }

    public void StartSpin()
    {
        RestartRoulette();
        GenerateItems();
        spinPositionStart = roulette.anchoredPosition.y;
        spinPositionEnd = result * 150f + spinPositionStart;
        spinning = true;
    }

    private void GenerateItems()
    {
        // destroy previous items
        foreach (Transform child in roulette)
        {
            Destroy(child.gameObject);
        }

        // create new items
        for (int i = 0; i < result + 2; i++)
        {
            Image newSlot = Instantiate(Slot, roulette);
            int generatedNumber;
            if (i == result)
            {
                generatedNumber = Random.Range(0, 1000);
                if (generatedNumber <= (int)(percentForRareItem * 1000f))
                    generatedNumber = 0;
                else
                    generatedNumber = Random.Range(1, Items.Count);
            }
            else
            {
                generatedNumber = Random.Range(0, Items.Count);
            }
            newSlot.sprite = Items[generatedNumber];
        }
    }
}
