using UnityEngine;

public class GravityCheck : MonoBehaviour
{
    [SerializeField] private float m_GravityTarget;


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Reinit");
            //collision.GetComponent<Rigidbody2D>().gravityScale = m_GravityTarget;
            collision.GetComponent<CharacterController2D>().m_GroundDir = CharacterController2D.GroundDirection.BOTTOM;
        }
    }
}
