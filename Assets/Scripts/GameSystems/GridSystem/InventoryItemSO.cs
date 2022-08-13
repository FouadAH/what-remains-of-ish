using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "newBrooch", menuName = "Items/New Brooch", order = 1)]
public class InventoryItemSO : ShopItemSO
{
    [Header("Brooch Info")]
    public Transform prefab;
    public int broochID;
    public int width;
    public int height;
    public bool canInteract = true;
    public bool isEquipped;

    public void CreateVisualGrid(Transform visualParentTransform, InventoryItemSO itemTetrisSO, float cellSize)
    {
        Transform visualTransform = Instantiate(prefab, visualParentTransform);

        // Create background
        //Transform template = visualTransform.Find("Template");
        //template.gameObject.SetActive(false);

        //for (int x = 0; x < itemTetrisSO.width; x++)
        //{
        //    for (int y = 0; y < itemTetrisSO.height; y++)
        //    {
        //        Transform backgroundSingleTransform = Instantiate(template, visualTransform);
        //        backgroundSingleTransform.gameObject.SetActive(true);
        //    }
        //}

        visualTransform.GetComponent<GridLayoutGroup>().cellSize = Vector2.one * cellSize;

        visualTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(itemTetrisSO.width, itemTetrisSO.height) * cellSize;

        visualTransform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        visualTransform.SetAsFirstSibling();
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridPositionList.Add(offset + new Vector2Int(x, y));
            }
        }
        return gridPositionList;
    }

    public override void ReceiveItem()
    {
        base.ReceiveItem();
        UI_HUD.instance.SetDebugText("Picked up a brooche!");
        UI_HUD.instance.broochInventoryGrid.TryAutoPlaceObject(this);
    }

    public override void OnBuyItem()
    {
        base.OnBuyItem();
        UI_HUD.instance.SetDebugText("Purchased a brooche!");
        UI_HUD.instance.broochInventoryGrid.TryAutoPlaceObject(this);
    }

    public void Equip()
    {
        if (!isEquipped)
        {
            isEquipped = true;
            Debug.Log("Equipped brooch: " + itemName);
            GameManager.instance.GetBool("equippedBrooch_" + broochID) = true;
        }
    }

    public void Unequip()
    {
        if (isEquipped)
        {
            isEquipped = false;
            Debug.Log("Unequipped brooch: " + itemName);
            GameManager.instance.GetBool("equippedBrooch_" + broochID) = false;
        }
    }

}
