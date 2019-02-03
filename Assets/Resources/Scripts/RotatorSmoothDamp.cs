using System.Collections;
using UnityEngine;

public class RotatorSmoothDamp : MonoBehaviour
{
    [SerializeField] private float m_MinAngle;
    [SerializeField] private float m_MaxAngle;
    [SerializeField] private float m_Delay;
    private float m_OriginAngle;
    private float m_TargetAngle;
    public Transform target;
    private bool m_IsRotating;
    private const float EPSILON = 0.01f;
    [Range(0, 1f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    private float m_Angle;



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

        /*for (float t = 0.0f; t < m_Delay; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, (m_OriginAngle - m_TargetAngle) * (1 - t / m_Delay) + m_TargetAngle));
            yield return null;
        }*/

        //transform.rotation = Vector3.SmoothDamp(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z), new Vector3(m_TargetAngle.x, m_TargetAngle.y, m_TargetAngle.z), ref m_Angle, m_MovementSmoothing);
        float newangle = Mathf.SmoothDampAngle(transform.eulerAngles.z, target.eulerAngles.z, ref m_Angle, m_MovementSmoothing);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, newangle));

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
        yield return null;
    }
}
