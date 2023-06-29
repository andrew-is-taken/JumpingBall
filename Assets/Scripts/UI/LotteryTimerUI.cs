using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LotteryTimerUI : MonoBehaviour
{
    private LotteryTimer timer;
    [SerializeField] private Button spinButton; // button that shows the ad
    [SerializeField] private GameObject blockingPan; // panel that blocks the ad from being shown
    [SerializeField] private TMP_Text timerText; // timer for the ad to be ready

    private void OnEnable()
    {
        timer = FindObjectOfType<LotteryTimer>();
        if(timer.timerTime > 0f)
            StartTimer();
    }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    public void StartTimer()
    {
        blockingPan.SetActive(true);
        spinButton.interactable = false;
        StartCoroutine(UpdateTimer());
    }

    /// <summary>
    /// Stops the timer after time ends.
    /// </summary>
    public void StopTimer()
    {
        blockingPan.SetActive(false);
        spinButton.interactable = true;
    }

    /// <summary>
    /// Updates the timer ui once per second.
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateTimer()
    {
        if (timer.counting)
            timerText.text = Mathf.Round(timer.timerTime).ToString();
        yield return new WaitForSeconds(1f);
        StartCoroutine(UpdateTimer());
    }
}
