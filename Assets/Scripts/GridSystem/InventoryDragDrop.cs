using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private CanvasGroup canvasGroup;

    private InventoryGrid inventoryTetris;
    private InventoryItem placedObject;
    private Image image;

    private void Awake() 
    {
        canvasGroup = GetComponent<CanvasGroup>();
        placedObject = GetComponent<InventoryItem>();
        image = GetComponent<Image>();
    }

    public void Setup(InventoryGrid inventoryTetris)
    {
        this.inventoryTetris = inventoryTetris;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        canvasGroup.alpha = .7f;
        canvasGroup.blocksRaycasts = false;
        image.maskable = false;
        placedObject.transform.parent = InventoryDragDropSystem.Instance.transform;

        //ItemTetrisSO.CreateVisualGrid(transform.GetChild(0), placedObject.GetPlacedObjectTypeSO() as ItemTetrisSO, inventoryTetris.GetGrid().GetCellSize());
        InventoryDragDropSystem.Instance.StartedDragging(inventoryTetris, placedObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        image.maskable = true;

        InventoryDragDropSystem.Instance.StoppedDragging(inventoryTetris, placedObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
    }


}
