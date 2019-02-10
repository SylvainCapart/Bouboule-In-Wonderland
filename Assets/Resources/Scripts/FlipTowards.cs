
using UnityEngine;

public class FlipTowards : MonoBehaviour
{
    public Transform m_Target;
    private bool m_FacingRight = false;
    private string m_TargetName = "";

    private void Start()
    {
        if (m_Target != null)
            m_TargetName = m_Target.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Target == null)
        {
            GameObject player = GameObject.Find("Player");
            if (player != null)
                m_Target = player.transform;
        }
        else
        {
            if (!m_FacingRight && m_Target.position.x - this.transform.position.x > 0)
            {
                FlipRotate();
            }
            else if (m_FacingRight && m_Target.position.x - this.transform.position.x < 0)
            {
                FlipRotate();
            }
        }
        
    }

    private void FlipRotate()
    {
        transform.Rotate(new Vector3(0, 180, 0));
        m_FacingRight = !m_FacingRight;
    }

    private void FlipScale()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
