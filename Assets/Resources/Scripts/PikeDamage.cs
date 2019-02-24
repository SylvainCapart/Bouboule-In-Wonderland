using UnityEngine;

public class PikeDamage : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().DamagePlayer(PlayerStats.Instance.m_MaxHealth/2);
        }
    }
}
