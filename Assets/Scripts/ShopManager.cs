using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance { get; private set; }

    public Canvas shopCanvas;
    public RectTransform shopContent;
    public GameObject confirmPanel;
    public GameObject itemViewPrefab;
    public TMPro.TMP_Text itemName;
    public TMPro.TMP_Text itemDescription;
    public Button buyButton;

    public bool shopIsActive;

    ShopItemSO currentSelectedItem;
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

    private void Update()
    {
        if (shopIsActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DisableShopUI();
            }
        }
    }

    public void EnableShopUI()
    {
        shopIsActive = true;
        DialogManager.instance.dialogueIsActive = true;
        DialogManager.instance.OnInteractStart();
        shopCanvas.enabled = true;
    }

    public void DisableShopUI()
    {
        ClearItems();
        currentShopItems.Clear();

        shopIsActive = false;
        DialogManager.instance.dialogueIsActive = false;
        DialogManager.instance.OnInteractEnd();
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
            if (!shopItems[i].hasBeenSold)
                InitItem(shopItems[i]);
        }

        //Select first item
        if (currentShopItems.Count > 0)
        {
            OnSelectItem(currentShopItems[0]);
        }
    }

    void InitItem(ShopItemSO shopItem)
    {
        if (!currentShopItems.Contains(shopItem))
        {
            currentShopItems.Add(shopItem);
        }

        ShopItemView shopItemView =  Instantiate(itemViewPrefab, shopContent).GetComponent<ShopItemView>();
        shopItemView.itemCost.text = shopItem.itemCost.ToString();
        shopItemView.itemSprite.sprite = shopItem.itemIcon;
        shopItemView.selectButton.onClick.AddListener(() => OnSelectItem(shopItem));
    }

    public void OnSelectItem(ShopItemSO shopItem)
    {
        itemName.text = shopItem.itemName;
        itemDescription.text = shopItem.itemDescription;
        currentSelectedItem = shopItem;
        buyButton.interactable = (GameManager.instance.currency >= shopItem.itemCost);
    }

    public void OnBuyItem(ShopItemSO shopItem)
    {
        if (GameManager.instance.currency >= shopItem.itemCost && !shopItem.hasBeenSold)
        {
            GameManager.instance.currency -= (int)shopItem.itemCost;
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
    }

    public void OnClickYes()
    {
        if (GameManager.instance.currency >= currentSelectedItem.itemCost && !currentSelectedItem.hasBeenSold)
        {
            GameManager.instance.currency -= (int)currentSelectedItem.itemCost;
            currentSelectedItem.OnBuyItem();
            currentSelectedItem.hasBeenSold = true;

            currentShopItems.Remove(currentSelectedItem);
            currentShopItems.TrimExcess();

            //Refrech item list
            ClearItems();
            AddItems(currentShopItems);
        }

        confirmPanel.SetActive(false);
    }

    public void OnClickNo()
    {
        confirmPanel.SetActive(false);
    }
}
