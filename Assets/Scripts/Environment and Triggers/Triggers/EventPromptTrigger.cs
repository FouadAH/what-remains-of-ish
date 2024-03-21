using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPromptTrigger : PromptTrigger
{
    public GameEvent OnInteractEvent;

    public override void Interact()
    {
        base.Interact();
        OnInteractEvent.Raise();
    }

}
