using UnityEngine;

public class IAPProcessor : MonoBehaviour
{
    private DataManager dataManager;

    private void Awake()
    {
        dataManager = GetComponent<DataManager>();
    }

    /// <summary>
    /// Writes information to save data when player purchased something.
    /// </summary>
    /// <param name="id"></param>
    public void OnPurchaseComplete(string id)
    {
        switch (id)
        {
            case "no_ads":
                dataManager.saveData.noAds = true;
                break;
            case "1000_coins":
                dataManager.saveData.crystalls += 1000;
                break;
            case "3000_coins":
                dataManager.saveData.crystalls += 3000;
                break;
            case "8000_coins":
                dataManager.saveData.crystalls += 8000;
                break;
            case "20000_coins":
                dataManager.saveData.crystalls += 20000;
                break;
        }
        dataManager.SaveDataToFile();
        dataManager.UpdateMoney();
    }
}
