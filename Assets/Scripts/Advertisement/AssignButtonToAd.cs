using UnityEngine;
using UnityEngine.UI;

public class AssignButtonToAd : MonoBehaviour
{
    public string AdName; // placement name

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ShowAd);
    }

    /// <summary>
    /// Starts the ad based on placement name.
    /// </summary>
    private void ShowAd()
    {
        FindObjectOfType<RewardedVideo>().ShowAd(AdName);
    }
}
