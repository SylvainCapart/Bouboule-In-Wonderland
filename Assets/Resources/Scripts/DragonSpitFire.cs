using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSpitFire : MonoBehaviour
{

    private ParticleSystem m_FireSource;
    private EnemyAI m_EnemyAI;
    private AudioManager m_AudioManager;
    [SerializeField] private float m_FireTriggerDistance = 1.5f;
    private bool m_TargetFireClose;

    public bool TargetClose
    {
        get
        {
            return m_TargetFireClose;
        }

        set
        {
            if (m_TargetFireClose == value) return;
            m_TargetFireClose = value;
            SpitFire(m_TargetFireClose);
        }
    }

    private void Awake()
    {
        if (m_EnemyAI == null)
            m_EnemyAI = GetComponent<EnemyAI>();

        if (m_FireSource == null)
        {
            ParticleSystem[] parts = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].tag == "FireSource" || parts[i].tag == "FireSourceGreen")
                {
                    m_FireSource = parts[i];
                    break;
                }

            }
        }
    }

    private void Start()
    {
        m_AudioManager = AudioManager.instance;
        if (m_AudioManager == null)
        {
            Debug.LogError("No audioManager found in " + this.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_EnemyAI.State == EnemyAI.EnemyState.TARGET)
        {
            if (m_EnemyAI.GetTarget.targettransform != null)
                TargetClose = Vector3.Distance(transform.position, m_EnemyAI.GetTarget.targettransform.position) <= m_FireTriggerDistance;

            if (m_EnemyAI.GetTarget.targettransform != null && m_FireSource.isPlaying)
            {
                Vector3 difference = m_EnemyAI.GetTarget.targettransform.position - transform.position;
                difference.Normalize();
                float rotX = -Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

                m_FireSource.transform.rotation = Quaternion.Euler(rotX, 90, 0);
            }
            else if (m_EnemyAI.GetTarget.targettransform == null && m_FireSource.isPlaying)
            {
                SpitFire(false);
            }
        }

        if (m_EnemyAI.State == EnemyAI.EnemyState.GIVEUP)
            TargetClose = false;
    }

    private void OnDestroy()
    {
        SpitFire(false);
    }

    private void SpitFire(bool fire)
    {
        if (fire != false)
        {
            m_FireSource.Play();
            if (m_AudioManager != null)
            {
                if (!m_AudioManager.IsSoundPlayed("DragonFire"))
                    m_AudioManager.PlaySound("DragonFire");
            }
        }
        else
        {
            m_FireSource.Stop();
            if (m_AudioManager != null)
                m_AudioManager.StopSound("DragonFire");
        }

    }
}
