using System.Collections;
using UnityEngine;

public class ItemSO : BaseScriptableObject
{
    public bool isOwnedByPlayer;

    public virtual void ReceiveItem()
    {
        isOwnedByPlayer = true;
    }
}
