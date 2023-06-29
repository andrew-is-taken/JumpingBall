using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] private Animator MenuAnim; // animator with menu ui

    private PlayerManager playerManager; // level manager
    private DataManager dataManager; // data manager
    private List<int> boughtSkins; // list of purchased skins

    [Header("Items")]
    public List<ShopItem> shopItems; // list of all items in shop
    [SerializeField] private List<int> shopItemsPrice; // list with prices of items

    [Header("Confirmation UI")]
    [SerializeField] private GameObject confirmationPanel; // confirmation screen to proceed purchasing
    [SerializeField] private TMP_Text confirmationPriceText; // price on confirmation screen
    [SerializeField] private Button buySkinButton; // button to proceed purchasing

    [Header("Rejection UI")]
    [SerializeField] private GameObject notEnoughMoneyPanel; // alert screen of lack of money
    [SerializeField] private TMP_Text notEnoughMoneyPriceText; // how much extra money user needs to buy skin
    [SerializeField] private TMP_Text alertText; // alert

    private int currentId; // selected skin id

    private void Awake()
    {
        if(playerManager == null)
            playerManager = FindObjectOfType<PlayerManager>();

        dataManager = playerManager.GetComponent<DataManager>();
        boughtSkins = dataManager.saveData.boughtSkins;
    }

    private void Start()
    {
        shopItems.Sort((x, y) => x.id.CompareTo(y.id));
        shopItems[playerManager.equippedSkin].ChangeItemState(true);

        for(int i = 0; i < boughtSkins.Count; i++)
        {
            shopItems[boughtSkins[i]].UnlockItem();
        }
    }

    /// <summary>
    /// Opens shop screen.
    /// </summary>
    public void OpenShop()
    {
        MenuAnim.SetBool("OpenShop", true);
    }

    /// <summary>
    /// Closes shop screen and saves new values.
    /// </summary>
    public void CloseShop()
    {
        dataManager.saveData.equippedSkin = playerManager.equippedSkin;
        dataManager.SaveDataToFile();
        MenuAnim.SetBool("OpenShop", false);
    }
    
    /// <summary>
    /// Checks if clicked item is purchased.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True if user owns the skin, otherwise false.</returns>
    public bool ClickedItemUnlocked(int id)
    {
        return boughtSkins.Contains(id);
    }

    /// <returns>Id of equipped skin.</returns>
    public int GetEquippedSkin()
    {
        return playerManager.equippedSkin;
    }

    /// <summary>
    /// Equips selected skin id.
    /// </summary>
    /// <param name="id"></param>
    public void EquipSelectedSkin(int id)
    {
        shopItems[playerManager.equippedSkin].ChangeItemState(false);
        playerManager.equippedSkin = id;
    }

    /// <summary>
    /// Asks user if he wants to buy the skin or reminds user if he needs more money.
    /// </summary>
    /// <param name="id"></param>
    public void ClickedOnLockedSkin(int id)
    {
        currentId = id;
        if (dataManager.saveData.crystalls > shopItemsPrice[id])
            OpenConfirmationPan();
        else
            OpenNotEnoughMoneyPan();
    }

    /// <summary>
    /// Opens confirmation screen.
    /// </summary>
    private void OpenConfirmationPan()
    {
        confirmationPanel.SetActive(true);
        confirmationPriceText.text = "Price: " + shopItemsPrice[currentId] + " <sprite anim=0,5,8>";
    }

    /// <summary>
    /// Opens alert screen.
    /// </summary>
    private void OpenNotEnoughMoneyPan()
    {
        notEnoughMoneyPanel.SetActive(true);
        notEnoughMoneyPriceText.text = "Price: " + shopItemsPrice[currentId] + " <sprite anim=0,5,8>";
        alertText.text = (shopItemsPrice[currentId] - dataManager.saveData.crystalls) + " <sprite anim=0,5,8>";
    }

    /// <summary>
    /// Closes alert screen.
    /// </summary>
    public void CloseNotEnoughMoneyPan()
    {
        notEnoughMoneyPanel.SetActive(false);
    }

    /// <summary>
    /// Withdraws money from player's data and equips the skin. Closes confirmation screen.
    /// </summary>
    public void ConfirmSkinBuy()
    {
        dataManager.ConfirmSkinBuy(shopItemsPrice[currentId], currentId);
        EquipSelectedSkin(currentId);

        FindObjectOfType<MenuMoneyManager>().updateMoney(dataManager.saveData.crystalls);
        shopItems[currentId].UnlockItem();
        shopItems[playerManager.equippedSkin].ChangeItemState(true);
        confirmationPanel.SetActive(false);
    }

    /// <summary>
    /// Closes confirmation screen.
    /// </summary>
    public void CancelSkinBuy()
    {
        confirmationPanel.SetActive(false);
    }
}
