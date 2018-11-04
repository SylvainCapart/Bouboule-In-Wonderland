using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;

	public float m_runSpeed = 40f;

	float m_horizontalMove = 0f;
    float m_verticalMove = 0f;

    [SerializeField] private bool m_jump = false;
    [SerializeField] private bool m_roll = false;
    [SerializeField] private bool m_charging = false;

    [SerializeField] private Collider2D m_climbingCollider;                // A collider that detects objects where climbing is possible
    [SerializeField] private bool m_climb = false;
    [SerializeField] private LayerMask m_WhatIsVine;                            // A mask determining what is vine to the character     


    private void Start()
    {
        if (m_climbingCollider == null)
            Debug.LogError(gameObject.name + " : m_climbingCollider is not assigned");
    }

    // Update is called once per frame
    void Update () {

        if (m_climbingCollider.IsTouchingLayers(m_WhatIsVine))
        {
            m_verticalMove = Input.GetAxisRaw("Vertical") * m_runSpeed;
            m_climb = true;
        }
        else
        {
            m_verticalMove = 0f;
            m_climb = false;
        }

        m_horizontalMove = Input.GetAxisRaw("Horizontal") * m_runSpeed;
       

        if (Input.GetButtonDown("Roll"))
        {
            m_roll = true;
        }
        else if (Input.GetButtonUp("Roll"))
        {
            m_roll = false;
        }

        m_charging = Input.GetButton("Roll");

        if (Input.GetButtonDown("Jump"))
		{
			m_jump = true;
		}


    }

	void FixedUpdate ()
	{
        // Move our character
		controller.Move(m_horizontalMove * Time.fixedDeltaTime, m_verticalMove * Time.fixedDeltaTime, m_jump, m_roll, m_charging, m_climb);
		m_jump = false;
	}
}
