using UnityEngine;

public class LotteryTimer : MonoBehaviour
{
    public bool counting = false;
    public float timerTime;

    private void Awake()
    {
        counting = false;
    }

    private void Update()
    {
        if(counting)
        {
            timerTime -= Time.deltaTime;
            if(timerTime < 0f)
                StopTimer();
        }
    }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    public void StartTimer()
    {
        counting = true;
        timerTime = 60f;
        FindObjectOfType<LotteryTimerUI>().StartTimer();
    }

    /// <summary>
    /// Stops the timer after time ends.
    /// </summary>
    private void StopTimer()
    {
        counting = false;
        FindObjectOfType<LotteryTimerUI>().StopTimer();
    }
}
