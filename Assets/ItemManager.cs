using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    Dictionary<string, ItemSO> itemDictionary = new();
    Dictionary<string, string> itemDataDictionary = new();

    public ItemSO[] shopItems;
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

        foreach(ItemSO item in shopItems)
        {
            itemDictionary.Add(item.Id, item);

            if (itemDictionary[item.Id] is ShopItemSO si)
            {
                si.hasBeenSold = false;
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
            if (itemDataDictionary.ContainsKey(shopItem.Id) == false)
            {
                itemDataDictionary.Add(shopItem.Id, JsonUtility.ToJson(shopItem));
                Debug.Log(JsonUtility.ToJson(shopItem));
            }
        }

        SaveManager.instance.currentSaveFile.gameData.item_data.data_entries = itemDataDictionary;
    }

    public void LoadData()
    {
        Debug.Log("Load Item Data");

        itemDataDictionary = SaveManager.instance.currentSaveFile.gameData.item_data.data_entries;

        if(itemDataDictionary == null)
        {
            Debug.Log("Item data dictionary empty");
            return;
        }

        foreach (string key in itemDataDictionary.Keys)
        {
            JsonUtility.FromJsonOverwrite(itemDataDictionary[key], itemDictionary[key]);
        }
    }
}
