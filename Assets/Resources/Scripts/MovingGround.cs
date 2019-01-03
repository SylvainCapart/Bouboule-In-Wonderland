
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    private GameObject m_Player;
    private Rigidbody2D m_Rigidbody2D;
    [SerializeField] private PointSwitch2D m_PointSwitch;

    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        if (m_Rigidbody2D == null)
            Debug.LogError(this.name + " : RB not found");
        if (m_PointSwitch == null)
            m_PointSwitch = GetComponent<PointSwitch2D>();
    }

    void FixedUpdate()
    {
        if (m_Player != null)
        {
            m_Player.GetComponent<Rigidbody2D>().AddForce(m_Rigidbody2D.velocity * m_PointSwitch.moveSpeed * 2);//* moveSpeed );
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            m_Player = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            m_Player = null;

        }
    }


}
