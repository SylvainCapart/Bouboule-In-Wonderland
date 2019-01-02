
using UnityEngine;

public class DragonDetection : MonoBehaviour
{
    [SerializeField] private DragonMgt m_DragonMgt;
    [SerializeField] private DragonMgt.TargetData[] m_TargetsArray;
    [SerializeField] private LayerMask m_WhatIsTarget;

    private void Awake()
    {
        if (m_DragonMgt == null)
            m_DragonMgt = GetComponentInParent<DragonMgt>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_DragonMgt.State != DragonMgt.DragonState.TARGET && m_DragonMgt.State != DragonMgt.DragonState.GIVEUP)
        {
            if (m_TargetsArray.Length >= 1)
            {
                for (int i = 0; i < m_TargetsArray.Length; i++)
                {
                    if (collision.tag == m_TargetsArray[i].targettag && collision.name == m_TargetsArray[i].targetname)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, collision.gameObject.transform.position - this.transform.position, 50f, m_WhatIsTarget);

                        if (m_DragonMgt.GetTarget != null)
                        {
                            if (m_TargetsArray[i].priority >= m_DragonMgt.GetTarget.targetpriority && hit.collider != null && hit.collider.gameObject.name == collision.name)
                            {
                                if (hit.collider.gameObject.name == "Player" && m_DragonMgt.m_IsPlayerSwimming) return;

                                m_DragonMgt.SetTargetState(collision.transform, m_TargetsArray[i].priority, DragonMgt.DragonState.TARGET);
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (m_DragonMgt.State != DragonMgt.DragonState.TARGET && m_DragonMgt.State != DragonMgt.DragonState.GIVEUP)
        {
            if (m_TargetsArray.Length >= 1)
            {
                for (int i = 0; i < m_TargetsArray.Length; i++)
                {
                    if (collision.tag == m_TargetsArray[i].targettag && collision.name == m_TargetsArray[i].targetname)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, collision.gameObject.transform.position - this.transform.position, 50f, m_WhatIsTarget);

                        if (m_DragonMgt.GetTarget != null)
                        {
                            if (m_TargetsArray[i].priority >= m_DragonMgt.GetTarget.targetpriority && hit.collider != null && hit.collider.gameObject.name == collision.name)
                            {
                                if (hit.collider.gameObject.name == "Player" && m_DragonMgt.m_IsPlayerSwimming) return;

                                m_DragonMgt.SetTargetState(collision.transform, m_TargetsArray[i].priority, DragonMgt.DragonState.TARGET);
                            }
                        }
                    }
                }
            }
        }
    }

   
}
