using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorials")]
    public GameObject healingTutorial;
    public GameObject wallJumpTutorial;
    public GameObject dashTutorial;
    public GameObject teleportTutorial;
    public GameObject boomerangTutorial;
    public GameObject restingTutorial;

    public bool tutorialIsActive = false;

    private GameObject currentActiveTutorial;
    private GameManager gm;
    private bool inputLock = true;

    public static TutorialManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (!tutorialIsActive || GameManager.instance.isPaused)
            return;
        
        if (!inputLock && Input.GetButtonDown("Interact"))
        {
            CloseTutorial();
        }
    }

    IEnumerator InputLock()
    {
        inputLock = true;
        yield return new WaitForSeconds(0.05f);
        inputLock = false;
    }

    public void DisplayTutorial(TutorialType tutorialType)
    {
        switch (tutorialType)
        {
            case TutorialType.Healing:
                Display(healingTutorial);
                break;

            case TutorialType.WallJump:
                Display(wallJumpTutorial);
                break;

            case TutorialType.Dash:
                Display(dashTutorial);
                break;

            case TutorialType.Teleport:
                Display(teleportTutorial);
                break;

            case TutorialType.Boomerang:
                Display(boomerangTutorial);
                break;

            case TutorialType.Resting:
                Display(restingTutorial);
                break;

            default:
                break;
        }
    }

    public void BoomerangTutorial()
    {
        DisplayTutorial(TutorialType.Boomerang);
    }

    void Display(GameObject tutorial)
    {
        inputLock = true;
        StartCoroutine(InputLock());

        tutorial.SetActive(true);
        currentActiveTutorial = tutorial;
        DialogManager.instance.dialogueIsActive = true;
        tutorialIsActive = true;

        gm.player.GetComponent<Player_Input>().DisablePlayerInput();
        gm.player.GetComponent<Player>().enabled = false;
    }

    void CloseTutorial()
    {
        inputLock = true;

        currentActiveTutorial.SetActive(false);
        currentActiveTutorial = null;
        DialogManager.instance.dialogueIsActive = false;
        tutorialIsActive = false;

        gm.player.GetComponent<Player_Input>().EnablePlayerInput();
        gm.player.GetComponent<Player>().enabled = true;

    }
}

public enum TutorialType
{
    Healing = 0,
    WallJump = 1,
    Dash = 2,
    Teleport = 3,
    Boomerang = 4,
    Resting = 5
}
