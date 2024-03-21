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

    PlayerInputMaster playerInputMaster;
    Player_Input player_Input;

    bool isSelected = false;
    bool isDragging = false;

    private void Awake() 
    {
        player_Input = FindObjectOfType<Player_Input>();
        playerInputMaster= player_Input.inputActions;
        canvasGroup = GetComponent<CanvasGroup>();
        placedObject = GetComponent<InventoryItem>();
        image = GetComponent<Image>();
        outlineMask = GetComponentInChildren<Mask>().gameObject;
        outlineMask.SetActive(false);
        playerInputMaster.UI.Submit.started += Click_started;
        playerInputMaster.UI.BroocheEquip.started += BroocheEquip_started;
    }

    private void BroocheEquip_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!isSelected)
        {
            return;
        }

        if (isDragging)
        {
            isDragging = false;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            image.maskable = true;
        }

        InventoryDragDropSystem.Instance.ToggleEquip(inventoryTetris, placedObject);
        InventoryDragDropSystem.Instance.isDragging = false;
        isSelected = false;
    }

    private void Click_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!isSelected)
        {
            return;
        }

        if (!isDragging)
        {
            isDragging = true;
            canvasGroup.alpha = .7f;
            canvasGroup.blocksRaycasts = false;
            image.maskable = false;
            placedObject.transform.parent = InventoryDragDropSystem.Instance.transform;

            InventoryDragDropSystem.Instance.StartedDraggingController(inventoryTetris, placedObject);
        }
        else if(isDragging)
        {
            isDragging = false;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            image.maskable = true;

            isSelected = false;
            InventoryDragDropSystem.Instance.StoppedDragging_Controller(inventoryTetris, placedObject);
        }
    }

    public void Setup(InventoryGrid inventoryTetris)
    {
        this.inventoryTetris = inventoryTetris;

        if(!EventSystem.current.alreadySelecting)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        outlineMask.SetActive(true);

        canvasGroup.alpha = .7f;
        canvasGroup.blocksRaycasts = false;
        image.maskable = false;
        placedObject.transform.parent = InventoryDragDropSystem.Instance.transform;

        //InventoryItemSO.CreateVisualGrid(transform.GetChild(0), placedObject.GetPlacedObjectTypeSO() as ItemTetrisSO, inventoryTetris.GetGrid().GetCellSize());
        InventoryDragDropSystem.Instance.StartedDragging(inventoryTetris, placedObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        outlineMask.SetActive(false);

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
        if (!player_Input.controllerConnected)
        {
            InventoryDragDropSystem.Instance.OnClickedInventoryItem(inventoryTetris, placedObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outlineMask.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outlineMask.SetActive(false);
    }


    public void Select()
    {
        if (InventoryDragDropSystem.Instance.isDragging)
        {
            return;
        }

        if (player_Input.controllerConnected)
        {
            InventoryDragDropSystem.Instance.OnClickedInventoryItem(inventoryTetris, placedObject);
        }

        outlineMask.SetActive(true);
        isSelected = true;
    }

    public void Deselect()
    {
        if (InventoryDragDropSystem.Instance.isDragging)
        {
            return;
        }

        outlineMask.SetActive(false);
        isSelected = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        Select();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (InventoryDragDropSystem.Instance.isDragging)
        {
            return;
        }

        outlineMask.SetActive(false);
        isSelected = false;
    }

    private void OnDisable()
    {
        Deselect();
    }
}
