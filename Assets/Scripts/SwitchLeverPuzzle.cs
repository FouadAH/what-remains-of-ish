using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLeverPuzzle : MonoBehaviour
{
    public List<SwitchLever> switchLevers = new List<SwitchLever>();
    public float timeInSeconds = 30f;
    public Door door;

    bool timerIsActive;
    bool isDone;
    bool allLeversActive = true;

    // Start is called before the first frame update
    void Start()
    {
        if (isDone)
        {
            foreach (SwitchLever switchLever in switchLevers)
            {
                switchLever.SetActive(true);
                door.SetStateInitial(true);
            }
            return;
        }

        door.SetStateInitial(false);
        foreach (SwitchLever switchLever in switchLevers)
        {
            switchLever.OnTriggerLever += OnLeverTriggered;
            switchLever.SetActive(false);
        }
    }

    void OnLeverTriggered()
    {
        if (!timerIsActive && !isDone)
        {
            StartCoroutine(TimerPuzzle());
        }
        else
        {
            CheckSwitches();
        }
    }
    
    void CheckSwitches()
    {
        allLeversActive = true;
        foreach (SwitchLever switchLever in switchLevers)
        {
            Debug.Log("Lever: " +switchLever.name+" Active: " + allLeversActive);
            if (!switchLever.isActive)
            {
                allLeversActive = false;
                break;
            }
        }
        if (allLeversActive)
        {
            EndPuzzle();
        }
    }

    void CheckPuzzleState()
    {
        if (!allLeversActive) 
        { 
            ResetPuzzle();
        }
    }
    private void ResetPuzzle()
    {
        foreach (SwitchLever switchLever in switchLevers)
        {
            switchLever.SetActive(false);
        }
        UI_HUD.instance.debugTimer.SetActive(false);
    }

    private void EndPuzzle()
    {
        isDone = true;
        door.SetState(true);
        StopAllCoroutines();
        timerIsActive = false;
        UI_HUD.instance.debugTimer.SetActive(false);
    }

    IEnumerator TimerPuzzle()
    {
        timerIsActive = true;

        UI_HUD.instance.debugTimer.SetActive(true);
        UI_HUD.instance.debugTimerText.SetText(timeInSeconds.ToString());

        int elapsedSeconds = 0;
        while (elapsedSeconds < timeInSeconds)
        {
            yield return new WaitForSeconds(1);
            elapsedSeconds++;
            int elapsedTime = (int)timeInSeconds - elapsedSeconds;
            UI_HUD.instance.debugTimerText.SetText(elapsedTime.ToString());
        }

        CheckPuzzleState();
        timerIsActive = false;
    }
}
