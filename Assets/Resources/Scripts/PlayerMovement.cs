using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;

    private float m_speedCoeff = 40f;

    float m_horizontalMove = 0f;
    float m_verticalMove = 0f;

    [SerializeField] private bool m_jump = false;
    [SerializeField] private bool m_roll = false;
    [SerializeField] private bool m_charging = false;
    [SerializeField] private bool m_swim = false;

    [SerializeField] private Collider2D m_climbingCollider;                // A collider that detects objects where climbing is possible
    [SerializeField] private Collider2D m_swimmingTrigger;
    [SerializeField] private bool m_climb = false;
    [SerializeField] private LayerMask m_WhatIsVine;                            // A mask determining what is vine to the character     
    [SerializeField] private LayerMask m_WhatIsWater;                            // A mask determining what is water to the character     

    [SerializeField] private float m_SwimShutoffDelay = 0.3f;                            // Deactivate swim after immersion
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
        Collider2D[] colliders;

        if (m_climbingCollider.IsTouchingLayers(m_WhatIsVine))
        {
            // m_verticalMove = Input.GetAxisRaw("Vertical") * m_speedCoeff;
            m_climb = true;
        }
        else
        {
            //m_verticalMove = 0f;
            m_climb = false;

        }

        colliders = Physics2D.OverlapCircleAll(this.transform.position, 0.01f, m_WhatIsWater);

        for (int i = 0; i < colliders.Length; i++)
        {
            // Debug.Log("colliders : " + colliders[i].name);
        }


        //Get the mouse position on the screen and send a raycast into the game world from that position.
        //Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //RaycastHit2D hit = Physics2D.Raycast(this.transform.position, this.transform.position, detect, m_WhatIsWater);
        //Debug.DrawLine(this.transform.position, worldPoint, Color.red);
        //Debug.Log("end : " + Input.mousePosition);
        //If something was hit, the RaycastHit2D.collider will not be null.

        /*
        
        if (m_swim)
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



        //if (m_swimmingTrigger.IsTouchingLayers(m_WhatIsWater))

        


        m_horizontalMove = Input.GetAxisRaw("Horizontal") * m_speedCoeff;

        m_verticalMove = Input.GetAxisRaw("Vertical") * m_speedCoeff;
        float m_flipAngleY = transform.rotation.y;
        //Debug.Log(m_horizontalMove + " " + m_verticalMove);




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


        //m_swim = !m_swim;
        if (!m_swim && m_LastSwimStatus)
        {
            m_jump = true;
            m_verticalMove = 30;
        }
        m_LastSwimStatus = m_swim;

    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(m_horizontalMove * Time.fixedDeltaTime, m_verticalMove * Time.fixedDeltaTime, m_jump, m_roll, m_charging, m_climb, m_swim);
        m_jump = false;


    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.LogError("other name : " + other.gameObject.name);
        if (other.gameObject.tag == "Water" && this.transform.position.y < other.gameObject.GetComponentInChildren<WaterLevel>().WaterLevelPosition.position.y)
        {
            
            //Debug.Log("SWIM : NEAZAZAZA :  STAY");
            m_swim = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Water" ) //&& m_swimmingTrigger.IsTouchingLayers(m_WhatIsWater))
        {
            if (this.transform.position.y > other.gameObject.GetComponentInChildren<WaterLevel>().WaterLevelPosition.position.y)
            {
                m_swim = false;
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 0f));
            }  
            else
            {
                m_swim = true;
            }
        }
    }

    IEnumerator DisableSwimFor(float deactivateTime)
    {
        m_swimmingTrigger.enabled = false;
        yield return (new WaitForSeconds(deactivateTime));
        m_swimmingTrigger.enabled = true;
    }

    float GetSwimAngle(float x, float y)
    {
        float moveDetected = 0.0f;
        float retAngle = 0f;

        if (x > moveDetected && y > moveDetected) //up right
            retAngle = 45f;
        else if (x > moveDetected && y == 0f) // right
            retAngle = 0f;
        else if (x > moveDetected && y < moveDetected) // bottom right
            retAngle = -45f;
        else if (x == 0f && y < moveDetected) // bottom
        {
            retAngle = -90f;
        }
        else if (x < moveDetected && y < moveDetected) // bottom left
            retAngle = 45f;
        else if (x < moveDetected && y == 0f) // left
            retAngle = 0f;
        else if (x < moveDetected && y > moveDetected) // up left
            retAngle = -45f;
        else if (x == 0f && y > moveDetected) // up
        {
            retAngle = 90f;
        }
        else if (x == 0f && y == 0f) // no move
            retAngle = 0f;
        else
        {
            retAngle = 0f;
            Debug.LogError("angle not possible");
        }

        

        return retAngle;
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


    bool IsPlayerMoving(float x, float y)
    {
        return (Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f);
    }
}
