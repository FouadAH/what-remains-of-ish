using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    public static InventoryGrid Instance { get; private set; }

    private Grid<GridObject> grid;
    public int gridWidth;
    public int gridHeight;
    public RectTransform itemContainer;
    public bool isEquipScreen;

    public InventoryItemSO blockedTileSO;
    public InventoryItemSO unblockableTileSO;

    public PlayerDataSO PlayerDataSO;

    private void Awake()
    {
        Instance = this;

        float cellSize = 100f;
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

        if (isEquipScreen)
        {
            //corners
            BlockTile(0, 0);
            BlockTile(0, 3);
            BlockTile(3, 0);
            BlockTile(3, 3);

            //ulockable
            AddUnblockableTile(1, 0);
            AddUnblockableTile(2, 0);

            AddUnblockableTile(0, 1);
            AddUnblockableTile(0, 2);

            AddUnblockableTile(3, 1);
            AddUnblockableTile(3, 2);

            AddUnblockableTile(1, 3);
            AddUnblockableTile(2, 3);
        }
    }

    private void OnEnable()
    {
        Setup();
    }

    public void OnUnlockTile(int x, int y)
    {
        if (isEquipScreen)
        {
            UnblockTile(x, y);
        }
    }

    void BlockTile(int x, int y)
    {
        grid.GetGridObject(x, y).isBlocked = true;
        InventoryItem inventoryItem = InventoryItem.CreateCanvas(itemContainer, grid.GetWorldPosition(x, y), new Vector2Int(x, y), blockedTileSO);
        grid.GetGridObject(x, y).inventoryItem = inventoryItem;
    }

    void AddUnblockableTile(int x, int y)
    {
        grid.GetGridObject(x, y).isBlocked = true;
        InventoryItem inventoryItem = InventoryItem.CreateCanvas(itemContainer, grid.GetWorldPosition(x, y), new Vector2Int(x, y), unblockableTileSO);
        BlockedInventoryItem blocked = inventoryItem as BlockedInventoryItem;
        blocked.OnUnblocked += UnblockTile;
        grid.GetGridObject(x, y).inventoryItem = inventoryItem;
    }

    void UnblockTile(int x, int y)
    {
        grid.GetGridObject(x, y).isBlocked = false;
        grid.GetGridObject(x, y).inventoryItem.DestroySelf();
    }


    public void Setup()
    {
        foreach (InventoryItemSO broocheItem in ItemManager.Instance.broochItems)
        {
            if (broocheItem.isOwnedByPlayer)
            {
                if (!broocheItem.hasBeenPlacedOnGrid)
                {
                    if (broocheItem.isEquipped && isEquipScreen)
                    {
                        TryAutoPlaceObject(broocheItem);
                        broocheItem.hasBeenPlacedOnGrid = true;
                    }
                    else if (!broocheItem.isEquipped && !isEquipScreen)
                    {
                        TryAutoPlaceObject(broocheItem);
                        broocheItem.hasBeenPlacedOnGrid = true;
                    }
                }
                else
                {
                    if (broocheItem.isEquipped && isEquipScreen)
                    {
                        TryPlace(broocheItem, broocheItem.gridCoordinates);
                        broocheItem.hasBeenPlacedOnGrid = true;
                    }
                    else if (!broocheItem.isEquipped && !isEquipScreen)
                    {
                        TryPlace(broocheItem, broocheItem.gridCoordinates);
                        broocheItem.hasBeenPlacedOnGrid = true;
                    }
                }
            }
        }
    }

    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int y;
        public InventoryItem inventoryItem;
        public bool isBlocked = false;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            inventoryItem = null;
        }

        public override string ToString()
        {
            return x + ", " + y + "\n" + inventoryItem;
        }

        public void SetBlockedTile(Vector2Int origin)
        {
            isBlocked = true;
        }

        public void SetPlacedObject(InventoryItem inventoryItem)
        {
            this.inventoryItem = inventoryItem;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject()
        {
            inventoryItem = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public InventoryItem GetPlacedObject()
        {
            return inventoryItem;
        }

        public bool CanBuild()
        {
            return inventoryItem == null && !isBlocked;
        }

        public bool HasPlacedObject()
        {
            return inventoryItem != null;
        }

    }

    public Grid<GridObject> GetGrid()
    {
        return grid;
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXY(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        return grid.IsValidGridPosition(gridPosition);
    }

    public bool TryAutoPlaceObject(InventoryItemSO inventoryItemSO)
    {
        bool isPlaced = false;
        for (int y = grid.GetHeight(); y >= 0; y--)
        {
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                isPlaced = TryPlace(inventoryItemSO, new Vector2Int(x, y));

                if (isPlaced)
                {
                    return isPlaced;
                }
            }
        }
        return isPlaced;
    }

    public bool TryPlace(InventoryItemSO inventoryItemSO, Vector2Int placedObjectOrigin)
    {
        List<Vector2Int> gridPositionList = inventoryItemSO.GetGridPositionList(placedObjectOrigin);

        bool canPlace = true;
        foreach (Vector2Int gridPos in gridPositionList)
        {
            bool isValidPosition = grid.IsValidGridPosition(gridPos);
            if (!isValidPosition)
            {
                // Not valid
                canPlace = false;
                break;
            }
            if (!grid.GetGridObject(gridPos.x, gridPos.y).CanBuild())
            {
                canPlace = false;
                break;
            }
        }

        if (canPlace)
        {

            // Object Placed!
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y);
            InventoryItem inventoryItem = InventoryItem.CreateCanvas(itemContainer, placedObjectWorldPosition, placedObjectOrigin, inventoryItemSO);
            inventoryItem.GetComponent<InventoryDragDrop>().Setup(this);
            foreach (Vector2Int gridPos in gridPositionList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObject(inventoryItem);
            }

            inventoryItemSO.gridCoordinates = placedObjectOrigin;
            inventoryItemSO.broocheData.gridCoordinates = placedObjectOrigin;

            Debug.Log("Placing item");
            return true;

        }
        else
        {
            // Object CANNOT be placed!
            //Debug.Log("Cannot place item");
            return false;
        }
    }

    public void RemoveItemAt(Vector2Int removeGridPosition)
    {
        InventoryItem inventoryItem = grid.GetGridObject(removeGridPosition.x, removeGridPosition.y).GetPlacedObject();

        if (inventoryItem != null)
        {
            // Demolish
            Destroy(inventoryItem.gameObject);

            List<Vector2Int> gridPositionList = inventoryItem.GetGridPositionList();
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
            }
        }
    }

    public RectTransform GetItemContainer()
    {
        return itemContainer;
    }


}
