using UnityEngine;

public class SawDamage : MonoBehaviour
{

    [SerializeField] private int m_SawDamage = 30;
    private CircleCollider2D m_CircleCollider;

    private void Start()
    {
        if (m_CircleCollider == null)
        m_CircleCollider = GetComponent<CircleCollider2D>();
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().DamagePlayer(m_SawDamage);
        }
    }


}
