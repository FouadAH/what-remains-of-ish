using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newArticaft", menuName = "Items/New Artifact", order = 1)]
public class Artifact : ItemSO
{
    public GameEvent ReceivedArtifactEvent;
    public override void ReceiveItem()
    {
        ReceivedArtifactEvent.Raise();
    }
}
