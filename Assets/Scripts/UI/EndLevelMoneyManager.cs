using UnityEngine;
using TMPro;

public class EndLevelMoneyManager : MonoBehaviour
{
    private int result; // total result

    [SerializeField] private int lerpFrom;
    [SerializeField] private string currentResult; // current result for lerp
    [SerializeField] private string oldResult; // old result for lerp

    private TMP_Text resultMoney; // ui text after level end
    private float t; // time for lerp

    private void Start()
    {
        resultMoney = GetComponent<TMP_Text>();
    }

    private void FixedUpdate()
    {
        LerpMoney();
    }

    /// <summary>
    /// Lerps money from 0 to the result of the level.
    /// </summary>
    private void LerpMoney()
    {
        t += Time.deltaTime;
        if (t <= 1.1f)
        {
            currentResult = ((int)Mathf.Lerp(lerpFrom, result, t)).ToString();
            if (oldResult == lerpFrom.ToString() || oldResult == null)
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
    }

    /// <summary>
    /// Resets the result values and starts the lerp of money.
    /// </summary>
    /// <param name="res"></param>
    public void gotNewResult(int res, int lerpStart)
    {
        t = 0f;
        result = res;
        lerpFrom = lerpStart;
        oldResult = lerpFrom.ToString();
        currentResult = oldResult;
        GetComponent<TMP_Text>().text = "0 <sprite anim=0,5,8>";
    }
}
