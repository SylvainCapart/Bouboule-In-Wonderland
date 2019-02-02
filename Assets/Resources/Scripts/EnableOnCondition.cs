using UnityEngine;

public class EnableOnCondition : MonoBehaviour
{
    public enum EnableCondition { DESTROY };
    [SerializeField] private EnableCondition m_Condition;
    [SerializeField] private GameObject[] m_ConditionObjects;
    [SerializeField] private GameObject[] m_ToEnableObjects;
    [SerializeField] private GameObject[] m_ToDisableObjects;
    private bool m_ConditionFilled;

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
                        Enable();
                        Disable();
                    }

                    break;
                default:
                    break;
            }
        }
    }

    private void Enable()
    {
        for (int i = 0; i < m_ToEnableObjects.Length; i++)
        {
            m_ToEnableObjects[i].SetActive(true);
        }
    }

    private void Disable()
    {
        for (int i = 0; i < m_ToDisableObjects.Length; i++)
        {
            m_ToDisableObjects[i].SetActive(false);
        }
    }

}
