using UnityEngine;

public class GroundSecurityCheck : MonoBehaviour
{
    private Vector3 m_LastValidPos;
    private bool m_ValidPos = true;
    private Rigidbody2D m_Rb;

    private void Start()
    {
        m_LastValidPos = transform.position;
        if (m_Rb == null)
            m_Rb = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //after resolving physics, check if the player is not in the ground
        if (m_ValidPos != true)
        {
            transform.position = m_LastValidPos;
            m_Rb.velocity = Vector2.zero;
        }
        else
        {

            m_LastValidPos = transform.position;


        }
        m_ValidPos = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            m_ValidPos = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {


        if (collision.gameObject.tag == "Ground")
        {
            m_ValidPos = false;
        }
    }

}
