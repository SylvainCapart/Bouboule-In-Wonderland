using System.Collections;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float m_MinAngle;
    [SerializeField] private float m_MaxAngle;
    [SerializeField] private float m_Delay;
    private float m_OriginAngle;
    private float m_TargetAngle;
    private bool m_IsRotating;
    private const float EPSILON = 0.01f;


    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, m_MinAngle));
        m_OriginAngle = m_MinAngle;
        m_TargetAngle = m_MaxAngle;

    }

    // Update is called once per frame
    void Update()
    {
        if (!m_IsRotating)
            StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        m_IsRotating = true;

        for (float t = 0.0f; t < m_Delay; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0,  (m_OriginAngle - m_TargetAngle) * (1 - t / m_Delay) + m_TargetAngle));
            yield return null;
        }

        if (Mathf.Abs(m_MaxAngle - m_TargetAngle) < EPSILON)
        {
            m_TargetAngle = m_MinAngle;
            m_OriginAngle = m_MaxAngle;
        }
        else if (Mathf.Abs(m_MinAngle - m_TargetAngle) < EPSILON)
        {
            m_TargetAngle = m_MaxAngle;
            m_OriginAngle = m_MinAngle;
        }
        else
            Debug.LogError(name + " : unexpected angle detected");

        m_IsRotating = false;
    }
}
