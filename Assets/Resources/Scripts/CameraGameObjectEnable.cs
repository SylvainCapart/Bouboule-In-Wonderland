using UnityEngine;

public class CameraGameObjectEnable : MonoBehaviour
{
    [SerializeField] private GameObject[] m_ActivableObjectsArray;
    [SerializeField] private float m_ActivationDistance = 12f;


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
                m_ActivableObjectsArray[i].SetActive(true);
            }
        }
    }
}
