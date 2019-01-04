using UnityEngine;

public class EnemySpecificGiveup : MonoBehaviour
{
    [SerializeField] private EnemyAI m_EnemyAI;

    private void Awake()
    {
        if (m_EnemyAI == null)
            m_EnemyAI = GetComponentInParent<EnemyAI>();
    }

    public bool IsSpecificGiveup()
    {
        bool specificCondition = false;

        if (m_EnemyAI.GetTarget != null)
        {
            switch (transform.tag)
            {
                case "Fish":
                    switch (m_EnemyAI.GetTarget.targettransform.tag)
                    {
                        case "Player":
                            specificCondition = (m_EnemyAI.m_IsPlayerSwimming == m_EnemyAI.m_UndetectIfPlayerSwimIs);

                            if (m_EnemyAI.GetTarget.targettransform.GetComponent<AlgaHide>() != null)
                            {
                                specificCondition = m_EnemyAI.GetTarget.targettransform.GetComponent<AlgaHide>().m_Hidden;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case "Dragon":
                    switch (m_EnemyAI.GetTarget.targettransform.tag)
                    {
                        case "Player":
                            specificCondition = (m_EnemyAI.m_IsPlayerSwimming == m_EnemyAI.m_UndetectIfPlayerSwimIs);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

        }
        return specificCondition;
    }

    public bool IsSpecificUndetect(GameObject obj)
    {
        bool specificCondition = false;

        if (m_EnemyAI.GetTarget != null)
        {
            switch (transform.tag)
            {
                case "Fish":
                    switch (obj.tag)
                    {
                        case "Player":
                            specificCondition = (m_EnemyAI.m_IsPlayerSwimming == m_EnemyAI.m_UndetectIfPlayerSwimIs);

                            if (obj.GetComponent<AlgaHide>() != null)
                            {
                                specificCondition = obj.GetComponent<AlgaHide>().m_Hidden;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case "Dragon":
                    switch (obj.tag)
                    {
                        case "Player":
                            specificCondition = (m_EnemyAI.m_IsPlayerSwimming == m_EnemyAI.m_UndetectIfPlayerSwimIs);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

        }
        return specificCondition;
    }
}
