using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;
    private bool m_IsMovementAllowed = true;

    private float m_speedCoeff = 40f;

    float m_horizontalMove = 0f;
    float m_verticalMove = 0f;

    [SerializeField] private bool m_Jump = false;
    [SerializeField] private bool m_Roll = false;
    [SerializeField] private bool m_Charging = false;
    [SerializeField] private bool m_Swim = false;
    [SerializeField] private bool m_Climb = false;

    [SerializeField] private Collider2D m_climbingCollider;                // A collider that detects objects where climbing is possible
    [SerializeField] private Collider2D m_swimmingCollider;
    [SerializeField] private LayerMask m_WhatIsVine;                            // A mask determining what is vine to the character     
    [SerializeField] private LayerMask m_WhatIsWater;                            // A mask determining what is water to the character     

    private bool m_LastSwimStatus = false;
    private bool m_FacingRight = true;

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
        if (m_climbingCollider == null)
            Debug.LogError(gameObject.name + " : m_climbingCollider is not assigned");
    }



    // Update is called once per frame
    void Update()
    {
        if (!IsMovementAllowed) return;

        if (m_climbingCollider.IsTouchingLayers(m_WhatIsVine))
        {
            // m_verticalMove = Input.GetAxisRaw("Vertical") * m_speedCoeff;
            m_Climb = true;
        }
        else
        {
            //m_verticalMove = 0f;
            m_Climb = false;

        }

        m_horizontalMove = Input.GetAxisRaw("Horizontal") * m_speedCoeff;

        m_verticalMove = Input.GetAxisRaw("Vertical") * m_speedCoeff;



        if (Input.GetKey(KeyCode.LeftControl) && (Input.GetKey(KeyCode.LeftArrow)))
        {
            Debug.Log("Crtl");
            m_horizontalMove = Input.GetAxisRaw("Horizontal") * m_speedCoeff;

            m_verticalMove = Input.GetAxisRaw("Vertical") * m_speedCoeff;
        }


        if (Input.GetButtonDown("Roll"))
        {
            m_Roll = true;
        }
        else if (Input.GetButtonUp("Roll"))
        {
            m_Roll = false;
        }

        m_Charging = Input.GetButton("Roll");

        if (Input.GetButtonDown("Jump"))
        {
            m_Jump = true;
        }

        if (!m_Swim && m_LastSwimStatus)
        {
            m_Jump = true;
            m_verticalMove = 30;
        }
        m_LastSwimStatus = m_Swim;




    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(m_horizontalMove * Time.fixedDeltaTime, m_verticalMove * Time.fixedDeltaTime, m_Jump, m_Roll, m_Charging, m_Climb, m_Swim);

        m_Jump = false;
    }

    public void RollPlayer(bool roll)
    {
        m_Roll = roll;
    }


    void OnTriggerExit2D(Collider2D other)
    {

        if (other.gameObject.tag == "Water")
        {

            m_Swim = false;
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 0f));
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {

        if (other.gameObject.tag == "Water")
        {
      
                m_Swim = true;

        }

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
