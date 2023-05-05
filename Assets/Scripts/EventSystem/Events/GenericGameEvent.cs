using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

[CreateAssetMenu(fileName = "newGenericEvent", menuName = "Game Events/New Generic Event")]
public class GenericGameEvent<T> : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<GenericEventListener<T>> eventListeners =
        new List<GenericEventListener<T>>();

    public void Raise(T eventContent)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(eventContent);
    }

    public void RegisterListener(GenericEventListener<T> listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(GenericEventListener<T> listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}
