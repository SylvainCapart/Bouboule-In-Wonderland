using UnityEngine;

public class PointSwitch : MonoBehaviour {

    public Transform[] points;
    private int targetPointIndex = 0;
    public float moveSpeed = 40f;
    private Rigidbody2D m_Rigidbody2D;
    private Vector3 velocity = Vector3.zero;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    private bool m_FacingRight = false;
    private const float EPSILON = 0.01f;


    private void Start()
    {
        if (points.Length < 2)
            Debug.LogError("Number of points unsufficient or points non assigned");

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        if (m_Rigidbody2D == null)
            Debug.LogError(this.name + " : RB not found");
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        float horizontalMove;
        if (System.Math.Abs(points[targetPointIndex].position.x - this.transform.position.x) > EPSILON)
        {
            horizontalMove = ((points[targetPointIndex].position.x - this.transform.position.x) / Mathf.Abs(points[targetPointIndex].position.x - this.transform.position.x));
        }
        else
        {
            horizontalMove = 0f;
        }

        Move(horizontalMove * moveSpeed * Time.fixedDeltaTime);
        //Vector2 checkDistance = new Vector2(Vector2.Distance(points[targetPointIndex].position, this.transform.position), 0);

        if (Vector2.Distance(points[targetPointIndex].position, this.transform.position) < 0.3f)
        {
            if (targetPointIndex >= points.Length - 1)
            {
                targetPointIndex = 0;
            }
            else
            {
                ++targetPointIndex;
            }
            Flip();
        }


    }


    public void Move(float move)
    {
        
        Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
        
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
