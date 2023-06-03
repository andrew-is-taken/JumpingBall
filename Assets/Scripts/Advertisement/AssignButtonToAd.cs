using UnityEngine;
using UnityEngine.UI;

public class AssignButtonToAd : MonoBehaviour, IAdButton
{
    public string AdName; // placement name

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ShowAd);
    }

    /// <summary>
    /// Starts the ad based on placement name.
    /// </summary>
    public void ShowAd()
    {
        FindObjectOfType<RewardedVideo>().ShowAd(this, AdName);
    }

    /// <summary>
    /// Turns off button when the ad is unavailable
    /// </summary>
    public void OnAdUnavailable()
    {
        GetComponent<Button>().interactable = false;
    }
}
