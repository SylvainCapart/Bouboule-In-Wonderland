using System.Collections;
using UnityEngine;

public class Portal2Points : MonoBehaviour
{
    [SerializeField] private Transform m_TargetDoor;
    private bool m_IsDoorActive = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
         if (collision.tag == "Player" && m_IsDoorActive)
        {
            //StartCoroutine(DoorShutOff(m_TargetDoor.GetComponent<CapsuleCollider2D>(), 1f));
            m_TargetDoor.GetComponent<Portal2Points>().m_IsDoorActive = false;
            collision.transform.position = m_TargetDoor.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        m_IsDoorActive = true;
    }

    private IEnumerator DoorShutOff(CapsuleCollider2D portal, float delay)
    {
        portal.enabled = false;
        yield return new WaitForSeconds(delay);
        portal.enabled = true;
    }
}
