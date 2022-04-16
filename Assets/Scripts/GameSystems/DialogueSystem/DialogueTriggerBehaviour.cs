using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;

[Serializable]
public class DialogueTriggerBehaviour : PlayableBehaviour
{
    public DialogueNodeSO dialogAsset;
    public bool JumpToEnd = false;

    private PlayableGraph graph;
    private Playable thisPlayable;
    private EventHandler dialogueEndHandler;
    private bool began = false;

    public override void OnPlayableCreate(Playable playable)
    {
        graph = playable.GetGraph();
        thisPlayable = playable;
        began = false;

        dialogueEndHandler = OnDialogueEnd;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (dialogAsset != null && !began)
        {
            if (DialogManager.instance)
            {
                graph.GetRootPlayable(0).SetSpeed(0);
                DialogManager.instance.OnDialogueClipEnd += dialogueEndHandler;
                DialogManager.instance.StartDialogue(dialogAsset.dialog, GetType());
                began = true;
            }
            else
            {
                if (JumpToEnd && began) JumpToEndofPlayable();
                graph.GetRootPlayable(0).SetSpeed(1);
            }
        }
    }


    public void OnDialogueEnd(object sender, EventArgs args)
    {
        DialogManager.instance.OnDialogueClipEnd -= dialogueEndHandler;
        graph.GetRootPlayable(0).SetSpeed(1);
        if (JumpToEnd) JumpToEndofPlayable();
    }

    private void JumpToEndofPlayable()
    {
        graph.GetRootPlayable(0).SetTime(graph.GetRootPlayable(0).GetTime() + thisPlayable.GetDuration());
    }
}