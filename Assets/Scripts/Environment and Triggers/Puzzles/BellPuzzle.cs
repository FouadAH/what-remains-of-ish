using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellPuzzle : MonoBehaviour
{
    public List<Bell> bells= new List<Bell>();
    public GameEvent OnPuzzleEnd;
    int activeCount;

    private void OnEnable()
    {
        activeCount = 0;
        foreach (var bell in bells)
        {
            bell.OnTriggered += Bell_OnTriggered;
            bell.OnDeactivated += Bell_OnDeactivated;
        }
    }

    private void OnDisable()
    {
        foreach (var bell in bells)
        {
            bell.OnTriggered -= Bell_OnTriggered;
            bell.OnDeactivated -= Bell_OnDeactivated;
        }
    }

    private void Bell_OnDeactivated()
    {
        activeCount--;
    }

    private void Bell_OnTriggered()
    {
        activeCount++;

        if(activeCount == bells.Count) 
        {
            OnPuzzleEnd.Raise();
        }
    }
}
