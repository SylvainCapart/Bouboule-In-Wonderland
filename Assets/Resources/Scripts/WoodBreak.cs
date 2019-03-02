using UnityEngine;

public class WoodBreak : MonoBehaviour
{
    [SerializeField] private float m_BreakSpeed = 7f;
    private Animator m_Anim;
    private BoxCollider2D m_BoxCollider;
    private CameraShake m_CameraShake;
    private AudioManager m_AudioManager;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Anim == null)
            m_Anim = GetComponent<Animator>();
        if (m_BoxCollider == null)
            m_BoxCollider = GetComponent<BoxCollider2D>();

        m_CameraShake = CameraShake.instance;

        m_AudioManager = AudioManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Mathf.Abs(collision.gameObject.GetComponent<Rigidbody2D>().velocity.x) > m_BreakSpeed)
            {
                if (collision.gameObject.transform.position.x > transform.position.x)
                {
                    m_Anim.SetBool("BrokenRight", true);
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2(0f, 3f);
                }
                else
                {
                    m_Anim.SetBool("BrokenLeft", true);
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity -= new Vector2(0f, 3f);
                }



                m_BoxCollider.enabled = false;
                m_CameraShake.Shake(0.2f, 0.2f);
                m_AudioManager.PlaySound("WoodBreak");

                Destroy(this.gameObject, 0.5f);
            }
        }
    }
}
