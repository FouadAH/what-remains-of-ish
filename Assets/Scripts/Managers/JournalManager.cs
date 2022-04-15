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
                JournalEntry journalEntry = Instantiate(entryViewPrefab, journalEntriesContent.transform).GetComponent<JournalEntry>();
                journalEntry.journalTitle.text = entry.journalTitle;
                journalEntry.button.onClick.AddListener(() => OnClickJournalEntry(entry));
            }
        }
    }

    public void ClearEntries()
    {
        for (int i = 0; i < journalEntriesContent.transform.childCount; i++)
        {
            Destroy(journalEntriesContent.transform.GetChild(i).gameObject);
        }
    }

    void OnClickJournalEntry(JournalEntrySO journalEntry)
    {
        journalEntryTitle.text = journalEntry.journalTitle;
        journalEntryContent.text = journalEntry.journalContent;
        journalEntry.hasBeenRead = true;
    }

    private void OnEnable()
    {
        DisplayEntries();
    }
}
