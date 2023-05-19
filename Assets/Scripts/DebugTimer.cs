using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugTimer : MonoBehaviour
{
    private float timeFromStart;
    private float min;
    private float sec;

    private void Start()
    {
        gameObject.SetActive(Debug.isDebugBuild);
    }

    void Update()
    {
        UpdateTimer();
        UpdateText();
    }

    private void UpdateTimer()
    {
        timeFromStart += Time.deltaTime;
        min = Mathf.FloorToInt(timeFromStart / 60f);
        sec = Mathf.FloorToInt(timeFromStart % 60f);
    }

    private void UpdateText()
    {
        GetComponent<TMP_Text>().text = timeFromStart.ToString();
    }
}
