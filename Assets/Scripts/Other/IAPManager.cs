using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

[RequireComponent(typeof(IAPProcessor))]
public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    IStoreController m_StoreController;

    private IAPProcessor processor;

    private string coins_1000 = "1000_coins";
    private string coins_3000 = "3000_coins";
    private string coins_8000 = "8000_coins";
    private string coins_20000 = "20000_coins";
    private string no_ads = "no_ads";

    private void Start()
    {
        processor = GetComponent<IAPProcessor>();
        InitializePurchasing();
    }

    /// <summary>
    /// Initializes the iap system.
    /// </summary>
    private void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(coins_1000, ProductType.Consumable);
        builder.AddProduct(coins_3000, ProductType.Consumable);
        builder.AddProduct(coins_8000, ProductType.Consumable);
        builder.AddProduct(coins_20000, ProductType.Consumable);
        builder.AddProduct(no_ads, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// When user pressed button to donate.
    /// </summary>
    /// <param name="id"></param>
    public void Purchase(int id)
    {
        switch (id)
        {
            case 0:
                m_StoreController.InitiatePurchase(coins_1000);
                break;
            case 1:
                m_StoreController.InitiatePurchase(coins_3000);
                break;
            case 2:
                m_StoreController.InitiatePurchase(coins_8000);
                break;
            case 3:
                m_StoreController.InitiatePurchase(coins_20000);
                break;
            case 4:
                m_StoreController.InitiatePurchase(no_ads);
                break;
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("In-App Purchasing successfully initialized");
        m_StoreController = controller;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

        if (message != null)
        {
            errorMessage += $" More details: {message}";
        }

        Debug.Log(errorMessage);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var product = args.purchasedProduct;

        OnPurchaseComplete(product.definition.id);

        Debug.Log($"Purchase Complete - Product: {product.definition.id}");

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
            $" Purchase failure reason: {failureDescription.reason}," +
            $" Purchase failure details: {failureDescription.message}");
    }

    public void OnPurchaseComplete(string id)
    {
        processor.OnPurchaseComplete(id);
    }
}
