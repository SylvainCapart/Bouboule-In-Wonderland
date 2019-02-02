using UnityEngine;

public class FlipOnCondition : MonoBehaviour
{
    public enum EnableCondition { DESTROY };
    [SerializeField] private EnableCondition m_Condition;
    [SerializeField] private GameObject[] m_ConditionObjects;
    private bool m_ConditionFilled;
    private EnemyAI m_EnemyAI;

    private void Start()
    {
        m_EnemyAI = GetComponent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_ConditionFilled)
        {
            switch (m_Condition)
            {
                case EnableCondition.DESTROY:
                    m_ConditionFilled = true;
                    for (int i = 0; i < m_ConditionObjects.Length; i++)
                    {
                        if (m_ConditionObjects[i] != null)
                        {
                            m_ConditionFilled = false;
                        }
                    }
                    if (m_ConditionFilled == true)
                    {
                        transform.Rotate(new Vector3(0, 180, 0));
                        m_EnemyAI.m_FacingRight = !m_EnemyAI.m_FacingRight;
                    }
                    break;
                default:
                    break;
            }
        }
    }


    private void FlipRotate()
    {
        transform.Rotate(new Vector3(0, 180, 0));
    }

    private void FlipScale()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
