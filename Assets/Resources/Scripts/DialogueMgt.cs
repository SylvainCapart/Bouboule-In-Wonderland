using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMgt : MonoBehaviour {

    private Queue<string> sentences;
    public Text nameText;
    public Text dialogueText;
    public Animator animator;
    [SerializeField]
    private int clickNumber;

    private bool dialogueReactivated;



    // Use this for initialization
    void Start () {
        Debug.Log("test");
        sentences = new Queue<string>();
        dialogueReactivated = false;

    }

    private void Update()
    {
        if(!dialogueReactivated)
        {
            ActivateDialogue();
            //GameObject.Find("ContinueText").GetComponent<TextBlinker>().Awake();
            dialogueReactivated = true;
        }
    }


    public void StartDialogue(Dialogue dialogue)
    {
        clickNumber = 0;
        animator.SetBool("isOpen", true);

        nameText.text = dialogue.name;
        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
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
        ++clickNumber;
        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        Debug.Log("End Dialogue");

    }

    void DeactivateDialogue()
    {
        GameObject.Find("ContinueButton").gameObject.SetActive(false);
        animator.SetBool("isOpen", false);
    }

    void ActivateDialogue()
    {
        GameObject.Find("DialogueBox").transform.Find("ContinueButton").gameObject.SetActive(true);

    }

    public void incrementSquirrelsDestroyed()
    {

    }


}
