using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public Animator MenuAnim; // animator with menu ui

    private LevelManager levelManager; // manager
    private List<int> boughtSkins; // list of purchased skins

    public List<ShopItem> shopItems; // list of all items in shop
    public List<int> shopItemsPrice; // list with prices of items

    public GameObject confirmationPanel; // confirmation screen to proceed purchasing
    public TMP_Text confirmationPriceText; // price on confirmation screen
    public Button buySkinButton; // button to proceed purchasing

    public GameObject notEnoughMoneyPanel; // alert screen of lack of money
    public TMP_Text notEnoughMoneyPriceText; // how much extra money user needs to buy skin
    public TMP_Text alertText; // alert

    private int currentId; // selected skin id

    private void Awake()
    {
        if(levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();
        boughtSkins = levelManager.saveData.boughtSkins;
    }

    private void Start()
    {
        shopItems.Sort((x, y) => x.id.CompareTo(y.id));
        shopItems[levelManager.equippedSkin].ChangeItemState(true);

        for(int i = 0; i < boughtSkins.Count; i++)
        {
            shopItems[i].UnlockItem();
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
        levelManager.saveData.equippedSkin = levelManager.equippedSkin;
        levelManager.SaveDataToFile();
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
        return levelManager.equippedSkin;
    }

    /// <summary>
    /// Equips selected skin id.
    /// </summary>
    /// <param name="id"></param>
    public void EquipSelectedSkin(int id)
    {
        shopItems[levelManager.equippedSkin].ChangeItemState(false);
        levelManager.equippedSkin = id;
    }

    /// <summary>
    /// Asks user if he wants to buy the skin or reminds user if he needs more money.
    /// </summary>
    /// <param name="id"></param>
    public void ClickedOnLockedSkin(int id)
    {
        currentId = id;
        if (levelManager.saveData.crystalls > shopItemsPrice[id])
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
        alertText.text = (shopItemsPrice[currentId] - levelManager.saveData.crystalls) + " <sprite anim=0,5,8>";
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
        levelManager.saveData.crystalls -= shopItemsPrice[currentId];
        levelManager.saveData.boughtSkins.Add(currentId);
        levelManager.saveData.equippedSkin = currentId;
        EquipSelectedSkin(currentId);
        levelManager.SaveDataToFile();

        FindObjectOfType<MenuMoneyManager>().updateMoney(levelManager.saveData.crystalls);
        shopItems[currentId].UnlockItem();
        shopItems[levelManager.equippedSkin].ChangeItemState(true);
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
