using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementRemote : MonoBehaviour
{

    public CharacterController2D controller;
    [SerializeField] private ClickedEnemyAI m_EnemyAI;
    private bool m_IsMovementAllowed = true;

    private float m_speedCoeff = 40f;

    float m_horizontalMove = 0f;
    float m_verticalMove = 0f;

    [SerializeField] private bool m_Jump = false;
    [SerializeField] private bool m_Roll = false;
    [SerializeField] private bool m_Charging = false;
    [SerializeField] private bool m_Swim = false;
    [SerializeField] private bool m_Climb = false;

    private bool m_FacingRight = true;

    [SerializeField] private Transform m_LeftPoint;
    [SerializeField] private Transform m_RightPoint;
    [SerializeField] private float m_ReachTol = 0.2f;
    private Transform m_CurrentFocus;

    public bool IsMovementAllowed
    {
        get
        {
            return m_IsMovementAllowed;
        }

        set
        {
            if (m_IsMovementAllowed == value) return;
            if (value == false)
            {
                m_Jump = false;
                m_Roll = false;
                m_Charging = false;
                m_Swim = false;
                m_Climb = false;
                m_horizontalMove = 0f;
                m_verticalMove = 0f;
            }
            m_IsMovementAllowed = value;

        }
    }

    private void Start()
    {
        m_CurrentFocus = m_LeftPoint;
    }

    void FixedUpdate()
    {

        if (m_EnemyAI.GetTarget.targettransform == m_LeftPoint && m_EnemyAI.State == ClickedEnemyAI.EnemyState.SLEEP)
        {
            m_horizontalMove = -40f;
            m_CurrentFocus = m_LeftPoint;

        }
        else if (m_EnemyAI.GetTarget.targettransform == m_RightPoint && m_EnemyAI.State == ClickedEnemyAI.EnemyState.SLEEP)
        {
            m_horizontalMove = 40f;
            m_CurrentFocus = m_RightPoint;
        }

        if (Vector2.Distance(transform.position, m_CurrentFocus.position) <= m_ReachTol)
        {
            m_horizontalMove = 0f;
        }
        // Move our character
        controller.Move(m_horizontalMove * Time.fixedDeltaTime, m_verticalMove * Time.fixedDeltaTime, m_Jump, m_Roll, m_Charging, m_Climb, m_Swim);

        m_Jump = false;
    }

    public void RollPlayer(bool roll)
    {
        m_Roll = roll;
    }


    private void FlipScale()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }


}
