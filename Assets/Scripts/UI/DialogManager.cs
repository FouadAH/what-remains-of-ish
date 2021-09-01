using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed;
    public bool dialogueIsActive = false;

    int index;
    StringBuilder sb = new StringBuilder("", 50);
    Queue<string> sentences;
    GameManager gm;

    public static DialogManager instance;

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
        gm = FindObjectOfType<GameManager>();
        sentences = new Queue<string>();
    }

    private void Update()
    {
        if (!dialogueIsActive || GameManager.instance.isPaused)
            return;

        if(Input.GetButtonDown("Attack"))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialog dialogue)
    {
        gm.player.GetComponent<Player_Input>().enabled = false;
        animator.SetBool("isOpen", true);

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
        dialogueIsActive = true;
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        sb.Clear();
        foreach (char letter in sentence.ToCharArray())
        {
            sb.Append(letter);
            dialogueText.text = sb.ToString();
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialogue()
    {
        dialogueIsActive = false;
        animator.SetBool("isOpen", false);
        gm.player.GetComponent<Player_Input>().enabled = true;
    }
}
