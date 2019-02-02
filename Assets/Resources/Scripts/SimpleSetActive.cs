using UnityEngine;

public class SimpleSetActive : MonoBehaviour
{
    [SerializeField] private GameObject[] m_Objects;
    [SerializeField] private bool m_ActiveState;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < m_Objects.Length; i++)
        {
            m_Objects[i].SetActive(m_ActiveState);
        }
    }
}
