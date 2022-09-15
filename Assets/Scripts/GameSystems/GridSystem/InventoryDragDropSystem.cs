using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    PlayerInputMaster inputActions;
    Player_Input player_Input;

    Vector2 navigationVector;

    Vector2Int objectGridPosition;
    InventoryGrid currentObjectGrid;
    Vector2Int minGrid = new(0, 0);
    Vector2Int maxGrid;


    public bool isDragging;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inputActions = GameManager.instance.player.GetComponent<Player_Input>().inputActions;
        player_Input = GameManager.instance.player.GetComponent<Player_Input>();
        inputActions.UI.Navigate.started += Navigate_started;
    }

    public GameObject GetFirstItem()
    {
        Grid<InventoryGrid.GridObject> grid = inventoryTetrisList[1].GetGrid();

        for (int y = grid.GetHeight(); y >= 0; y--)
        {
            for (int x = grid.GetWidth(); x >= 0; x--)
            {
                if (grid.GetGridObject(x, y) != null)
                {
                    if (!grid.GetGridObject(x, y).CanBuild())
                    {
                        return grid.GetGridObject(x, y).inventoryItem.gameObject;
                    }
                }
            }
        }
        
        return null;
    }

    private void OnDisable()
    {
        if (isDragging)
        {
            if (player_Input.controllerConnected)
            {
                StoppedDragging_Controller(currentObjectGrid, currentInventoryItem);
            }
            else
            {
                StoppedDragging(currentObjectGrid, currentInventoryItem);
            }
        }

        currentInventoryItem = null;
    }

    private void Update()
    {
        if (draggingPlacedObject != null)
        {
            Vector2 targetPosition;

            if (player_Input.controllerConnected)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(draggingInventoryTetris.GetItemContainer(), desiredWorldPosition, null, out targetPosition);
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(draggingInventoryTetris.GetItemContainer(), Input.mousePosition, null, out targetPosition);
            }

            targetPosition += new Vector2(-mouseDragAnchoredPositionOffset.x, -mouseDragAnchoredPositionOffset.y);

            // Snap position
            targetPosition /= 10;
            targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
            targetPosition *= 10;

            // Move and rotate dragged object
            draggingPlacedObject.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(draggingPlacedObject.GetComponent<RectTransform>().anchoredPosition,
                targetPosition, Time.fixedDeltaTime * 20f);
        }
    }

    private void Navigate_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (draggingPlacedObject != null)
        {
            navigationVector = inputActions.UI.Navigate.ReadValue<Vector2>().normalized;
            bool canPlace = true;

            if (navigationVector.x > 0 && navigationVector.y == 0)
            {
                for (int i = 0; i < draggingPlacedObject.inventoryItemSO.height; i++)
                {
                    if (!draggingInventoryTetris.GetGrid().IsValidGridPosition(new Vector2Int(objectGridPosition.x + draggingPlacedObject.inventoryItemSO.width, objectGridPosition.y + i)))
                    {
                        canPlace = false;
                    }
                }

                if (canPlace)
                {
                    objectGridPosition.x++;
                }
            }
            else if (navigationVector.x == 0 && navigationVector.y > 0)
            {
                for (int i = 0; i < draggingPlacedObject.inventoryItemSO.width; i++)
                {
                    if (!draggingInventoryTetris.GetGrid().IsValidGridPosition(new Vector2Int(objectGridPosition.x + i, objectGridPosition.y + draggingPlacedObject.inventoryItemSO.height)))
                    {
                        canPlace = false;
                    }
                }
                if (canPlace)
                {
                    objectGridPosition.y++;
                }
            }
            else if (navigationVector.x == 0 && navigationVector.y < 0)
            {
                for (int i = 0; i < draggingPlacedObject.inventoryItemSO.width; i++)
                {
                    if (!draggingInventoryTetris.GetGrid().IsValidGridPosition(new Vector2Int(objectGridPosition.x + i, objectGridPosition.y - 1)))
                    {
                        canPlace = false;
                    }
                }
                if (canPlace)
                {
                    objectGridPosition.y--;
                }
            }
            else if (navigationVector.x < 0 && navigationVector.y == 0)
            {
                for (int i = 0; i < draggingPlacedObject.inventoryItemSO.height; i++)
                {
                    if (!draggingInventoryTetris.GetGrid().IsValidGridPosition(new Vector2Int(objectGridPosition.x - 1, objectGridPosition.y + i)))
                    {
                        canPlace = false;
                    }
                }
                if (canPlace)
                {
                    objectGridPosition.x--;
                }
            }

            maxGrid = new Vector2Int(draggingInventoryTetris.GetGrid().GetWidth(), draggingInventoryTetris.GetGrid().GetHeight());
            objectGridPosition.Clamp(minGrid, maxGrid);

            desiredWorldPosition = draggingInventoryTetris.GetGrid().GetWorldPosition(objectGridPosition.x, objectGridPosition.y);
        }

    }

    public void StartedDragging(InventoryGrid inventoryTetris, InventoryItem placedObject)
    {
        isDragging = true;

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

    Vector3 desiredWorldPosition;
    public void StartedDraggingController(InventoryGrid inventoryTetris, InventoryItem placedObject)
    {
        isDragging = true;

        // Started Dragging
        draggingInventoryTetris = inventoryTetris;
        draggingPlacedObject = placedObject;

        Cursor.visible = false;

        Vector3 itemWorldPosition = inventoryTetris.GetGrid().GetWorldPosition(placedObject.GetGridPosition().x, placedObject.GetGridPosition().y);
        desiredWorldPosition = itemWorldPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryTetris.GetItemContainer(), itemWorldPosition, null, out Vector2 anchoredPosition);
        Vector2Int mouseGridPosition = inventoryTetris.GetGridPosition(anchoredPosition);

        // Calculate Grid Position offset from the placedObject origin to the mouseGridPosition
        mouseDragGridPositionOffset = mouseGridPosition - placedObject.GetGridPosition();

        // Calculate the anchored poisiton offset, where exactly on the image the player clicked
        mouseDragAnchoredPositionOffset = anchoredPosition - placedObject.GetComponent<RectTransform>().anchoredPosition;
        objectGridPosition = draggingPlacedObject.GetGridPosition();
        currentObjectGrid = inventoryTetris;
    }

    public void StoppedDragging(InventoryGrid fromInventoryTetris, InventoryItem placedObject)
    {
        isDragging = false;

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

    public void StoppedDragging_Controller(InventoryGrid fromInventoryTetris, InventoryItem placedObject)
    {
        isDragging = false;

        draggingInventoryTetris = null;
        draggingPlacedObject = null;

        Cursor.visible = true;

        // Remove item from its current inventory
        fromInventoryTetris.RemoveItemAt(placedObject.GetGridPosition());

        InventoryGrid toInventoryTetris = null;

        // Find out which InventoryTetris is under the mouse position
        foreach (InventoryGrid inventoryTetris in inventoryTetrisList)
        {
            Vector3 screenPoint = desiredWorldPosition;
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
            Vector3 screenPoint = desiredWorldPosition;
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
        if (currentInventoryItem == inventoryItem)
        {
            ToggleEquip(fromInventoryGrid, inventoryItem);
        }

        currentInventoryItem = inventoryItem;
        itemName.text = inventoryItem.GetPlacedObjectTypeSO().GetName();
        itemDescription.text = inventoryItem.GetPlacedObjectTypeSO().GetDescription();
    }

    public void ToggleEquip(InventoryGrid fromInventoryGrid, InventoryItem inventoryItem)
    {
        InventoryGrid toInventoryGrid = inventoryTetrisList[0];
        foreach (InventoryGrid inventory in inventoryTetrisList)
        {
            if (inventory != fromInventoryGrid)
            {
                toInventoryGrid = inventory;
            }
        }

        if (toInventoryGrid.TryAutoPlaceObject(inventoryItem.GetPlacedObjectTypeSO()))
        {
            currentInventoryItem = null;
            fromInventoryGrid.RemoveItemAt(inventoryItem.GetGridPosition());

            // Item placed!
            if (toInventoryGrid == inventoryTetrisList[0])
            {
                inventoryItem.GetPlacedObjectTypeSO().Equip();
            }
            else
            {
                inventoryItem.GetPlacedObjectTypeSO().Unequip();
            }

            inventoryItem.GetComponent<InventoryDragDrop>().Select();
        }
    }

}