using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDragDropSystem : MonoBehaviour
{
    public static InventoryDragDropSystem Instance { get; private set; }

    [SerializeField] private List<InventoryGrid> inventoryTetrisList;

    public TMPro.TMP_Text itemName;
    public TMPro.TMP_Text itemDescription;

    private InventoryGrid draggingInventoryTetris;
    private InventoryItem draggingPlacedObject;
    private Vector2Int mouseDragGridPositionOffset;
    private Vector2 mouseDragAnchoredPositionOffset;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //foreach (InventoryGrid inventoryTetris in inventoryTetrisList)
        //{
        //    inventoryTetris.OnObjectPlaced += (object sender, InventoryItem placedObject) => {

        //    };
        //}
    }

    private void Update()
    {
        if (draggingPlacedObject != null)
        {
            // Calculate target position to move the dragged item
            RectTransformUtility.ScreenPointToLocalPointInRectangle(draggingInventoryTetris.GetItemContainer(), Input.mousePosition, null, out Vector2 targetPosition);
            targetPosition += new Vector2(-mouseDragAnchoredPositionOffset.x, -mouseDragAnchoredPositionOffset.y);

            // Snap position
            targetPosition /= 10;
            targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
            targetPosition *= 10;

            // Move and rotate dragged object
            draggingPlacedObject.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(draggingPlacedObject.GetComponent<RectTransform>().anchoredPosition, targetPosition, Time.deltaTime * 20f);
        }
    }

    public void StartedDragging(InventoryGrid inventoryTetris, InventoryItem placedObject)
    {
        // Started Dragging
        draggingInventoryTetris = inventoryTetris;
        draggingPlacedObject = placedObject;

        Cursor.visible = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryTetris.GetItemContainer(), Input.mousePosition, null, out Vector2 anchoredPosition);
        Vector2Int mouseGridPosition = inventoryTetris.GetGridPosition(anchoredPosition);

        // Calculate Grid Position offset from the placedObject origin to the mouseGridPosition
        mouseDragGridPositionOffset = mouseGridPosition - placedObject.GetGridPosition();

        // Calculate the anchored poisiton offset, where exactly on the image the player clicked
        mouseDragAnchoredPositionOffset = anchoredPosition - placedObject.GetComponent<RectTransform>().anchoredPosition;

    }

    public void StoppedDragging(InventoryGrid fromInventoryTetris, InventoryItem placedObject)
    {
        draggingInventoryTetris = null;
        draggingPlacedObject = null;

        Cursor.visible = true;

        // Remove item from its current inventory
        fromInventoryTetris.RemoveItemAt(placedObject.GetGridPosition());

        InventoryGrid toInventoryTetris = null;

        // Find out which InventoryTetris is under the mouse position
        foreach (InventoryGrid inventoryTetris in inventoryTetrisList)
        {
            Vector3 screenPoint = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryTetris.GetItemContainer(), screenPoint, null, out Vector2 anchoredPosition);
            Vector2Int placedObjectOrigin = inventoryTetris.GetGridPosition(anchoredPosition);
            placedObjectOrigin = placedObjectOrigin - mouseDragGridPositionOffset;

            if (inventoryTetris.IsValidGridPosition(placedObjectOrigin))
            {
                toInventoryTetris = inventoryTetris;
                break;
            }
        }

        // Check if it's on top of a InventoryTetris
        if (toInventoryTetris != null)
        {
            Vector3 screenPoint = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInventoryTetris.GetItemContainer(), screenPoint, null, out Vector2 anchoredPosition);
            Vector2Int placedObjectOrigin = toInventoryTetris.GetGridPosition(anchoredPosition);
            placedObjectOrigin = placedObjectOrigin - mouseDragGridPositionOffset;

            bool tryPlaceItem = toInventoryTetris.TryPlace(placedObject.GetPlacedObjectTypeSO(), placedObjectOrigin);

            if (tryPlaceItem)
            {
                // Item placed!
                if (toInventoryTetris == inventoryTetrisList[0])
                {
                    placedObject.GetPlacedObjectTypeSO().Equip();
                }
                else
                {
                    placedObject.GetPlacedObjectTypeSO().Unequip();
                }
            }
            else
            {
                // Drop on original position
                fromInventoryTetris.TryPlace(placedObject.GetPlacedObjectTypeSO(), placedObject.GetGridPosition());
            }
        }
        else
        {
            // Drop on original position
            fromInventoryTetris.TryPlace(placedObject.GetPlacedObjectTypeSO(), placedObject.GetGridPosition());
        }
    }

    InventoryItem currentInventoryItem;
    public void OnClickedInventoryItem(InventoryGrid fromInventoryGrid, InventoryItem inventoryItem)
    {
        Debug.Log(fromInventoryGrid);

        if (currentInventoryItem == inventoryItem)
        {
            InventoryGrid toInventoryGrid = inventoryTetrisList[0];
            foreach (InventoryGrid inventory in inventoryTetrisList)
            {
                if(inventory != fromInventoryGrid) {
                    Debug.Log(toInventoryGrid);
                    toInventoryGrid = inventory;
                }
            }

            if (toInventoryGrid.TryAutoPlaceObject(inventoryItem.GetPlacedObjectTypeSO()))
            {
                currentInventoryItem = null;
                fromInventoryGrid.RemoveItemAt(inventoryItem.GetGridPosition());
            }

            Debug.Log("Double Click");
        }

        currentInventoryItem = inventoryItem;
        itemName.text = inventoryItem.GetPlacedObjectTypeSO().GetName();
        itemDescription.text = inventoryItem.GetPlacedObjectTypeSO().GetDescription();
    }

}