using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newInventoryItemSO", menuName = "UI Items/New Ability Inventory Item")]
public class AbilityInventoryItemSO : ScriptableObject
{
    public string itemName;

    [TextArea]
    public string tutorialPromptText;

    [TextArea]
    public string descriptionText;

    public AbilityType abilityType;
}
