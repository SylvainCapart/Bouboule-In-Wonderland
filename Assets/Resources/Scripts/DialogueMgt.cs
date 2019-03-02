using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMgt : MonoBehaviour {

    public static DialogueMgt instance;

    private Queue<string> sentences;
    private Dialogue m_CurrentDialogue;
    [Header("Optional : ")] public Text nameText;
    public Text dialogueText;
    [SerializeField] private Animator m_DialogueBoxAnim;
    [SerializeField] private Animator m_SceneAnim;
    private bool m_DialogueActivated;
    [SerializeField] private GameObject m_ContinueButton;

    private Coroutine m_TypeSentenceCo;
     public bool m_IsSentencesQueueEmpty;

    public delegate void DialogueMgtDeleguate();
    public static event DialogueMgtDeleguate OnEmptyQueue;


    // Use this for initialization
    void Start () 
    {
        sentences = new Queue<string>();

        if (m_ContinueButton == null)
            Debug.LogError(name + " : continue button is missing");
    }

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (m_CurrentDialogue != null && sentences.Count >= 0 && m_ContinueButton.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DisplayNextSentence();
            }
        }
    }


    public void StartDialogue(Dialogue dialogue)
    {
        m_CurrentDialogue = dialogue;
        m_DialogueActivated = true;
        m_ContinueButton.SetActive(true);
        m_DialogueBoxAnim.SetBool("PanelOpen", true);

        if (m_CurrentDialogue.sceneManager != null)
            m_CurrentDialogue.sceneManager.SpecificStartDialogue(dialogue); 

        if (nameText != null)
            nameText.text = dialogue.name;

        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        m_IsSentencesQueueEmpty = false;
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (Time.timeScale < 0.01f)
            return;

        if (sentences.Count == 0)
        {
            m_IsSentencesQueueEmpty = true;
            if (OnEmptyQueue != null)
                OnEmptyQueue();
            EndDialogue();
            return;
        }

        StopAllCoroutines();
        for (int i = 0; i < m_CurrentDialogue.scenes.Length; i++)
        {
            if (sentences.Count == m_CurrentDialogue.scenes[i].sceneIndex)
            {

                if (m_CurrentDialogue.sceneManager != null)
                    StartCoroutine(m_CurrentDialogue.sceneManager.SpecificSceneAction(m_CurrentDialogue.scenes[i].sceneIndex));
                if (m_CurrentDialogue.scenes[i].sceneClip != null)
                {
                    m_SceneAnim.SetBool(sentences.Count.ToString(), true);
                }

            }
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;

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
        if (m_CurrentDialogue.sceneManager != null)
            m_CurrentDialogue.sceneManager.SpecificEndDialogue();
        DeactivateDialogue();

    }

    public void DeactivateDialogue()
    {
        m_DialogueActivated = false;
        m_ContinueButton.SetActive(false);
        m_DialogueBoxAnim.SetBool("PanelOpen", false);
        m_DialogueBoxAnim.SetBool("ArrowBlink", false);
    }

    public void ActivateDialogue()
    {
        m_DialogueActivated = true;
        m_ContinueButton.SetActive(true);
        m_DialogueBoxAnim.SetBool("PanelOpen", true);
        m_DialogueBoxAnim.SetBool("ArrowBlink", true);
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

    public void ShutOffContinueButton()
    {
        m_DialogueActivated = false;
        m_ContinueButton.SetActive(false);
        m_DialogueBoxAnim.SetBool("ArrowBlink", false);
    }


    public void ShutOnContinueButton()
    {
        m_DialogueActivated = true;
        m_ContinueButton.SetActive(true);
        m_DialogueBoxAnim.SetBool("ArrowBlink", true);
    }

}
