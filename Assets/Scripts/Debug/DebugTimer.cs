using UnityEngine;
using TMPro;

public class DebugTimer : MonoBehaviour
{
    private float timeFromStart; // time from start to current frame

    private void Start()
    {
        gameObject.SetActive(Debug.isDebugBuild);
    }

    void Update()
    {
        UpdateTimer();
        UpdateText();
    }

    /// <summary>
    /// Updates the timer every frame.
    /// </summary>
    private void UpdateTimer()
    {
        timeFromStart += Time.deltaTime;
    }

    /// <summary>
    /// Updates the text on ui.
    /// </summary>
    private void UpdateText()
    {
        GetComponent<TMP_Text>().text = timeFromStart.ToString();
    }
}
