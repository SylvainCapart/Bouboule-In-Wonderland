using System.Collections;
using UnityEngine;
using UnityStandardAssets._2D;

public class CameraFocus : MonoBehaviour
{
    [SerializeField] private Camera2DFollow m_CameraFollow;
    [SerializeField] private Transform m_NewTarget;
    [SerializeField] private float m_FocusDuration;
    [SerializeField] private bool m_DampingShutOff = true;
    [SerializeField] private float m_SpeedFreezeDuration;
    private bool m_Triggered = false;

    private bool m_IsFocusing;
    private bool m_IsSpeedFrozen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !m_Triggered)
        {
            if (!m_IsFocusing)
                StartCoroutine(TemporaryFocus(m_NewTarget, m_FocusDuration));

            if (!m_IsSpeedFrozen)
                StartCoroutine(SpeedShutOff(collision, m_SpeedFreezeDuration));

            AudioManager.instance.PlaySound("InceptionStab");
            m_Triggered = true;
        }

    }

    private IEnumerator TemporaryFocus(Transform target, float delay)
    {
        m_IsFocusing = true;

        Transform oldTarget = m_CameraFollow.target;
        m_CameraFollow.target = target;
        if (m_DampingShutOff)
            StartCoroutine(m_CameraFollow.DampingShutOff(m_FocusDuration));

        yield return new WaitForSeconds(delay);

        m_CameraFollow.target = oldTarget;
        m_IsFocusing = false;
        if (m_FocusDuration > m_SpeedFreezeDuration)
            Destroy(this.gameObject);
    }

    private IEnumerator SpeedShutOff(Collider2D collision, float delay)
    {
        m_IsSpeedFrozen = true;

        Vector2 oldspeed = collision.attachedRigidbody.velocity;
        float oldgravity = collision.attachedRigidbody.gravityScale;
        collision.attachedRigidbody.velocity = Vector2.zero;
        collision.attachedRigidbody.gravityScale = 0f;

        collision.gameObject.GetComponent<PlayerMovement>().IsMovementAllowed = false;

        yield return new WaitForSeconds(delay);
        collision.attachedRigidbody.velocity = oldspeed;
        collision.attachedRigidbody.gravityScale = oldgravity;
        collision.gameObject.GetComponent<PlayerMovement>().IsMovementAllowed = true;

        m_IsSpeedFrozen = false;
        if (m_FocusDuration <= m_SpeedFreezeDuration)
            Destroy(this.gameObject);
    }

}
