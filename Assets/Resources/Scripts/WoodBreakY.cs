using UnityEngine;

public class WoodBreakY : MonoBehaviour
{
    [SerializeField] private float m_BreakSpeed = 10f;
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

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector2 speed = collision.gameObject.GetComponent<Rigidbody2D>().velocity;

            if (Mathf.Abs(speed.y) > m_BreakSpeed)
            {
                if (collision.gameObject.transform.position.y > transform.position.y)
                {
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2(0f, 3f);
                    m_Anim.SetBool("BrokenTop", true);
                }
                else
                {
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity -= new Vector2(0f, 3f);
                    m_Anim.SetBool("BrokenBottom", true);
                }



                m_BoxCollider.enabled = false;
                m_CameraShake.Shake(0.2f, 0.2f);
                m_AudioManager.PlaySound("WoodBreak");



                Destroy(this.gameObject, 0.5f);

            }
        }
    }



}
