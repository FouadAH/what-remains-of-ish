using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    private InventoryItemSO inventoryItemSO;
    private Vector2Int origin;

    public static InventoryItem CreateCanvas(Transform parent, Vector2 anchoredPosition, Vector2Int origin, InventoryItemSO inventoryItemSO)
    {
        Transform placedObjectTransform = Instantiate(inventoryItemSO.prefab, parent);
        placedObjectTransform.rotation = Quaternion.identity;
        placedObjectTransform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        InventoryItem inventoryItem = placedObjectTransform.GetComponent<InventoryItem>();
        inventoryItem.inventoryItemSO = inventoryItemSO;
        inventoryItem.origin = origin;

        return inventoryItem;
    }


    public Vector2Int GetGridPosition()
    {
        return origin;
    }

    public void SetOrigin(Vector2Int origin)
    {
        this.origin = origin;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return inventoryItemSO.GetGridPositionList(origin);
    }

    public virtual void DestroySelf()
    {
        Destroy(gameObject);
    }

    public override string ToString()
    {
        return inventoryItemSO.nameString;
    }

    public InventoryItemSO GetPlacedObjectTypeSO()
    {
        return inventoryItemSO;
    }

}
