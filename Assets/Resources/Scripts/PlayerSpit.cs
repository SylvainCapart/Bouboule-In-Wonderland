using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpit : MonoBehaviour
{
    private Animator m_Anim;
    private PlayerStats stats;

    [SerializeField] private ParticleSystem[] m_SpitEffects;

    private float lastDirection = 1f;

    enum SpitParticle { FIRE, BUBBLE };
    private SpitParticle m_SpitStatus = SpitParticle.FIRE;
    private SpitParticle m_LastSpitStatus = SpitParticle.FIRE;
    private bool m_isSpitting = false;

    // Use this for initialization
    void Start()
    {
        stats = PlayerStats.instance;

        m_Anim = this.GetComponent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");

    }

    private void FixedUpdate()
    {
        bool m_button = Input.GetButton("Fire1");
    }

    // Update is called once per frame
    void Update()
    {

        bool swimming = GetComponentInParent<CharacterController2D>().m_Swimming;
        bool swim = GetComponentInParent<CharacterController2D>().m_Swim;
        bool grounded = GetComponentInParent<CharacterController2D>().Grounded;
        bool roll = GetComponentInParent<CharacterController2D>().m_Rolling;
        bool climb = GetComponentInParent<CharacterController2D>().m_Climbing;

        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        float swimOffset = 0f;

        if (this.transform.parent.transform.localScale.x == -1)
            swimOffset = 180f;
        else
            swimOffset = 0f;

        m_SpitEffects[(int)m_SpitStatus].transform.rotation = Quaternion.Euler(0f, 0f, rotZ + swimOffset);


        m_SpitStatus = (swim ? SpitParticle.BUBBLE : SpitParticle.FIRE);
        if (m_SpitStatus != m_LastSpitStatus)
            StopSpit();
        m_LastSpitStatus = m_SpitStatus;


        switch (m_SpitStatus)
        {
            case SpitParticle.FIRE:

                if (!roll && !climb && !swim && (stats.CurrentOxygen > 0))
                {
                    if (Input.GetButton("Fire1"))
                    {
                        StartSpit();
                    }
                    else if (!Input.GetButton("Fire1"))
                    {
                        StopSpit();
                    }
                }
                else
                {
                    StopSpit();
                }
                break;
            case SpitParticle.BUBBLE:
                if ((swim && !swimming) || (swim && swimming && grounded))
                {
                    if (Input.GetButton("Fire1"))
                    {
                        StartSpit();
                    }
                    else if (!Input.GetButton("Fire1"))
                    {
                        StopSpit();
                    }
                }
                else if (!swim || swimming)
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
        if (!m_isSpitting)
        {
            m_SpitEffects[(int)m_SpitStatus].Play();
            m_isSpitting = true;
        }


    }

    public void StopSpit()
    {
        m_Anim.SetBool("MouthOpen", false);

        for (int i = 0; i < m_SpitEffects.Length; i++)
        {
            m_SpitEffects[i].Stop();
        }
        m_isSpitting = false;

    }



}
