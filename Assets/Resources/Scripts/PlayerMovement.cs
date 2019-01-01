using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;

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


    private void Start()
    {
        if (m_climbingCollider == null)
            Debug.LogError(gameObject.name + " : m_climbingCollider is not assigned");
    }



    // Update is called once per frame
    void Update()
    {

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



        //Get the mouse position on the screen and send a raycast into the game world from that position.
        //Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //RaycastHit2D hit = Physics2D.Raycast(this.transform.position, this.transform.position, detect, m_WhatIsWater);
        //Debug.DrawLine(this.transform.position, worldPoint, Color.red);
        //Debug.Log("end : " + Input.mousePosition);
        //If something was hit, the RaycastHit2D.collider will not be null.

        /*
        
        if (m_Swim)
        {


            //m_flipAngleY = 

            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, GetSwimAngle(m_horizontalMove, m_verticalMove)));
            // If the input is moving the player right and the player is facing left...
            if (m_horizontalMove >= 0 && !m_FacingRight && IsPlayerMoving(m_horizontalMove, m_verticalMove))
            {
                FlipScale();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (m_horizontalMove < 0 && m_FacingRight && IsPlayerMoving(m_horizontalMove, m_verticalMove))
            {
                FlipScale();
            }

        }

    */

        m_horizontalMove = Input.GetAxisRaw("Horizontal") * m_speedCoeff;

        m_verticalMove = Input.GetAxisRaw("Vertical") * m_speedCoeff;


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


    void OnTriggerExit2D(Collider2D other)
    {

        if (other.gameObject.tag == "Water" )
        {
            if (this.transform.position.y > other.bounds.center.y + other.bounds.extents.y)
            {
                m_Swim = false;
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 0f));
            }  
            else
            {
                m_Swim = true;
            }
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
