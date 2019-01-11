using System.Collections.Generic;
using UnityEngine;

public class FlameTrigger : MonoBehaviour
{
    public bool m_FireTriggered;
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    [SerializeField] private GameObject[] m_ObjectToEnable;
    [SerializeField] private Collider2D m_SausageCollider;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];

        if (m_ObjectToEnable.Length == 0)
        {
            Destroy(this);
        }

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
                    DialogueMgt dialogueMgt = FindObjectOfType<DialogueMgt>();
                    if (dialogueMgt != null)
                    {
                        dialogueMgt.DisplayNextSentence();
                        dialogueMgt.ShutOffContinueButton();
                    }


                    m_FireTriggered = true;
                    this.enabled = false;
                }
            }
        }


        //if (Physics2D.OverlapPoint(particles[i].position) == m_SausageCollider)
            //if (particles[i].position.y > (closesttilemap.GetComponent<CompositeCollider2D>().bounds.center.y + closesttilemap.GetComponent<CompositeCollider2D>().bounds.extents.y))
            
        /*
        Debug.LogError("HERE3");
        m_FireTriggered = true;
        for (int i = 0; i < m_ObjectToEnable.Length; i++)
        {
            if (m_ObjectToEnable[i] != null)
            m_ObjectToEnable[i].SetActive(true);
        }
        FindObjectOfType<DialogueMgt>().DisplayNextSentence();*/

    }


}
