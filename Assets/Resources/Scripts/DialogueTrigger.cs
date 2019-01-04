using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueMgt>().StartDialogue(dialogue);
    }

    public void Start()
    {
        StartCoroutine(AutoTriggerDialogue());
    }

    private IEnumerator AutoTriggerDialogue()
    {
        yield return (new WaitForSeconds(1f));
        FindObjectOfType<DialogueMgt>().StartDialogue(dialogue);
    }

}
