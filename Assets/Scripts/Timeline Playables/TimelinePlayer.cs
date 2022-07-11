using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelinePlayer : Savable
{
    PlayableDirector playableDirector;
    Player_Input player_Input;
    public bool playOnStart;
    public bool playOnce;
    public TimelineData timelineData;
    public Cinemachine.CinemachineVirtualCamera cutsceneCAM;

    public struct TimelineData
    {
       public bool hasPlayed;
    }

    public override void Awake()
    {
        base.Awake();
        playableDirector = GetComponent<PlayableDirector>();
        playableDirector.played += PlayableDirector_played;
        playableDirector.stopped += PlayableDirector_stopped;
    }

    public override void Start()
    {
        base.Start();

        if (GameManager.instance.player == null)
            player_Input = FindObjectOfType<Player_Input>();
        else
            player_Input = GameManager.instance.player.GetComponent<Player_Input>();

        //#if UNITY_EDITOR

        //        if (!timelineData.hasPlayed)
        //        {
        //            if (GameManager.instance.player == null)
        //                player_Input = FindObjectOfType<Player_Input>();
        //            else
        //                player_Input = GameManager.instance.player.GetComponent<Player_Input>();

        //            if (playOnStart && !timelineData.hasPlayed)
        //                StartTimeline();
        //        }
        //        else
        //        {
        //            cutsceneCAM.enabled = false;
        //        }
        //#endif
    }

    private void PlayableDirector_stopped(PlayableDirector obj)
    {
        CutsceneManager.instance.isCutscenePlaying = false;
        player_Input.EnablePlayerInput();
    }

    private void PlayableDirector_played(PlayableDirector obj)
    {
        CutsceneManager.instance.isCutscenePlaying = true;
        player_Input.DisablePlayerInput();
    }

    public void StartTimeline()
    {
        cutsceneCAM.enabled = true;
        timelineData.hasPlayed = true;
        playableDirector.Play();
    }

    public override string SaveData()
    {
       return JsonUtility.ToJson(timelineData);
    }

    public override void LoadDefaultData()
    {
        Debug.Log("Loading timeline data default");

        timelineData.hasPlayed = false;
        if (!timelineData.hasPlayed)
        {
            if (GameManager.instance.player == null)
                player_Input = FindObjectOfType<Player_Input>();
            else
                player_Input = GameManager.instance.player.GetComponent<Player_Input>();

            if (playOnStart && !timelineData.hasPlayed)
                StartTimeline();
        }
        else
        {
            cutsceneCAM.enabled = false;
        }
    }

    public override void LoadData(string data, string version)
    {
        Debug.Log("Loading timeline data");
        timelineData = JsonUtility.FromJson<TimelineData>(data);
        if (!timelineData.hasPlayed)
        {
            if (GameManager.instance.player == null)
                player_Input = FindObjectOfType<Player_Input>();
            else
                player_Input = GameManager.instance.player.GetComponent<Player_Input>();

#if !UNITY_EDITOR
            if (playOnStart && !timelineData.hasPlayed)
                StartTimeline();
#endif
        }
        else
        {
            cutsceneCAM.enabled = false;
        }
    }
}
