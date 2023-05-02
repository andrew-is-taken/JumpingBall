using UnityEngine;
using UnityEngine.UI;

public class AssignButtonToAd : MonoBehaviour
{
    public string AdName;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ShowAd);
    }

    private void ShowAd()
    {
        FindObjectOfType<RewardedVideo>().ShowAd(AdName);
    }
}
