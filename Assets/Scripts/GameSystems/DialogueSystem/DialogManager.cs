using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;

    public float typingSpeed;
    public float typingSpeedSkip;

    float currentTypingSpeed;

    public bool dialogueIsActive = false;

    int index;
    StringBuilder sb = new StringBuilder("", 50);
    Queue<string> sentences;

    public event EventHandler OnDialogueClipEnd = delegate { };
    public static DialogManager instance;
    Player_Input player_Input;

    bool isTyping = false;
    string currentSentence;

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

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        player_Input = FindObjectOfType<Player_Input>();
        player_Input.OnDialogueNext += Player_Input_OnDialogueNext;
        sentences = new Queue<string>();
    }

    private void Player_Input_OnDialogueNext()
    {
        //Debug.Log("Player_Input_OnDialogueNext");

        //if (!dialogueIsActive || GameManager.instance.isPaused)
        //    return;

        //if (!inputLock)
        //{
        //    Debug.Log("inputlock check");

        //    if (isTyping)
        //    {
        //        Debug.Log("Skip");

        //        SkipToEndOfSentence();
        //    }
        //    else
        //    {
        //        DisplayNextSentence();
        //        Debug.Log("Next");

        //    }
        //}
    }

    public bool inputLock = true;
    private void Update()
    {

        if (!dialogueIsActive || GameManager.instance.isPaused)
            return;

        if (player_Input.inputActions.UI.DialogueNext.WasPressedThisFrame())
        {
            if (!inputLock)
            {
                if (isTyping)
                {
                    SkipToEndOfSentence();
                }
                else
                {
                    DisplayNextSentence();
                }
            }
        }
    }

    public void DisplaySentence(string sentence)
    {
        Debug.Log(sentence);
    }

    IEnumerator InputLock()
    {
        inputLock = true;
        yield return new WaitForSeconds(0.05f);
        inputLock = false;
        //Debug.Log(inputLock);
    }
    Type currentContext;
    public void StartDialogue(Dialog dialogue, Type context)
    {
        currentContext = context;

        inputLock = true;
        StartCoroutine(InputLock());

        if (!dialogueIsActive)
        {
            dialogueIsActive = true;
            player_Input.DisablePlayerInput();
            animator.SetBool("isOpen", true);
        }

        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        speakerText.text = dialogue.speakerName;
        DisplayNextSentence();
    }

    Coroutine typeSentenceRoutine;
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            //Debug.Log(currentContext);
            if(currentContext.Equals(typeof(DialogueTriggerBehaviour)))
            {
                EndDialogueClip();
            }
            else
            {
                EndDialogue();
            }
            return;
        }

        currentSentence = sentences.Dequeue();

        if(typeSentenceRoutine!=null)
            StopCoroutine(typeSentenceRoutine);

        currentTypingSpeed = typingSpeed;
        typeSentenceRoutine = StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        sb.Clear();
        foreach (char letter in sentence.ToCharArray())
        {
            sb.Append(letter);
            dialogueText.text = sb.ToString();
            yield return new WaitForSeconds(currentTypingSpeed);
        }
        isTyping = false;
    }

    void SkipToEndOfSentence()
    {
        isTyping = false;
        if (typeSentenceRoutine != null)
            StopCoroutine(typeSentenceRoutine);

        dialogueText.text = currentSentence;
    }

    void EndDialogueClip()
    {
        Debug.Log("End dialogue from clip");
        inputLock = true;
        dialogueIsActive = false;
        OnDialogueClipEnd.Invoke(this, null);
        animator.SetBool("isOpen", false);
    }

    public void EndDialogue()
    {
        inputLock = true;
        dialogueIsActive = false;
        OnDialogueClipEnd.Invoke(this, null);
        player_Input.EnablePlayerInput();
        animator.SetBool("isOpen", false);
    }

    public void OnInteractStart()
    {
        dialogueIsActive = true;
        player_Input.DisablePlayerInput();
    }

    public void OnInteractEnd()
    {
        dialogueIsActive = false;
        player_Input.EnablePlayerInput();
    }
}
