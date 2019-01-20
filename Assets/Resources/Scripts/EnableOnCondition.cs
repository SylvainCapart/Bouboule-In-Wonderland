using UnityEngine;

public class EnableOnCondition : MonoBehaviour
{
    public enum EnableCondition { DESTROY };
    [SerializeField] private EnableCondition m_Condition;
    [SerializeField] private GameObject[] m_ConditionObjects;
    [SerializeField] private GameObject[] m_ToEnableObjects;

    // Update is called once per frame
    void Update()
    {
        switch (m_Condition)
        {
            case EnableCondition.DESTROY:
                bool condfilled = true;
                for (int i = 0; i < m_ConditionObjects.Length; i++)
                {
                    if (m_ConditionObjects[i] != null)
                    {
                        condfilled = false;
                    }
                }
                if (condfilled == true)
                {
                    Enable();
                }

                break;
            default:
                break;
        } 
    }

    private void Enable()
    {
        for (int i = 0; i < m_ToEnableObjects.Length; i++)
        {
            m_ToEnableObjects[i].SetActive(true);
        }
    }

}
