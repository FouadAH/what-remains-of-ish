using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StringEvent : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<StringEventListener> eventListeners =
        new List<StringEventListener>();

    public void Raise(string eventContent)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(eventContent);
    }

    public void RegisterListener(StringEventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(StringEventListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}
