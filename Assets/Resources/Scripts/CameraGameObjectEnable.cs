using UnityEngine;

public class CameraGameObjectEnable : MonoBehaviour
{
    [SerializeField] private GameObject[] m_ActivableObjectsArray;
    private float m_ActivationDistance = 15f;
    private float m_DeactivationDistance = 15f;

    private void Awake()
    {
        for (int i = 0; i < m_ActivableObjectsArray.Length; i++)
        {
             m_ActivableObjectsArray[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < m_ActivableObjectsArray.Length; i++)
        {
            if (Vector2.Distance(m_ActivableObjectsArray[i].transform.position, Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.center)) 
            <= m_ActivationDistance)
            {
                if (!m_ActivableObjectsArray[i].activeSelf)
                    m_ActivableObjectsArray[i].SetActive(true);
            }
            else
            {
                if (m_ActivableObjectsArray[i].activeSelf)
                    m_ActivableObjectsArray[i].SetActive(false);
            }
        }
    }
}
