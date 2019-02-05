using System.Collections;
using UnityEngine;

public class SawDamage : MonoBehaviour
{

    [SerializeField] private int m_SawDamage = 30;
    [SerializeField] private float m_SawDamageDelay = 0.5f;
    private CircleCollider2D m_CircleCollider;
    private bool m_IsDamaging = false;

    private void Start()
    {
        if (m_CircleCollider == null)
        m_CircleCollider = GetComponent<CircleCollider2D>();
    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!m_IsDamaging && collision.gameObject.tag == "Player")
        {
            StartCoroutine(Damage(collision));
        }
    }

    private IEnumerator Damage(Collision2D collision)
    {
        m_IsDamaging = true;
        collision.gameObject.GetComponent<Player>().DamagePlayer(m_SawDamage);
        yield return new WaitForSeconds(m_SawDamageDelay);
        m_IsDamaging = false;
    }


}
