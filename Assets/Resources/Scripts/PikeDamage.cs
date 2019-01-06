using UnityEngine;

public class PikeDamage : MonoBehaviour
{

    [SerializeField] private int m_PikeDamage = 50;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().DamagePlayer(m_PikeDamage);
        }
    }


}
