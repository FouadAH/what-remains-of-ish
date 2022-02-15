using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newArticaft", menuName = "Items/New Artifact", order = 1)]
public class Artifact : ItemSO
{
    public override void ReceiveItem()
    {
        base.ReceiveItem();
        UI_HUD.instance.SetDebugText("Picked up an artifact!");
    }
}
