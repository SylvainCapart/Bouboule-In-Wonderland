using UnityEngine;

public class SimpleSetActive : MonoBehaviour
{
    enum ActionCondition { ONENTER, ONEXIT};
    [SerializeField] private ActionCondition m_ActionCondition;
    [SerializeField] private GameObject[] m_Objects;
    [SerializeField] private bool m_ActiveState;
    private bool m_IsActivated = false;
    [SerializeField] private bool m_ShakeCamera = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( !m_IsActivated && m_ActionCondition == ActionCondition.ONENTER && collision.tag == "Player")
        {
            for (int i = 0; i < m_Objects.Length; i++)
            {
                m_Objects[i].SetActive(m_ActiveState);
            }
            if (m_ShakeCamera)
                FindObjectOfType<CameraShake>().Shake(0.2f, 0.2f);

            m_IsActivated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ( !m_IsActivated && m_ActionCondition == ActionCondition.ONEXIT && collision.tag == "Player")
        {
            for (int i = 0; i < m_Objects.Length; i++)
            {
                m_Objects[i].SetActive(m_ActiveState);
            }
            if (m_ShakeCamera)
                FindObjectOfType<CameraShake>().Shake(0.2f, 0.2f);

            m_IsActivated = true;
        }
    }
}
