using UnityEngine;
using UnityEngine.Events;

public class StringEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public StringEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<string> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(string eventContent)
    {
        Response.Invoke(eventContent);
    }
}
