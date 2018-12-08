using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpit : MonoBehaviour
{
    private Animator m_Anim;

    //[SerializeField] private ParticleSystem m_MouthFireEffect;
    //[SerializeField] private ParticleSystem m_mouthBubbleEffect;
    [SerializeField] private ParticleSystem[] m_SpitEffects;

    [SerializeField] private const string mouthFireStr = "MouthFire";

    private float lastDirection = 1f;

    //[SerializeField] private bool m_isSpittingFire = false;
    //[SerializeField] private bool m_isSpittingBubble = false;

    enum SpitParticle { FIRE, BUBBLE };
    private SpitParticle m_SpitStatus = SpitParticle.FIRE;
    private SpitParticle m_LastSpitStatus = SpitParticle.FIRE;

    // Use this for initialization
    void Start()
    {
        m_Anim = this.GetComponent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");

    }

    // Update is called once per frame
    void Update()
    {

        bool swimming = GetComponentInParent<CharacterController2D>().m_Swimming;
        bool swim = GetComponentInParent<CharacterController2D>().m_Swim;
        bool grounded = GetComponentInParent<CharacterController2D>().Grounded;
        bool roll = GetComponentInParent<CharacterController2D>().m_Rolling;
        bool climb = GetComponentInParent<CharacterController2D>().m_Climbing;

        // ensure that fire is following the player direction
        if (Input.GetAxisRaw("Horizontal") != 0f && Input.GetAxisRaw("Horizontal") != lastDirection)
        {
            m_SpitEffects[(int)m_SpitStatus].transform.Rotate(new Vector3(180, 0, 0));

            lastDirection = Input.GetAxisRaw("Horizontal");
        }

        m_SpitStatus = (swim ? SpitParticle.BUBBLE : SpitParticle.FIRE);
        if (m_SpitStatus != m_LastSpitStatus)
            StopSpit();
        m_LastSpitStatus = m_SpitStatus;


        switch (m_SpitStatus)
        {
            case SpitParticle.FIRE:

                if (!roll && !climb && !swim)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        StartSpit();
                    }
                    else if (Input.GetButtonUp("Fire1"))
                    {
                        StopSpit();
                    }
                }
                else //if (climb || roll)
                {
                    StopSpit();
                }
                break;
            case SpitParticle.BUBBLE:
                if ((swim && !swimming) || (swim && swimming && grounded))
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        StartSpit();
                    }
                    else if (Input.GetButtonUp("Fire1"))
                    {
                        StopSpit();
                    }
                }
                else if (!swim )//|| swimming)
                {
                    StopSpit();
                }
                break;
            default:
                Debug.LogError(this.name + "Spit Particle Status not found");
                break;
        }



    }


    public void StartSpit()
    {
        m_Anim.SetBool("MouthOpen", true);
        /*for (int i = 0; i < m_SpitEffects.Length; i++)
        {
            m_SpitEffects[i].Stop();
        }*/
        m_SpitEffects[(int)m_SpitStatus].Play();

    }

    public void StopSpit()
    {
        m_Anim.SetBool("MouthOpen", false);

        for (int i = 0; i < m_SpitEffects.Length; i++)
        {
            m_SpitEffects[i].Stop();
        }

        /*
        switch (m_SpitStatus)
        {
            case SpitParticle.FIRE:
                m_MouthFireEffect.Stop();
                m_isSpittingFire = false;
                break;
            case SpitParticle.BUBBLE:
                m_mouthBubbleEffect.Stop();
                m_isSpittingBubble = false;
                break;
            default:
                Debug.LogError(this.name + "Spit Particle Status not found");
                break;
        }*/



    }



}
