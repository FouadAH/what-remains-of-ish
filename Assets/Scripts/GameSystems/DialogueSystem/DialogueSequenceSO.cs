using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDialogueSequence", menuName = "New Dialogue Sequence", order = 1)]
public class DialogueSequenceSO : ScriptableObject
{
    public List<DialogueNodeSO> dialogeNodes;
}

