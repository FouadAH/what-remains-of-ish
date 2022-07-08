using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueTimelineTrigger : Savable
{
    PlayableDirector playableDirector;
    Player_Input player_Input;

    protected Animator promptAnimator;
    protected DialogManager dialogueManager;
    protected Canvas promptCanvas;
    public DialogueNodeSO endDialogue;

    public string promptText = "Talk";

    public TimelineData timelineData;
    //public Cinemachine.CinemachineVirtualCamera cutsceneCAM;

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
        player_Input = GameManager.instance.player.GetComponent<Player_Input>();
        promptCanvas = GetComponentInChildren<Canvas>();
        promptAnimator = promptCanvas.GetComponent<Animator>();
        dialogueManager = DialogManager.instance;
    }

    private void PlayableDirector_stopped(PlayableDirector obj)
    {
        player_Input.EnablePlayerInput();
    }

    private void PlayableDirector_played(PlayableDirector obj)
    {
        player_Input.DisablePlayerInput();
    }


    protected void DisplayPrompt()
    {
        promptAnimator.ResetTrigger("PopOut");
        promptAnimator.SetTrigger("PopIn");
    }

    protected void RemovePrompt()
    {
        promptAnimator.ResetTrigger("PopIn");
        promptAnimator.SetTrigger("PopOut");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        promptCanvas.GetComponentInChildren<TMPro.TMP_Text>().text = promptText;

        if (collision.GetComponent<Player>())
        {
            DisplayPrompt();
        }
    }

    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButtonDown("Interact"))
        {
            Interact();
        }

        if (!dialogueManager.dialogueIsActive)
        {
            DisplayPrompt();
        }
        else
        {
            RemovePrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            RemovePrompt();
        }
    }

    public virtual void Interact()
    {
        if (!dialogueManager.dialogueIsActive)
        {
            if (timelineData.hasPlayed) 
            {
                dialogueManager.StartDialogue(endDialogue.dialog, GetType());
            }
            else
            {
                StartTimeline();
            }
        }
    }

    public void StartTimeline()
    {
        //cutsceneCAM.enabled = true;
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
            //cutsceneCAM.enabled = false;
        }
    }
}
