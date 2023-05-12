using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlockedInventoryItem : InventoryItem
{
    public PlayerDataSO PlayerDataSO;
    public GameEvent OnUseYarn;
    public UnityAction<int,int> OnUnblocked;

    Selectable selectable;
    bool isDestroying; 

    void Start()
    {
        InitBlockedTile();
    }

    private void OnEnable()
    {
        InitBlockedTile();
    }

    public void InitBlockedTile()
    {
        if(isDestroying) return;

        if (selectable == null)
        {
            selectable = GetComponent<Selectable>();
        }

        selectable.interactable = (PlayerDataSO.playerYarnAmount.Value > 0);
    }

    public void OnYarnItemUsed()
    {
        InitBlockedTile();
    }

    public void OnSelect()
    {
        isDestroying = true;
        PlayerDataSO.playerYarnAmount.Value--;
        OnUseYarn.Raise();

        OnUnblocked?.Invoke(GetGridPosition().x, GetGridPosition().y);
    }
}
