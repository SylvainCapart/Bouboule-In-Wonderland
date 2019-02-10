using System.Collections.Generic;
using UnityEngine;

public class FlameTrigger : MonoBehaviour
{
    public bool m_FireTriggered;
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private ParticleSystem m_SausageBurn;
    public Collider2D m_SausageCollider;
    private GameObject m_SorcereRightHand;
    public SpriteRenderer m_SausageImg;
    private DialogueMgt m_DialogueMgt;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];

        if (GameMaster.gm.m_IntroSceneEnded)
            Destroy(this);

        m_SorcereRightHand = GameObject.FindGameObjectWithTag("SorcererRightHand");
        if (m_SorcereRightHand == null)
        {
            Destroy(this);
            return;
        }


        m_SausageCollider = m_SorcereRightHand.GetComponentInChildren<CapsuleCollider2D>();
        if (m_SausageCollider == null)
        {
            Destroy(this);
        }
        m_SausageBurn = m_SorcereRightHand.GetComponentInChildren<ParticleSystem>();
        m_SausageImg = m_SorcereRightHand.GetComponentInChildren<SpriteRenderer>();
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

                    if (m_SausageBurn != null)
                        m_SausageBurn.Play();


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
