using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI dialogueText;
    private int index;
    public float typingSpeed;
    StringBuilder sb = new StringBuilder("", 50);
    private Queue<string> sentences;

    public GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        
    }

    void Start()
    {
        sentences = new Queue<string>();
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
        animator.SetBool("isOpen", false);
        gm.player.GetComponent<Player_Input>().enabled = true;
    }
}
