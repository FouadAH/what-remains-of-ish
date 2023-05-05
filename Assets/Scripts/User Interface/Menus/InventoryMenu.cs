using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    public GridLayoutGroup abilityGrid;
    
    public TMPro.TMP_Text titleText;
    public TMPro.TMP_Text tutorialText;
    public TMPro.TMP_Text descriptionText;

    public List<AbilityInventoryItem> abilityItems;

    public AbilityPickupChannel pickupChannel;

    private void Start()
    {
        pickupChannel.OnPickupAbility += EnableAbilityItem;

        foreach(var item in abilityItems)
        {
            item.OnSelectItem += OnSelectInventoryItem;
        }
    }

    private void OnDestroy()
    {
        pickupChannel.OnPickupAbility -= EnableAbilityItem;

        foreach (var item in abilityItems)
        {
            item.OnSelectItem -= OnSelectInventoryItem;
        }
    }

    public void EnableAbilityItem(AbilityType abilityType)
    {
        AbilityInventoryItem abilityInventoryItem = abilityItems.Find((obj) => obj.AbilityInventoryItemSO.abilityType == abilityType);
        abilityInventoryItem.gameObject.SetActive(true);
    }

    public void OnSelectInventoryItem(AbilityInventoryItemSO abilityInventoryItemSO)
    {
        titleText.text = abilityInventoryItemSO.itemName;
        descriptionText.text = abilityInventoryItemSO.descriptionText;
    }
}
