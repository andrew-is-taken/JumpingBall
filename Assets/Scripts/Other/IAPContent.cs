using UnityEngine;
using UnityEngine.UI;

public class IAPContent : MonoBehaviour
{
    private DataManager dataManager;
    [SerializeField] private Button noAdsButton;
    [SerializeField] private GameObject noAdsPurchased;

    private void Awake()
    {
        dataManager = FindObjectOfType<DataManager>();
        noAdsButton.interactable = !dataManager.saveData.noAds;
        noAdsPurchased.SetActive(dataManager.saveData.noAds);
    }

    public void Purchase(int id)
    {
        dataManager.GetComponent<IAPManager>().Purchase(id);
    }
}
