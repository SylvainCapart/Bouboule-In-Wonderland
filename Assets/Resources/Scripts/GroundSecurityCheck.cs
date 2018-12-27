using UnityEngine;

public class GroundSecurityCheck : MonoBehaviour
{
    private Vector3 m_LastValidPos;
    private bool m_ValidPos = true;
    private Rigidbody2D m_Rb;

    private void Start()
    {
        m_LastValidPos = transform.parent.position;
        if (m_Rb == null)
            m_Rb = GetComponentInParent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            m_ValidPos = false; 
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            m_ValidPos = false;
        }
    }

    private void FixedUpdate()
    {
        if (m_ValidPos != true)
        {
            //m_Rb.velocity = Vector2.zero;
        }
    }

    private void LateUpdate()
    {
        //after resolving physics, check if the player is not in the ground
        if (m_ValidPos != true)
        {
            Debug.LogError("NOK");
            transform.parent.position = m_LastValidPos;
            m_Rb.velocity = Vector2.zero;
        }
        else
        {

            m_LastValidPos = transform.parent.position;


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
