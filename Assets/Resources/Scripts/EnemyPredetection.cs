using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPredetection : MonoBehaviour
{
    [SerializeField] private EnemyAI m_EnemyAI;

    private void Awake()
    {
        if (m_EnemyAI == null)
            m_EnemyAI = GetComponentInParent<EnemyAI>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_EnemyAI.State != EnemyAI.EnemyState.TARGET && m_EnemyAI.State != EnemyAI.EnemyState.GIVEUP && m_EnemyAI.State != EnemyAI.EnemyState.SURPRISED)
        {
            if (m_EnemyAI.m_TargetsArray.Length >= 1)
            {
                for (int i = 0; i < m_EnemyAI.m_TargetsArray.Length; i++)
                {
                    if (collision.tag == m_EnemyAI.m_TargetsArray[i].targettag && collision.name == m_EnemyAI.m_TargetsArray[i].targetname)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, collision.gameObject.transform.position - this.transform.position, 50f, m_EnemyAI.m_WhatIsTarget);
                        Debug.DrawLine(this.transform.position, collision.gameObject.transform.position);
                        if (m_EnemyAI.GetTarget != null)
                        {
                            if (m_EnemyAI.m_TargetsArray[i].priority >= m_EnemyAI.GetTarget.targetpriority && hit.collider != null && hit.collider.gameObject.name == collision.name)
                            {
                                if (m_EnemyAI.m_SpecificGiveup != null)
                                {
                                    if (m_EnemyAI.m_SpecificGiveup.IsSpecificUndetect(collision.gameObject))
                                        return;
                                }

                                m_EnemyAI.SetTargetState(collision.transform, m_EnemyAI.m_TargetsArray[i].priority, EnemyAI.EnemyState.SURPRISED);
                            }
                        }
                    }
                }
            }
        }
    }
}
