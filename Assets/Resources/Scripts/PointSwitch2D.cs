using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]

public class PointSwitch2D : MonoBehaviour
{

    public Transform[] points;
    private int targetPointIndex = 0;
    [SerializeField] private float m_MoveSpeed;
    private Rigidbody2D m_Rigidbody2D;
    private Vector3 velocity = Vector3.zero;
    private bool m_FacingRight = false;
    public bool m_Randomize;
    private int[] m_validChoices;
    [SerializeField] private bool m_ReverseScale;
    [SerializeField] private bool m_StartRight;
    [SerializeField] private bool m_PlayerFollowsMvt;

    private void Start()
    {
        m_FacingRight = m_StartRight;

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

    private void Update()
    {
        Vector3 dirVector = Vector3.zero;
        int lastIndex;

        dirVector = (points[targetPointIndex].position - this.transform.position);
        dirVector.Normalize();

        MoveTo(dirVector);

        if (Vector2.Distance(points[targetPointIndex].position, this.transform.position) < 0.1f)
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
            if (m_PlayerFollowsMvt)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null && player.transform.parent != null)
                    player.transform.parent = null;

            }


        }

        if (m_ReverseScale)
        {
            if (!m_FacingRight && this.transform.position.x - points[targetPointIndex].position.x > 0)
                Flip();
            else if (m_FacingRight && this.transform.position.x - points[targetPointIndex].position.x < 0)
                Flip();
        }

    }

    public void MoveTo(Vector3 dir)
    {
        transform.position += dir * m_MoveSpeed * Time.deltaTime;
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
