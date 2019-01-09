using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMgt : MonoBehaviour {

    private Queue<string> sentences;
    [Header("Optional : ")] public Text nameText;
    public Text dialogueText;
    [SerializeField] private Animator m_DialogueBoxAnim;
    [SerializeField] private Animator m_SceneAnim;
    private bool m_DialogueActivated;
    [SerializeField] private GameObject m_ContinueButton;
    //private bool dialogueReactivated;
    [SerializeField] private int[] m_SceneIndex; // for n dialogue sentences, a scene at sentence x should have index n - x

    private PlayerMovement m_PlayerMov;
    private Coroutine m_TypeSentenceCo;



    // Use this for initialization
    void Start () {

        sentences = new Queue<string>();

        if (m_ContinueButton == null)
            Debug.LogError(name + " : continue button is missing");

        m_PlayerMov = FindObjectOfType<PlayerMovement>();

    }

    private void Update()
    {
       /* if(!dialogueReactivated)
        {
            //ActivateDialogue();
            //GameObject.Find("ContinueText").GetComponent<TextBlinker>().Awake();
            dialogueReactivated = true;
        }*/
    }


    public void StartDialogue(Dialogue dialogue)
    {
        m_DialogueActivated = true;
        m_DialogueBoxAnim.SetBool("PanelOpen", true);
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

        Debug.Log(sentences.Count);

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        for (int i = 0; i < m_SceneIndex.Length; i++)
        {
            if (sentences.Count == m_SceneIndex[i])
            {
                Debug.Log(sentences.Count.ToString());
                m_SceneAnim.SetBool(sentences.Count.ToString(), true);
            }
        }

        if (sentences.Count == 26)
        {
            StartCoroutine(ShutOffContinueButton(1f));
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;


        //StopAllCoroutines();
        if (m_TypeSentenceCo != null)
            StopCoroutine(m_TypeSentenceCo);

        m_DialogueBoxAnim.SetBool("ArrowBlink", false);

        m_TypeSentenceCo = StartCoroutine(TypeSentence(sentence));




    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        if (m_DialogueActivated == true)
            m_DialogueBoxAnim.SetBool("ArrowBlink", true);
    }

    void EndDialogue()
    {
        m_PlayerMov.m_IsMovementAllowed = true;
        DeactivateDialogue();

    }

    void DeactivateDialogue()
    {

        //if (Input.GetButtonDown("Fire1") && m_DialogueActivated)
        // {
        m_DialogueActivated = false;
        m_ContinueButton.SetActive(false);
        m_DialogueBoxAnim.SetBool("PanelOpen", false);
        m_DialogueBoxAnim.SetBool("ArrowBlink", false);
        //}
        //GameObject.Find("ContinueButton").gameObject.SetActive(false);

    }

    void ActivateDialogue()
    {
        //GameObject.Find("DialogueBox").transform.Find("ContinueButton").gameObject.SetActive(true);
        //if (Input.GetButtonDown("Fire1") && !m_DialogueActivated)
        //{
        m_DialogueActivated = true;
        m_ContinueButton.SetActive(true);
        m_DialogueBoxAnim.SetBool("PanelOpen", true);
        m_DialogueBoxAnim.SetBool("ArrowBlink", true);
        //}
    }

    private IEnumerator ShutOffContinueButton(float delay)
    {
        m_DialogueActivated = false;
        m_ContinueButton.SetActive(false);
        yield return new WaitForSeconds(delay);
        m_DialogueActivated = true;
        m_ContinueButton.SetActive(true);
        m_DialogueBoxAnim.SetBool("ArrowBlink", true);
    }

}
