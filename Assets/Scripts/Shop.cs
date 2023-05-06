using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public Animator MenuAnim;

    private LevelManager levelManager;
    private List<int> boughtSkins;

    public List<ShopItem> shopItems;
    public List<int> shopItemsPrice;

    public GameObject confirmationPanel;
    public TMP_Text confirmationPriceText;
    public TMP_Text confirmationText;
    public Button buySkinButton;

    public GameObject notEnoughMoneyPanel;
    public TMP_Text notEnoughMoneyPriceText;
    public TMP_Text alertText;

    private int currentId;

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

    public void OpenShop()
    {
        MenuAnim.SetBool("OpenShop", true);
    }

    public void CloseShop()
    {
        levelManager.saveData.equippedSkin = levelManager.equippedSkin;
        levelManager.SaveDataToFile();
        MenuAnim.SetBool("OpenShop", false);
    }

    public bool ClickedItemUnlocked(int id)
    {
        return boughtSkins.Contains(id);
    }

    public int GetEquippedSkin()
    {
        return levelManager.equippedSkin;
    }

    public void EquipSelectedSkin(int id)
    {
        shopItems[levelManager.equippedSkin].ChangeItemState(false);
        levelManager.equippedSkin = id;
    }

    public void ClickedOnLockedSkin(int id)
    {
        currentId = id;
        if (levelManager.saveData.crystalls > shopItemsPrice[id])
            OpenConfirmationPan();
        else
            OpenNotEnoughMoneyPan();
    }

    private void OpenConfirmationPan()
    {
        confirmationPanel.SetActive(true);
        confirmationPriceText.text = "Price: " + shopItemsPrice[currentId] + " <sprite anim=0,5,8>";
        confirmationText.text = "Are you sure?";
    }

    private void OpenNotEnoughMoneyPan()
    {
        notEnoughMoneyPanel.SetActive(true);
        notEnoughMoneyPriceText.text = "Price: " + shopItemsPrice[currentId] + " <sprite anim=0,5,8>";
        alertText.text = (shopItemsPrice[currentId] - levelManager.saveData.crystalls) + " <sprite anim=0,5,8>";
    }

    public void CloseNotEnoughMoneyPan()
    {
        notEnoughMoneyPanel.SetActive(false);
    }

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

    public void CancelSkinBuy()
    {
        confirmationPanel.SetActive(false);
    }
}
