using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance { get; private set; }

    public Canvas shopCanvas;
    public RectTransform shopContent;
    public GameObject shopView;
    public GameObject confirmPanel;
    public GameObject itemViewPrefab;
    public TMPro.TMP_Text itemName;
    public TMPro.TMP_Text itemDescription;

    public bool shopIsActive;

    public GameEvent OnInteractStartEvent;
    public GameEvent OnInteractEndEvent;

    public IntegerReference playerCurrency;

    public ShopItemSO currentSelectedItem;
    List<ShopItemSO> currentShopItems = new List<ShopItemSO>();

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void OnPauseClicked()
    {
        if (shopIsActive)
        {
            DisableShopUI();
        }
    }

    //public void OnSubmitClicked()
    //{
    //    if (shopIsActive)
    //    {
    //        if (currentSelectedItem != null)
    //        {
    //            confirmPanel.SetActive(true);
    //            shopView.SetActive(false);
    //            EventSystem.current.SetSelectedGameObject(confirmPanel.GetComponentInChildren<Button>().gameObject);
    //        }
    //    }
    //}

    public void EnableShopUI()
    {
        shopIsActive = true;
        OnInteractStartEvent.Raise();

        shopCanvas.enabled = true;
        ClearItems();
    }

    public void DisableShopUI()
    {
        ClearItems();
        currentShopItems.Clear();

        shopIsActive = false;

        OnInteractEndEvent.Raise();
        shopCanvas.enabled = false;
    }

    public void ClearItems()
    {
        for (int i = 0; i < shopContent.childCount; i++)
        {
            Destroy(shopContent.GetChild(i).gameObject);
        }
    }

    public void AddItems(List<ShopItemSO> shopItems)
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            ItemManager.Instance.AddItemToDictionary(shopItems[i]);

            if (!shopItems[i].hasBeenSold)
            {
                InitItem(shopItems[i]);
            }

        }

        //Select first item
        if (currentShopItems.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(shopContent.GetChild(0).gameObject);
            Debug.Log(EventSystem.current.currentSelectedGameObject);
        }
    }

    void InitItem(ShopItemSO shopItem)
    {
        if (!currentShopItems.Contains(shopItem))
        {
            currentShopItems.Add(shopItem);
        }

        ShopItemView shopItemView =  Instantiate(itemViewPrefab, shopContent).GetComponent<ShopItemView>();
        shopItemView.itemSO = shopItem;
        shopItemView.itemCost.text = shopItem.GetCost().ToString();
        shopItemView.itemSprite.sprite = shopItem.GetIcon();
        shopItemView.itemSprite.preserveAspect = true;
        shopItemView.selectButton.onClick.AddListener(() => OnSelectItem());
    }

    public void OnSelectItem()
    {
        confirmPanel.SetActive(true);
        shopView.SetActive(false);
        EventSystem.current.SetSelectedGameObject(confirmPanel.GetComponentInChildren<Button>().gameObject);
    }

    public void OnBuyItem(ShopItemSO shopItem)
    {
        if (playerCurrency.Value >= shopItem.GetCost() && !shopItem.hasBeenSold)
        {
            playerCurrency.Value -= (int)shopItem.GetCost();
            shopItem.OnBuyItem();
            shopItem.hasBeenSold = true;

            currentShopItems.Remove(shopItem);
            currentShopItems.TrimExcess();

            //Refrech item list
            ClearItems();
            AddItems(currentShopItems);
        }
    }

    public void OnClickBuy()
    {
        confirmPanel.SetActive(true);
        shopView.SetActive(false);
        EventSystem.current.SetSelectedGameObject(confirmPanel.GetComponentInChildren<Button>().gameObject);

    }

    public void OnClickYes()
    {
        if (playerCurrency.Value >= currentSelectedItem.GetCost() && !currentSelectedItem.hasBeenSold)
        {
            playerCurrency.Value -= (int)currentSelectedItem.GetCost();
            currentSelectedItem.OnBuyItem();
            currentSelectedItem.hasBeenSold = true;

            currentShopItems.Remove(currentSelectedItem);
            currentShopItems.TrimExcess();

            //Refrech item list
            ClearItems();
            AddItems(currentShopItems);
        }

        confirmPanel.SetActive(false);
        shopView.SetActive(true);
    }

    public void OnClickNo()
    {
        shopView.SetActive(true);
        confirmPanel.SetActive(false);
    }
}
