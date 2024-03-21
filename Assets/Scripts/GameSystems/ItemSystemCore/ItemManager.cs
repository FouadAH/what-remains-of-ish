using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    Dictionary<string, ItemSO> itemDictionary = new();
    Dictionary<string, string> itemDataDictionary = new();

    public ItemSO[] shopItems;
    public InventoryItemSO[] broochItems;
    public static ItemManager Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        shopItems = Resources.LoadAll<ItemSO>("Items");
        broochItems = Resources.LoadAll<InventoryItemSO>("Items");
        LoadDefault();
    }

    private void Update()
    {
        //foreach (InventoryItemSO item in broochItems)
        //{
        //    switch (item.broochID)
        //    {
        //        case 0:
        //            item.equipEffectDelegate= null;
        //            break;
        //        case 1:
        //            item.equipEffectDelegate = null;
        //            break;
        //        case 2:
        //            item.equipEffectDelegate = null;
        //            break;
        //        default:
        //            break;
        //    }
        //    item.ApplyEffect_OnUpdate();
        //}
    }

    void LoadDefault()
    {
        Debug.Log("Load Default Item Data");

        foreach (ItemSO item in shopItems)
        {
            if (itemDictionary.ContainsKey(item.Id) == false)
            {
                itemDictionary.Add(item.Id, item);

                if (itemDictionary[item.Id] is ShopItemSO si)
                {
                    si.hasBeenSold = false;
                }

                if (itemDictionary[item.Id] is InventoryItemSO bi)
                {
                    bi.isOwnedByPlayer = false;
                    bi.isEquipped = false;
                    bi.hasBeenPlacedOnGrid = false;
                }
            }
        }
    }

    public void AddItemToDictionary(ShopItemSO shopItem)
    {
        if(shopItem == null)
        {
            return;
        }

        if (itemDictionary.ContainsKey(shopItem.Id))
        {
            return;
        }

        itemDictionary.Add(shopItem.Id, shopItem);
    }

    public ItemSO GetItemByID(string id)
    {
        if (itemDictionary.ContainsKey(id))
        {
            return itemDictionary[id];
        }

        return null;
    }

    public void SaveData()
    {
        Debug.Log("Save Item Data");

        foreach (ItemSO shopItem in itemDictionary.Values)
        {
            if(itemDataDictionary == null)
            {
                itemDataDictionary = new Dictionary<string, string>();
            }

            if (itemDataDictionary.ContainsKey(shopItem.Id) == false)
            {
                itemDataDictionary.Add(shopItem.Id, JsonUtility.ToJson(shopItem));
                //Debug.Log(JsonUtility.ToJson(shopItem));
            }
        }

        SaveManager.instance.currentSaveFile.gameData.item_data.data_entries = itemDataDictionary;
    }

    public void LoadData()
    {
        Debug.Log("Load Item Data");

        itemDataDictionary = SaveManager.instance.currentSaveFile.gameData.item_data.data_entries;

        if(itemDataDictionary == null || itemDataDictionary.Count == 0)
        {
            Debug.Log("Item data dictionary empty");
            LoadDefault();
            return;
        }

        foreach (string key in itemDataDictionary.Keys)
        {
            JsonUtility.FromJsonOverwrite(itemDataDictionary[key], itemDictionary[key]);
        }
    }
}
