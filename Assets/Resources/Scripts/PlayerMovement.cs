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

        /*if (m_swimmingTrigger.IsTouchingLayers(m_WhatIsWater))
        {
            m_swim = !m_swim;

            Debug.Log("SWIM changed  : " + m_swim);
            StartCoroutine(DisableSwimFor(m_SwimShutoffDelay));
        }*/


        m_horizontalMove = Input.GetAxisRaw("Horizontal") * m_speedCoeff;

        m_verticalMove = Input.GetAxisRaw("Vertical") * m_speedCoeff;

        //Debug.Log(m_horizontalMove + " " + m_verticalMove);
        if (m_swim)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, GetSwimAngle(m_horizontalMove, m_verticalMove)));
            /*
            if (m_verticalMove > 0.1f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 ));
                
                /*if (transform.rotation.z < 0.6f)
                    transform.Rotate(Vector3.forward * Time.deltaTime * 150);
            }
            else if (m_verticalMove < -0.1f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            }

            if (m_horizontalMove > 0.1f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                /*if (transform.rotation.z < 0.6f)
                    transform.Rotate(Vector3.forward * Time.deltaTime * 150);
            }
            else if (m_horizontalMove < -0.1f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            }*/


        }



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

    void OnTriggerStay2D(Collider2D other)
    {
        //Debug.LogError("other name : " + other.gameObject.name);
        if (other.gameObject.tag == "Water")
        {
            //m_verticalMove = Input.GetAxisRaw("Vertical") * m_speedCoeff;
            Debug.Log("SWIM :  :  STAY");
            m_swim = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //Debug.LogError("other name : " + other.gameObject.name);
        if (other.gameObject.tag == "Water")
        {
            //m_verticalMove = Input.GetAxisRaw("Vertical") * m_speedCoeff;
            Debug.Log("SWIM :EXIT : ");
            m_swim = false;
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

        // If the input is moving the player right and the player is facing left...
        if (x >= 0 && !m_FacingRight && IsPlayerMoving(m_horizontalMove, m_verticalMove))
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (x < 0 && m_FacingRight && IsPlayerMoving(m_horizontalMove, m_verticalMove))
        {
            // ... flip the player.
            Flip();
        }

        return retAngle;
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

    bool IsPlayerMoving(float x, float y)
    {
        return (Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f);
    }
}
