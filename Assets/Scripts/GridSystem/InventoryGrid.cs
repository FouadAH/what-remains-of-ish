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
    public InventoryItemSO currentItemTest;

    public List<InventoryItemSO> items = new List<InventoryItemSO>();


    private void Awake()
    {
        Instance = this;

        float cellSize = 100f;
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
    }
 

    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int y;
        public InventoryItem inventoryItem;

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
            return inventoryItem == null;
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

    private void Start()
    {
        foreach (InventoryItemSO item in items)
        {
            TryAutoPlaceObject(item);
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { currentItemTest = items[0]; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { currentItemTest = items[1]; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { currentItemTest = items[2]; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { currentItemTest = items[3]; }


        // Test Can Build
        if (Input.GetMouseButtonDown(0))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, Input.mousePosition, null, out Vector2 testAnchoredPosition);
            grid.GetXY(testAnchoredPosition, out int anchoredX, out int anchoredY);
            Vector2Int placedObjectOrigin = GetGridPosition(testAnchoredPosition);

            TryPlace(currentItemTest, placedObjectOrigin);
        }
    }

    public void TryAutoPlaceObject(InventoryItemSO inventoryItemSO)
    {
        for (int y = grid.GetHeight(); y >= 0; y--)
        {
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                bool isPlaced = TryPlace(inventoryItemSO, new Vector2Int(x, y));

                if (isPlaced)
                {
                    return;
                }
            }
        }
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

            Debug.Log("Placing item");
            return true;

        }
        else
        {
            // Object CANNOT be placed!
            Debug.Log("Cannot place item");
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
