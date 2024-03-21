using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityInventoryItem : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    public AbilityInventoryItemSO AbilityInventoryItemSO;
    public Action<AbilityInventoryItemSO> OnSelectItem;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelectItem?.Invoke(AbilityInventoryItemSO);
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelectItem?.Invoke(AbilityInventoryItemSO);
    }
}
