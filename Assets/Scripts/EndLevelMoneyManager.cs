using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndLevelMoneyManager : MonoBehaviour
{
    public int result;

    [HideInInspector] public string currentResult;
    [HideInInspector] public string oldResult;

    private TMP_Text resultMoney;
    private float t;

    private void Start()
    {
        resultMoney = GetComponent<TMP_Text>();
    }

    private void FixedUpdate()
    {
        t += Time.deltaTime;
        currentResult = ((int)Mathf.Lerp(0f, result, t)).ToString();
        if (oldResult == "0" || oldResult == null)
        {
            oldResult = currentResult;
            resultMoney.text = currentResult + " <sprite anim=0,5,8>";
        }
        else
        {
            resultMoney.text = resultMoney.text.Replace(oldResult + " ", currentResult + " ");
            oldResult = currentResult;
        }
    }

    public void gotNewResult(int res)
    {
        t = 0f;
        result = res;
        oldResult = "0";
        currentResult = "0";
        GetComponent<TMP_Text>().text = "0 <sprite anim=0,5,8>";
    }
}
