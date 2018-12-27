using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpit : MonoBehaviour
{
    private Animator m_Anim;
    private PlayerStats stats;
    private AudioManager audioManager;

    [SerializeField] private ParticleSystem[] m_SpitEffects;


    public enum SpitParticle { FIRE, BUBBLE };
    [SerializeField]  private SpitParticle m_SpitStatus = SpitParticle.FIRE;
    private string[] m_SoundNames = { "Fire", "Bubble" };
    private SpitParticle m_LastSpitStatus = SpitParticle.FIRE;
    private bool m_isSpitting = false;

    public delegate void OnVariableChangeDelegate(SpitParticle spitmode);
    public static event OnVariableChangeDelegate OnSpitModeChange;

    public SpitParticle SpitStatus
    {
        get
        { return m_SpitStatus;}

        set
        {
            if (m_SpitStatus == value) return;
            m_SpitStatus = value;
            if (OnSpitModeChange != null)
                OnSpitModeChange(m_SpitStatus);
            }
    }

    // Use this for initialization
    void Start()
    {
        stats = PlayerStats.Instance;

        m_Anim = this.GetComponent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");

        if (OnSpitModeChange != null)
            OnSpitModeChange(m_SpitStatus);

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audioManager found in " + this.name);
        }

    }
    

    // Update is called once per frame
    void Update()
    {

        bool swimming = GetComponentInParent<CharacterController2D>().Swimming;
        bool swim = GetComponentInParent<CharacterController2D>().Swim;
        bool grounded = GetComponentInParent<CharacterController2D>().Grounded;
        bool roll = GetComponentInParent<CharacterController2D>().Rolling;
        bool climb = GetComponentInParent<CharacterController2D>().Climbing;

        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        float swimOffset = 0f;

        if (this.transform.parent.transform.localScale.x == -1)
            swimOffset = 180f;
        else
            swimOffset = 0f;

        m_SpitEffects[(int)SpitStatus].transform.rotation = Quaternion.Euler(0f, 0f, rotZ + swimOffset);


        SpitStatus = (swim ? SpitParticle.BUBBLE : SpitParticle.FIRE);
        if (SpitStatus != m_LastSpitStatus)
            StopSpit();
        m_LastSpitStatus = SpitStatus;


        switch (SpitStatus)
        {
            case SpitParticle.FIRE:

                if (!roll && !climb && !swim)
                {
                    
                    if (Input.GetButton("Fire1") && (stats.CurrentOxygen > 10))
                    {
                        StartSpit();

                    }
                    else if ( (!Input.GetButton("Fire1")) || (stats.CurrentOxygen <= 0) )
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
                    if (Input.GetButton("Fire1") && (stats.CurrentOxygen > 10))
                    {
                        StartSpit();
                    }
                    else if ((!Input.GetButton("Fire1")) || (stats.CurrentOxygen <= 0))
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
            m_SpitEffects[(int)SpitStatus].Play();
            audioManager.PlaySound(m_SoundNames[(int)SpitStatus]);
            m_isSpitting = true;
        }


    }

    public void StopSpit()
    {
        m_Anim.SetBool("MouthOpen", false);

        for (int i = 0; i < m_SpitEffects.Length; i++)
        {
            m_SpitEffects[i].Stop();
            audioManager.StopSound(m_SoundNames[i]);

        }

        m_isSpitting = false;

    }



}
