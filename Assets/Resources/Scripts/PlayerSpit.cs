﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSpit : MonoBehaviour
{
    private Animator m_Anim;
    private PlayerStats stats;
    private AudioManager audioManager;

    [SerializeField] private ParticleSystem[] m_SpitEffects;


    public enum SpitParticle { FIRE, BUBBLE };
    [SerializeField] private SpitParticle m_SpitStatus = SpitParticle.FIRE;
    private string[] m_SoundNames = { "Fire", "Bubble" };
    private SpitParticle m_LastSpitStatus = SpitParticle.FIRE;
    private bool m_isSpitting = false;
    [SerializeField] private bool m_isSpittingAllowed = true;

    private const float EPSILON = 0.01f;

    public delegate void OnVariableChangeDelegate(SpitParticle spitmode);
    public static event OnVariableChangeDelegate OnSpitModeChange;

    public delegate void OnSpitActivationDelegate(bool state);
    public static event OnSpitActivationDelegate OnSpitActivationChange;

    public SpitParticle SpitStatus
    {
        get
        { return m_SpitStatus; }

        set
        {
            if (m_SpitStatus == value) return;
            m_SpitStatus = value;
            if (OnSpitModeChange != null)
                OnSpitModeChange(m_SpitStatus);
            if (m_SpitStatus == SpitParticle.BUBBLE)
            {
                stats.m_MaxOxygen = stats.m_MaxBreatheCapa;
                stats.CurrentOxygen = Mathf.RoundToInt(stats.CurrentOxygen * stats.m_MaxBreatheCapa / stats.m_MaxSpitCapa);
            }

            else if (m_SpitStatus == SpitParticle.FIRE)
            {
                stats.m_MaxOxygen = stats.m_MaxSpitCapa;
                stats.CurrentOxygen = Mathf.RoundToInt(stats.CurrentOxygen * stats.m_MaxSpitCapa / stats.m_MaxBreatheCapa);
            }

        }
    }

    public bool IsSpittingAllowed
    {
        get
        {
            return m_isSpittingAllowed;
        }

        set
        {
            if (m_isSpittingAllowed == value) return;
            if (value == false && m_isSpitting == true)
                StopSpit();
            if (OnSpitActivationChange != null)
                OnSpitActivationChange(value);
            m_isSpittingAllowed = value;
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
        if (IsSpittingAllowed == false) return;

        bool swimming = GetComponentInParent<CharacterController2D>().Swimming;
        bool swim = GetComponentInParent<CharacterController2D>().Swim;
        bool grounded = GetComponentInParent<CharacterController2D>().Grounded;
        bool roll = GetComponentInParent<CharacterController2D>().Rolling;
        bool climb = GetComponentInParent<CharacterController2D>().Climbing;

        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        float swimOffset = 0f;

        if (System.Math.Abs(this.transform.parent.transform.localScale.x - -1) < EPSILON)
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
                        // Check if the mouse was clicked over a UI element
                        if (!EventSystem.current.IsPointerOverGameObject())
                        {
                            StartSpit();
                        }
                    }
                    else if ((!Input.GetButton("Fire1")) || (stats.CurrentOxygen <= 0))
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
            if (Time.timeScale > 0f)
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

    private void OnDestroy()
    {
        StopSpit();
    }



}
