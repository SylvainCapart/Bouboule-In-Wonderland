using UnityEngine;

public class AlgaHide : MonoBehaviour
{
    public bool m_Hidden; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Alga")
        {
            m_Hidden = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Alga")
        {
            m_Hidden = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Alga")
        {
            m_Hidden = false;
        }
    }
}
