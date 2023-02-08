using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwitchLeverPuzzle : Savable
{
    [System.Serializable]
    public struct Data
    {
        public bool isDone;
    }

    [SerializeField]
    private Data puzzleData;

    public List<SwitchLever> switchLevers = new List<SwitchLever>();
    public float timeInSeconds = 30f;
    public Door door;

    [Header("Timer UI")]
    public GameObject debugTimer;
    public TMP_Text debugTimerText;

    bool timerIsActive;
    bool isDone;
    bool allLeversActive = true;

    EnemyAudio enemyAudio;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        enemyAudio = GetComponent<EnemyAudio>();
    }

    void OnLeverTriggered()
    {
        if (!timerIsActive && !isDone)
        {
            StartCoroutine(TimerPuzzle());
            enemyAudio.PlayEventOnce("event:/SFX/Interactive Objects/Ticking Puzzle");

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
        debugTimer.SetActive(false);
        enemyAudio.StopPlayingEvent();
    }

    private void EndPuzzle()
    {
        isDone = true;
        puzzleData.isDone = true;
        door.SetState(true);
        StopAllCoroutines();
        timerIsActive = false;
        debugTimer.SetActive(false);
    }

    IEnumerator TimerPuzzle()
    {
        timerIsActive = true;

        debugTimer.SetActive(true);
        debugTimerText.SetText(timeInSeconds.ToString());

        int elapsedSeconds = 0;
        while (elapsedSeconds < timeInSeconds)
        {
            yield return new WaitForSeconds(1);
            elapsedSeconds++;
            int elapsedTime = (int)timeInSeconds - elapsedSeconds;
            debugTimerText.SetText(elapsedTime.ToString());
        }

        CheckPuzzleState();
        timerIsActive = false;
    }

    public override string SaveData()
    {
        return JsonUtility.ToJson(puzzleData);
    }

    public override void LoadDefaultData()
    {
        puzzleData.isDone = false;
        door.SetStateInitial(false);
        foreach (SwitchLever switchLever in switchLevers)
        {
            switchLever.OnTriggerLever += OnLeverTriggered;
            switchLever.SetActive(false);
        }
    }

    public override void LoadData(string data, string version)
    {
        puzzleData = JsonUtility.FromJson<Data>(data);
        Debug.Log("loading puzzle state: " + puzzleData.isDone);

        if (puzzleData.isDone)
        {
            foreach (SwitchLever switchLever in switchLevers)
            {
                switchLever.SetActive(true);
                door.SetStateInitial(true);
            }
            return;
        }
        else
        {
            door.SetStateInitial(false);
            foreach (SwitchLever switchLever in switchLevers)
            {
                switchLever.OnTriggerLever += OnLeverTriggered;
                switchLever.SetActive(false);
            }
        }
    }
}
