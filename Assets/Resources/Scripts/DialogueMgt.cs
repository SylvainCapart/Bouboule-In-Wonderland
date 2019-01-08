using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMgt : MonoBehaviour {

    private Queue<string> sentences;
    [Header("Optional : ")] public Text nameText;
    public Text dialogueText;
    public Animator animator;
    [SerializeField]
    private int m_ClickNumber;
    private bool m_DialogueActivated;
    [SerializeField] private GameObject m_ContinueButton;
    private bool dialogueReactivated;

    private PlayerMovement m_PlayerMov;


    // Use this for initialization
    void Start () {

        sentences = new Queue<string>();
        dialogueReactivated = false;
        if (m_ContinueButton == null)
            Debug.LogError(name + " : continue button is missing");

        m_PlayerMov = FindObjectOfType<PlayerMovement>();

    }

    private void Update()
    {
        if(!dialogueReactivated)
        {
            //ActivateDialogue();
            //GameObject.Find("ContinueText").GetComponent<TextBlinker>().Awake();
            dialogueReactivated = true;
        }
    }


    public void StartDialogue(Dialogue dialogue)
    {
        m_DialogueActivated = true;
        m_ClickNumber = 0;
        animator.SetBool("PanelOpen", true);
        m_PlayerMov.m_IsMovementAllowed = false;

        if (nameText != null)
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
        ++m_ClickNumber;

        if (m_ClickNumber == 0)
        {
            //
        }

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

        //if (Input.GetButtonDown("Fire1") && m_DialogueActivated)
        // {
        m_ContinueButton.SetActive(false);
        m_DialogueActivated = false;
        animator.SetBool("PanelOpen", false);
       //}
        //GameObject.Find("ContinueButton").gameObject.SetActive(false);

    }

    void ActivateDialogue()
    {
        //GameObject.Find("DialogueBox").transform.Find("ContinueButton").gameObject.SetActive(true);
        //if (Input.GetButtonDown("Fire1") && !m_DialogueActivated)
        //{
        m_ContinueButton.SetActive(true);
        m_DialogueActivated = true;
        animator.SetBool("PanelOpen", true);
        //}
    }

}
