using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemySpecificGiveup : MonoBehaviour
{
    private EnemyAI m_EnemyAI;
    private bool m_IsPlayerSwimming;
    private bool m_NoWaterDetection;

    private void Start()
    {
        switch (transform.tag)
        {
            case "Fish":
                m_NoWaterDetection = false;
                break;
            case "Dragon":
                m_NoWaterDetection = true;
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
            if (IsSpecificUndetect(m_EnemyAI.GetTarget.targettransform.gameObject))
                m_EnemyAI.SetTargetState(m_EnemyAI.DetectionTransform, 0, EnemyAI.EnemyState.GIVEUP);
        }
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
                            specificCondition = (m_IsPlayerSwimming == m_NoWaterDetection);

                            if (!specificCondition && obj.GetComponent<AlgaHide>() != null)
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
                            specificCondition = (m_IsPlayerSwimming == m_NoWaterDetection);
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
