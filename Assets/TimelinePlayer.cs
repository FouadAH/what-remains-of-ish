using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelinePlayer : MonoBehaviour
{
    PlayableDirector playableDirector;
    Player_Input player_Input;
    public bool playOnStart;

    void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
        playableDirector.played += PlayableDirector_played;
        playableDirector.stopped += PlayableDirector_stopped;
    }

    private void Start()
    {
        if (GameManager.instance.player == null)
            player_Input = FindObjectOfType<Player_Input>();
        else
            player_Input = GameManager.instance.player.GetComponent<Player_Input>();

        if (playOnStart)
            StartTimeline();
    }

    private void PlayableDirector_stopped(PlayableDirector obj)
    {
        player_Input.EnablePlayerInput();
    }

    private void PlayableDirector_played(PlayableDirector obj)
    {
        player_Input.DisablePlayerInput();
    }

    public void StartTimeline()
    {
        playableDirector.Play();
    }

}
