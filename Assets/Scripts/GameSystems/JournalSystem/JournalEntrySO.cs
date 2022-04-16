using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newJournal", menuName = "Items/New Journal", order = 1)]
public class JournalEntrySO : ItemSO
{
    public int journalEntryID;
    public string journalTitle;
    [TextArea(5,10)] public string journalContent;
    public bool hasBeenFound;
    public bool hasBeenRead;
    public StringEvent journalPickupEvent;

    public override void ReceiveItem()
    {
        hasBeenFound = true;
        journalPickupEvent.Raise("\"" + journalTitle + "\" added to journal entries");
    }
}
