using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, 
    IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private CanvasGroup canvasGroup;

    private InventoryGrid inventoryTetris;
    private InventoryItem placedObject;
    private Image image;
    private GameObject outlineMask;

    private void Awake() 
    {
        canvasGroup = GetComponent<CanvasGroup>();
        placedObject = GetComponent<InventoryItem>();
        image = GetComponent<Image>();
        outlineMask = GetComponentInChildren<Mask>().gameObject;
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

        //InventoryItemSO.CreateVisualGrid(transform.GetChild(0), placedObject.GetPlacedObjectTypeSO() as ItemTetrisSO, inventoryTetris.GetGrid().GetCellSize());
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
        InventoryDragDropSystem.Instance.OnClickedInventoryItem(inventoryTetris, placedObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outlineMask.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outlineMask.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        outlineMask.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        outlineMask.SetActive(false);
    }
}
