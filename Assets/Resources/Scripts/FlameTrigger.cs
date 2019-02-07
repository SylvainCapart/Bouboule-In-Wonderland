using System.Collections.Generic;
using UnityEngine;

public class FlameTrigger : MonoBehaviour
{
    public bool m_FireTriggered;
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    [SerializeField] private GameObject[] m_ObjectToEnable;
    [SerializeField] private Collider2D m_SausageCollider;
    private DialogueMgt m_DialogueMgt;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];

        if (m_ObjectToEnable.Length == 0)
        {
            Destroy(this);
        }
        m_DialogueMgt = DialogueMgt.instance;

    }

    private void OnParticleTrigger()
    {
        if (m_FireTriggered) Destroy(this);

        int count = ps.GetParticles(particles);
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = particles[i].position;
            if (m_SausageCollider != null)
            {
                if (m_SausageCollider.OverlapPoint(pos))
                {
                    if (m_FireTriggered) return;


                    for (int j = 0; j < m_ObjectToEnable.Length; j++)
                    {
                        if (m_ObjectToEnable[j] != null)
                            m_ObjectToEnable[j].SetActive(true);
                    }

                    if (m_DialogueMgt != null)
                    {
                        m_DialogueMgt.DisplayNextSentence();
                        m_DialogueMgt.ShutOffContinueButton();
                    }


                    m_FireTriggered = true;
                    this.enabled = false;
                }
            }
        }
    }
}
