using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class JournalEntrySO : ScriptableObject
{
    public int journalEntryID;
    public string journalTitle;
    [TextArea(5,10)] public string journalContent;
    public bool hasBeenFound;
    public bool hasBeenRead;
}
