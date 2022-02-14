using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalManager : MonoBehaviour
{
    public List<JournalEntrySO> journalEntries = new List<JournalEntrySO>();
    public GameObject journalEntriesContent;
    public GameObject entryViewPrefab;
    public TMPro.TMP_Text journalEntryTitle;
    public TMPro.TMP_Text journalEntryContent;

    public void DisplayEntries()
    {
        ClearEntries();

        foreach (JournalEntrySO entry in journalEntries)
        {
            if (entry.hasBeenFound)
            {
                Instantiate(entryViewPrefab, journalEntryContent.transform);
            }
        }
    }

    public void ClearEntries()
    {
        for (int i = 0; i < journalEntryContent.transform.childCount; i++)
        {
            Destroy(journalEntryContent.transform.GetChild(i).gameObject);
        }
    }
}
