using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public Dialogue dialogue;
    private DialogueMgt m_DialogueMgt;

    public void TriggerDialogue()
    {
        m_DialogueMgt.StartDialogue(dialogue);
    }

    public void Start()
    {
        m_DialogueMgt = DialogueMgt.instance;
    }

    private IEnumerator AutoTriggerDialogue()
    {
        yield return (new WaitForSeconds(1f));
        m_DialogueMgt.StartDialogue(dialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!dialogue.triggered)
            {
                dialogue.triggered = true;
                TriggerDialogue();
            }
        }

    }
}
