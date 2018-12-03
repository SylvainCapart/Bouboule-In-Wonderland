using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireThrow : MonoBehaviour {
    private Animator m_Anim;
    private ParticleSystem m_mouthFireEffect;
    [SerializeField] private const string mouthFireStr = "MouthFire";
    private float lastDirection = 1f;
    public bool m_isSpittingFire = false;


    // Use this for initialization
    void Start () {
        m_Anim = this.GetComponentInParent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");

        Component[] children = GetComponentsInChildren<ParticleSystem>();
 
        foreach (ParticleSystem childParticleSystem in children)
        {
            if (childParticleSystem.name == mouthFireStr)
            {
                m_mouthFireEffect = childParticleSystem;
            }
        }
        if (m_mouthFireEffect == null)
            Debug.LogError("Mouthfire effect not found");
        //Vector3 rotationVector = new Vector3(0, 180, 0);
        
    }
	
	// Update is called once per frame
	void Update () {
        // ensure that fire is following the player direction
        if (Input.GetAxisRaw("Horizontal") != 0f && Input.GetAxisRaw("Horizontal") != lastDirection)
        {
            m_mouthFireEffect.transform.Rotate(new Vector3(0, 90, 0));
            lastDirection = Input.GetAxisRaw("Horizontal");
        }

        // only enable to spit fire if player is not rolling
        if (!GetComponentInParent<CharacterController2D>().m_Rolling && !GetComponentInParent<CharacterController2D>().m_Climbing && !GetComponentInParent<CharacterController2D>().m_Swim)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                StartSpitFire();
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                StopSpitFire();
            }
        }

    }

    public void StartSpitFire()
    {
        m_Anim.SetBool("MouthOpen", true);
        m_mouthFireEffect.Play();
        m_isSpittingFire = true;
    }

    public void StopSpitFire()
    {
        m_Anim.SetBool("MouthOpen", false);
        m_mouthFireEffect.Stop();
        m_isSpittingFire = false;
    }

}
