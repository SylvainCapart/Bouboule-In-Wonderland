using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]

public class PointSwitch3D : MonoBehaviour {

    public Transform[] points;
    private int targetPointIndex = 0;
    public float moveSpeed = 40f;
    private Rigidbody2D m_Rigidbody2D;
    [Range(0, .5f)][SerializeField] private float m_AxisAdjustment = 0f;
    private Vector3 velocity = Vector3.zero;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    private bool m_FacingRight = false;
    [SerializeField] private bool m_isObjectFlying = false;
    public bool m_Randomize;
    [SerializeField]
    private int[] m_validChoices;


    private void Start()
    {
        if (points.Length < 2)
            Debug.LogError("Number of points unsufficient or points non assigned");

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        if (m_Rigidbody2D == null)
            Debug.LogError(this.name + " : RB not found");

        if (m_Rigidbody2D != null)
        {
            m_Rigidbody2D.gravityScale = 0f;
        }

        if (m_Randomize && points.Length < 3)
        {
            Debug.Log("Not enough points to randomize path");
        }

        if (m_Randomize)
        {
            m_validChoices = new int[points.Length - 1];
            for (int i = 0; i < points.Length - 1; ++i)
            {
                m_validChoices[i] = i + 1;
            }
        }
    }

    private void FixedUpdate()
    {
        float horizontalMove = 0f;
        float verticalMove = 0f;
        int lastIndex;

        if (m_isObjectFlying || Mathf.Abs(points[targetPointIndex].position.x - this.transform.position.x) > m_AxisAdjustment)
        {
            horizontalMove = ((points[targetPointIndex].position.x - this.transform.position.x) / Mathf.Abs(points[targetPointIndex].position.x - this.transform.position.x));
        }

        if (m_isObjectFlying || Mathf.Abs(points[targetPointIndex].position.y - this.transform.position.y) > m_AxisAdjustment)
        {
            verticalMove = ((points[targetPointIndex].position.y - this.transform.position.y) / Mathf.Abs(points[targetPointIndex].position.y - this.transform.position.y));
        }


        Move(horizontalMove * moveSpeed * Time.fixedDeltaTime, verticalMove * moveSpeed * Time.fixedDeltaTime);
        //Vector2 checkDistance = new Vector2(Vector2.Distance(points[targetPointIndex].position, this.transform.position), 0);

        if (Vector2.Distance(points[targetPointIndex].position, this.transform.position) < 0.3f)
        {
            if (m_Randomize)
            {
                lastIndex = targetPointIndex;
                targetPointIndex = GetRandomTagetIndex();
                m_validChoices[GetValueIndex(targetPointIndex)] = lastIndex;

            }
            else
            {
                if (targetPointIndex >= points.Length - 1)
                {
                    targetPointIndex = 0;

                }
                else
                {
                    ++targetPointIndex;
                }
            }

        }

        if (!m_FacingRight && this.transform.position.x - points[targetPointIndex].position.x > 0)
            Flip();
        else if (m_FacingRight && this.transform.position.x - points[targetPointIndex].position.x < 0)
            Flip();
        else
            {/* Do nothing */}



        }


    public void Move(float moveX, float moveY)
    {

        Vector3 targetVelocity = new Vector2(moveX * 10f, moveY * 10);

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

    private int GetRandomTagetIndex()
    {
        return m_validChoices[(Random.Range(0, points.Length - 1))];
    }

    private int GetValueIndex(int value)
    {
        for (int i = 0; i < m_validChoices.Length; i++)
        {
            if (m_validChoices[i] == value)
                return i;

        }
        return 0;
    }
}
