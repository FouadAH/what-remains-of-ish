using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStop : MonoBehaviour
{
    float speed;
    bool restoreTime = false;
    public bool timeStopIsActive = false;
    Player_Input player_Input;

    public static TimeStop instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

    private void Start()
    {
        player_Input = FindObjectOfType<Player_Input>();
    }

    void Update()
    {
        if (restoreTime)
        {
            if(Time.timeScale < 1f)
            {
                Time.timeScale += Time.deltaTime * speed;
            }
            else
            {
                Time.timeScale = 1f;
                player_Input.enabled = true;
                timeStopIsActive = false;

                restoreTime = false;
            }
        }
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
    }

    public void StopTime(float changeTime, float restoreSpeed, float delay)
    {
        timeStopIsActive = true;
        speed = restoreSpeed;

        if (delay > 0)
        {
            StopCoroutine(StartTimeAgain(delay));
            StartCoroutine(StartTimeAgain(delay));
        }
        else
        {
            restoreTime = true;
        }

        player_Input.enabled = false;
        Time.timeScale = changeTime;
    }

    IEnumerator StartTimeAgain(float atm)
    {
        yield return new WaitForSecondsRealtime(atm);
        restoreTime = true;
    }
}
