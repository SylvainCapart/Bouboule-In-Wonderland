using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpen : MonoBehaviour {

    private const string m_openTrigTag = "Player";
    private Animator m_Anim;


    // Use this for initialization
    void Start () {
        m_Anim = this.GetComponentInParent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");

    }
	
	// Update is called once per frame
	void Update () {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.gameObject.tag == m_openTrigTag)
            m_Anim.SetBool("OpenChest", true);
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == m_openTrigTag)
            m_Anim.SetBool("OpenChest", false);
        
    }
    
}
