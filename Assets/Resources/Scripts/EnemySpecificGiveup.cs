using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemySpecificGiveup : MonoBehaviour
{
    private EnemyAI m_EnemyAI;
    private bool m_IsPlayerSwimming;
    private bool m_UndetectIfPlayerSwimIs;

    private void Start()
    {
        switch (transform.tag)
        {
            case "Fish":
                m_UndetectIfPlayerSwimIs = false;
                break;
            case "Dragon":
                m_UndetectIfPlayerSwimIs = true;
                break;
            default:
                break;
        }
    }

    private void Awake()
    {
        if (m_EnemyAI == null)
            m_EnemyAI = GetComponentInParent<EnemyAI>();

        CharacterController2D.OnSwimChangeRaw += OnPlayerSwim;
    }

    private void OnDestroy()
    {
        CharacterController2D.OnSwimChangeRaw -= OnPlayerSwim;
    }

    private void Update()
    {
        if (m_EnemyAI.State == EnemyAI.EnemyState.TARGET)
        {
            if (IsSpecificGiveup())
                m_EnemyAI.SetTargetState(m_EnemyAI.DetectionTransform, 0, EnemyAI.EnemyState.GIVEUP);

            if (m_IsPlayerSwimming == m_UndetectIfPlayerSwimIs)
                m_EnemyAI.SetTargetState(m_EnemyAI.DetectionTransform, 0, EnemyAI.EnemyState.GIVEUP);
        }
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
                            specificCondition = (m_IsPlayerSwimming == m_UndetectIfPlayerSwimIs);

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
                            specificCondition = (m_IsPlayerSwimming == m_UndetectIfPlayerSwimIs);
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
                            specificCondition = (m_IsPlayerSwimming == m_UndetectIfPlayerSwimIs);

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
                            specificCondition = (m_IsPlayerSwimming == m_UndetectIfPlayerSwimIs);
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

    private void OnPlayerSwim(bool state)
    {
        m_IsPlayerSwimming = state;
    }


}
