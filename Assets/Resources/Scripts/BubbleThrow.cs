using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleThrow : MonoBehaviour {
    private Animator m_Anim;
    private ParticleSystem m_mouthBubbleEffect;
    [SerializeField] private const string mouthBubbleStr = "MouthBubble";
    private float lastDirection = 1f;
    public bool m_isSpittingBubble = false;


    // Use this for initialization
    void Start () {
        m_Anim = this.GetComponentInParent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");

        Component[] children = GetComponentsInChildren<ParticleSystem>();
 
        foreach (ParticleSystem childParticleSystem in children)
        {
            if (childParticleSystem.name == mouthBubbleStr)
            {
                m_mouthBubbleEffect = childParticleSystem;
            }
        }
        if (m_mouthBubbleEffect == null)
            Debug.LogError("Mouthfire effect not found");
        //Vector3 rotationVector = new Vector3(0, 180, 0);
        
    }
	
	// Update is called once per frame
	void Update () {
        // ensure that fire is following the player direction
        if (Input.GetAxisRaw("Horizontal") != 0f && Input.GetAxisRaw("Horizontal") != lastDirection)
        {
            m_mouthBubbleEffect.transform.Rotate(new Vector3(0, 90, 0));
            lastDirection = Input.GetAxisRaw("Horizontal");
        }

        // only enable to spit fire if player is not rolling
        if (!GetComponentInParent<CharacterController2D>().m_Swimming && GetComponentInParent<CharacterController2D>().m_Swim)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.LogError("HERE");
                StartSpitBubble();
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                StopSpitBubble();
            }
        }

    }

    public void StartSpitBubble()
    {
        m_Anim.SetBool("MouthOpen", true);
        m_mouthBubbleEffect.Play();
        m_isSpittingBubble = true;
    }

    public void StopSpitBubble()
    {
        m_Anim.SetBool("MouthOpen", false);
        m_mouthBubbleEffect.Stop();
        m_isSpittingBubble = false;
    }

}
